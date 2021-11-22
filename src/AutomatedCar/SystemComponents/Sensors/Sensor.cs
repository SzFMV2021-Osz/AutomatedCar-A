namespace AutomatedCar.SystemComponents.Sensors
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Helpers;
    using AutomatedCar.SystemComponents.Packets;
    using Avalonia;
    using Avalonia.Media;

    public abstract class Sensor : SystemComponent
    {
        protected ISensorPacket sensorPacket;
        protected WorldObject sensorObject;
        protected readonly int distance;
        private readonly int angleOfView;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sensor"/> class.
        /// </summary>
        /// <param name="virtualFunctionBus">vbs.</param>
        /// <param name="angleOfView">Angle of view in degree.</param>
        /// <param name="distance">Distance in meters.</param>
        public Sensor(VirtualFunctionBus virtualFunctionBus, int angleOfView, int distance)
            : base(virtualFunctionBus)
        {
            if (angleOfView < 0 || angleOfView > 360 || distance < 0)
            {
                throw new ArgumentOutOfRangeException("Sensor initialized with invalid " + (distance < 0 ? "distance" : "angleOfView"));
            }
            else
            {
                this.angleOfView = angleOfView;
                this.distance = distance * 50;  // 1 meter is counted as approx 50 pixel
            }
        }

        /// <summary>Gets or sets the sensor's location on car.</summary>
        public Point RelativeLocation { get; set; }

        public IList<Point> Points
        {
            get => this.DrawTriangle();
        }

        private static PolylineGeometry GetRawGeometry(int triangleBase, int distance)
        {
            List<Point> points = new ()
            {
                new Point(0, 0),
                new Point(2 * triangleBase, 0),
                new Point(triangleBase, distance),
                new Point(0, 0),
            };

            return new PolylineGeometry(points, true);
        }

        private PolylineGeometry GetGeometry()
        {
            PolylineGeometry geometry = Utils.RotateRawGeometry(this.sensorObject.RawGeometries[0], this.sensorObject.RotationPoint, this.sensorObject.Rotation);

            return Utils.ShiftGeometryWithWorldCoordinates(geometry, this.sensorObject.X, this.sensorObject.Y);
        }

        private IList<Avalonia.Point> DrawTriangle()
        {
            int triangleBase = (int)(this.distance * Math.Tan(Utils.ConvertToRadians(this.angleOfView / 2)));
            List<Point> points = new ()
            {
                new Point(0 + this.RelativeLocation.X, 0 + this.RelativeLocation.Y),
                new Point(-triangleBase + this.RelativeLocation.X, -this.distance + this.RelativeLocation.Y),
                new Point(triangleBase + this.RelativeLocation.X, -this.distance + this.RelativeLocation.Y),
                new Point(0 + this.RelativeLocation.X, 0 + this.RelativeLocation.Y),
            };

            return points;
        }

        protected void CalculateSensorData(AutomatedCar car, ObservableCollection<WorldObject> worldObjects)
        {
            this.SetSensor(car);
            this.FindObjectsInSensorArea(worldObjects);
            this.FilterRelevantObjects();
            this.sensorPacket.ClosestObject = Utils.FindClosestObject(this.sensorPacket.RelevantObjects, car);
        }

        protected abstract bool IsRelevant(WorldObject worldObject);

        private void SetSensor(AutomatedCar car)
        {
            int triangleBase = (int)(this.distance * Math.Tan(Utils.ConvertToRadians(this.angleOfView / 2)));

            if (this.sensorObject == null)
            {
                this.sensorObject = new WorldObject(car.X + car.RotationPoint.X, car.Y + car.RotationPoint.Y, "sensor.png");
                this.sensorObject.RawGeometries.Add(GetRawGeometry(triangleBase, this.distance));
                this.sensorObject.Collideable = false;
                this.sensorObject.Geometries.Add(this.GetGeometry());
            }
            else
            {
                this.sensorObject.X = car.X + car.RotationPoint.X;
                this.sensorObject.Y = car.Y + car.RotationPoint.Y;
                this.sensorObject.Geometries[0] = this.GetGeometry();
            }

            this.sensorObject.RotationPoint = new (triangleBase, this.distance + car.RotationPoint.Y - (int)this.RelativeLocation.Y);
            this.sensorObject.Rotation = car.Rotation;
        }

        private void FindObjectsInSensorArea(ObservableCollection<WorldObject> worldObjects)
        {
            List<WorldObject> detectedObjects = new ();

            foreach (WorldObject currObject in worldObjects)
            {
                foreach (Point point in Utils.GetPoints(currObject))
                {
                    if (this.sensorObject.Geometries[0].FillContains(point) && !detectedObjects.Contains(currObject))
                    {
                        detectedObjects.Add(currObject);
                    }
                }
            }

            this.sensorPacket.DetectedObjects = detectedObjects;
        }

        private void FilterRelevantObjects()
        {
            this.sensorPacket.RelevantObjects = this.sensorPacket.DetectedObjects.Where(wo => this.IsRelevant(wo)).ToList();
        }
    }
}