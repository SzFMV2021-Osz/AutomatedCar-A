namespace AutomatedCar.SystemComponents.Sensors
{
    using System.Collections.Generic;
    using System.Linq;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets;
    using Avalonia;
    using Avalonia.Media;

    public sealed class Radar : Sensor
    {
        private readonly Dictionary<int, double> previousObjects;
        private PolylineGeometry laneGeometry;

        public Radar(VirtualFunctionBus virtualFunctionBus)
            : base(virtualFunctionBus, 60, 30)
        {
            this.previousObjects = new Dictionary<int, double>();
            this.sensorPacket = new RadarPacket();
            virtualFunctionBus.RadarPacket = (IRadarPacket)this.sensorPacket;
        }

        public override void Process()
        {
            AutomatedCar car = World.Instance.ControlledCar;

            this.CalculateSensorData(car, World.Instance.WorldObjects);
            this.CalculateRadarData(car);
        }

        protected override bool IsRelevant(WorldObject worldObject)
        {
            return worldObject.Collideable;
        }

        private void CalculateRadarData(AutomatedCar car)
        {
            this.CreateLaneGeometry();

            Dictionary<int, Point> permanentObjectsInRadar = this.GetPermanentObjectsInRadar();
            IList<WorldObject> closingObjects = this.GetClosingObjects(permanentObjectsInRadar, car);

            ((IRadarPacket)this.sensorPacket).ClosingObjects = closingObjects;
            ((IRadarPacket)this.sensorPacket).ClosestObjectInLane = this.GetClosestObjectInLane(closingObjects, car);
            this.SavePreviousObjectDistances(car);
        }

        private void SavePreviousObjectDistances(AutomatedCar car)
        {
            this.previousObjects.Clear();

            foreach (WorldObject currObj in this.sensorPacket.RelevantObjects)
            {
                this.previousObjects.Add(currObj.Id, DistanceBetween(new Point(currObj.X, currObj.Y), new Point(car.X, car.Y)));
            }
        }

        private Dictionary<int, Point> GetPermanentObjectsInRadar()
        {
            Dictionary<int, Point> permanentObjects = new ();
            foreach (WorldObject currObj in this.sensorPacket.RelevantObjects)
            {
                if (this.previousObjects.ContainsKey(currObj.Id))
                {
                    permanentObjects.Add(currObj.Id, new Point(currObj.X, currObj.Y));
                }
            }

            return permanentObjects;
        }

        private IList<WorldObject> GetClosingObjects(Dictionary<int, Point> objectsInRadar, AutomatedCar car)
        {
            IList<WorldObject> closingObjects = new List<WorldObject>();
            foreach (var currPoint in objectsInRadar)
            {
                double currDst = DistanceBetween(currPoint.Value, new Point(car.X, car.Y));
                if (currDst < this.previousObjects[currPoint.Key])
                {
                    closingObjects.Add(this.sensorPacket.RelevantObjects.Where(d => d.Id == currPoint.Key).FirstOrDefault());
                }
            }

            return closingObjects;
        }

        private void CreateLaneGeometry()
        {
            if (this.laneGeometry == null)
            {
                this.laneGeometry = this.GetSelfLaneGeometry();
            }
        }

        private PolylineGeometry GetSelfLaneGeometry()
        {
            IList<Point> points = new List<Point>()
            {
                new Point(0, this.sensorObject.RotationPoint.Y),
                new Point(2 * this.sensorObject.RotationPoint.X, this.sensorObject.RotationPoint.Y),
                new Point(2 * this.sensorObject.RotationPoint.X, this.sensorObject.RotationPoint.Y - this.distance),
                new Point(0, this.sensorObject.RotationPoint.Y - this.distance),
                new Point(0, this.sensorObject.RotationPoint.Y),
            };

            return new PolylineGeometry(points, true);
        }

        private bool IsObjectInLane(WorldObject currObject)
        {
            PolylineGeometry geometry = RotateRawGeometry(this.laneGeometry, this.sensorObject.RotationPoint, this.sensorObject.Rotation);
            geometry = ShiftGeometryWithWorldCoordinates(geometry, this.sensorObject.X, this.sensorObject.Y);

            foreach (var point in GetPoints(currObject))
            {
                if (geometry.FillContains(point))
                {
                    return true;
                }
            }

            return false;
        }

        private WorldObject GetClosestObjectInLane(IList<WorldObject> worldObjects, AutomatedCar car)
        {
            IList<WorldObject> incomingObjectsInLane = worldObjects.Where(c => this.IsObjectInLane(c)).ToList();
            return FindClosestObject(incomingObjectsInLane, car);
        }
    }
}