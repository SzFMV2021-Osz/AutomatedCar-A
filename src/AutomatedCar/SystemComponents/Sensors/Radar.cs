namespace AutomatedCar.SystemComponents.Sensors
{
    using System.Collections.Generic;
    using System.Linq;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets;
    using Avalonia;

    public sealed class Radar : Sensor
    {
        private readonly Dictionary<int, double> previousObjects;

        public Radar(VirtualFunctionBus virtualFunctionBus)
            : base(virtualFunctionBus, 60, 200)
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
            Dictionary<int, Point> permanentObjectsInRadar = this.GetPermanentObjectsInRadar();
            Dictionary<int, double> closingObjects = this.GetClosingObjects(permanentObjectsInRadar, car);

            this.SaveToPacket(closingObjects);
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

        private Dictionary<int, double> GetClosingObjects(Dictionary<int, Point> objectsInRadar, AutomatedCar car)
        {
            Dictionary<int, double> closingElements = new ();
            foreach (var currPoint in objectsInRadar)
            {
                double currDst = DistanceBetween(currPoint.Value, new Point(car.X, car.Y));
                if (currDst < this.previousObjects[currPoint.Key])
                {
                    closingElements.Add(currPoint.Key, this.previousObjects[currPoint.Key]);
                }
            }

            return closingElements;
        }

        private void SaveToPacket(Dictionary<int, double> id)
        {
            int closestObjectID = id.OrderBy(w => w.Value).FirstOrDefault().Key;

            ((IRadarPacket)this.sensorPacket).ClosingObjects.Clear();
            foreach (var currentID in id)
            {
                WorldObject currObj = this.sensorPacket.RelevantObjects
                    .Where(d => d.Id == currentID.Key)
                    .FirstOrDefault();
                ((IRadarPacket)this.sensorPacket).ClosingObjects.Add(currObj);

                if (currentID.Key == closestObjectID)
                {
                    ((IRadarPacket)this.sensorPacket).ClosestObjectInLane = currObj;
                }
            }
        }
    }
}