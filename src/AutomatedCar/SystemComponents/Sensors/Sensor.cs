namespace AutomatedCar.SystemComponents.Sensors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets;
    using Avalonia;
    using Avalonia.Media;

    public abstract class Sensor : SystemComponent, ISensor
    {
        protected ISensorPacket sensorPacket;
        private readonly int angleOfView;
        private readonly int distance;
        private PolylineGeometry sensorArea;

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
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                this.angleOfView = angleOfView;
                this.distance = distance * 50;  // 1 meter is 50 pixel
            }
        }

        public Point RelativeLocation { get; set; }

        protected void CalculateSensorArea(IAutomatedCar car)
        {
            double radius = this.distance * Math.Tan(ConvertToRadians(this.angleOfView / 2));
            double sin = Math.Sin(ConvertToRadians(car.Rotation));
            double cos = Math.Cos(ConvertToRadians(car.Rotation));

            Point location = new (car.X + this.RelativeLocation.X, car.Y + this.RelativeLocation.Y);

            Point pointToConvert0 = new (this.RelativeLocation.X - car.RotationPoint.X, this.RelativeLocation.Y - car.RotationPoint.Y);
            Point pointToConvert1 = new (this.RelativeLocation.X - radius - car.RotationPoint.X, this.RelativeLocation.Y - this.distance - car.RotationPoint.Y);
            Point pointToConvert2 = new (this.RelativeLocation.X + radius - car.RotationPoint.X, this.RelativeLocation.Y - this.distance - car.RotationPoint.Y);

            Point convertedPoint0 = new ((pointToConvert0.X * cos) - (pointToConvert0.Y * sin), (pointToConvert0.X * sin) + (pointToConvert0.Y * cos));
            Point convertedPoint1 = new ((pointToConvert1.X * cos) - (pointToConvert1.Y * sin), (pointToConvert1.X * sin) + (pointToConvert1.Y * cos));
            Point convertedPoint2 = new ((pointToConvert2.X * cos) - (pointToConvert2.Y * sin), (pointToConvert2.X * sin) + (pointToConvert2.Y * cos));

            List<Point> points = new ()
            {
                new Point(location.X + convertedPoint0.X, location.Y + convertedPoint0.Y),
                new Point(location.X + convertedPoint1.X, location.Y + convertedPoint1.Y),
                new Point(location.X + convertedPoint2.X, location.Y + convertedPoint2.Y),
                new Point(location.X + convertedPoint0.X, location.Y + convertedPoint0.Y),
            };

            this.sensorArea = new PolylineGeometry(points, true);
        }

        protected void FindObjectsInSensorArea(IEnumerable<IWorldObject> worldObjects, IAutomatedCar car)
        {
            foreach (IWorldObject currentObj in worldObjects)
            {
                foreach (Point point in GetPoints(currentObj))
                {
                    if (this.sensorArea.FillContains(point) && !this.sensorPacket.DetectedObjects.Contains(currentObj))
                    {
                        this.sensorPacket.DetectedObjects.Add(currentObj);
                    }
                }
            }

            this.sensorPacket.DetectedObjects.Remove(car);
        }

        protected void FindClosestObject(IAutomatedCar car)
        {
            Point carPoint = new (car.X, car.Y);
            IWorldObject closestObject = null;

            foreach (IWorldObject currObject in this.sensorPacket.RelevantObjects)
            {
                double minDistance = double.MaxValue;
                foreach (Point currPoint in GetPoints(currObject))
                {
                    double currDistance = DistanceBetween(carPoint, currPoint);
                    if (currDistance < minDistance)
                    {
                        minDistance = currDistance;
                        closestObject = currObject;
                    }
                }
            }

            this.sensorPacket.ClosestObject = closestObject;
        }

        protected void FilterRelevantObjects()
        {
            this.sensorPacket.RelevantObjects = this.sensorPacket.DetectedObjects.Where(wo => this.IsRelevant(wo)).ToList();
        }

        protected abstract bool IsRelevant(IWorldObject worldObject);

        private static List<Point> GetPoints(IWorldObject wo)
        {
            Point basePoint = new (wo.X, wo.Y);
            List<Point> points = new ();
            try
            {
                foreach (Geometry geometry in wo.RawGeometries)
                {
                    points.Add(basePoint + geometry.Bounds.Center);
                    points.Add(basePoint + geometry.Bounds.TopLeft - geometry.Bounds.Position);
                    points.Add(basePoint + geometry.Bounds.TopRight - geometry.Bounds.Position);
                    points.Add(basePoint + geometry.Bounds.BottomLeft - geometry.Bounds.Position);
                    points.Add(basePoint + geometry.Bounds.BottomRight - geometry.Bounds.Position);
                }
            }
            catch (Exception)
            {
            }

            return points;
        }

        private static double DistanceBetween(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        private static double ConvertToRadians(double angle)
        {
            return angle * (Math.PI / 180);
        }
    }
}