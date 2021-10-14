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
        protected readonly int distance;
        private readonly int angleOfView;
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

        protected void CalculateBasicSensorData(IAutomatedCar car, IEnumerable<IWorldObject> worldObjects)
        {
            this.CalculateSensorArea(car);
            this.FindObjectsInSensorArea(worldObjects, car);
            this.FilterRelevantObjects();
            this.sensorPacket.ClosestObject = this.FindClosestObject(this.sensorPacket.RelevantObjects, car);
        }

        protected abstract bool IsRelevant(IWorldObject worldObject);

        protected IWorldObject FindClosestObject(IEnumerable<IWorldObject> worldObjects, IAutomatedCar car)
        {
            Point carPoint = new (car.X, car.Y);
            IWorldObject closestObject = null;

            foreach (IWorldObject currObject in worldObjects)
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

            return closestObject;
        }

        private void CalculateSensorArea(IAutomatedCar car)
        {
            double radius = this.distance * Math.Tan(ConvertToRadians(this.angleOfView / 2));
            double sin = Math.Sin(ConvertToRadians(car.Rotation));
            double cos = Cosine(ConvertToRadians(car.Rotation));

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

        private void FindObjectsInSensorArea(IEnumerable<IWorldObject> worldObjects, IAutomatedCar car)
        {
            ICollection<IWorldObject> detectedObjects = new List<IWorldObject>();

            foreach (IWorldObject currObject in worldObjects)
            {
                foreach (Point point in GetPoints(currObject))
                {
                    if (this.sensorArea.FillContains(point) && !detectedObjects.Contains(currObject))
                    {
                        detectedObjects.Add(currObject);
                    }
                }
            }

            detectedObjects.Remove(car);
            this.sensorPacket.DetectedObjects = detectedObjects;
        }

        private void FilterRelevantObjects()
        {
            this.sensorPacket.RelevantObjects = this.sensorPacket.DetectedObjects.Where(wo => this.IsRelevant(wo)).ToList();
        }

        private static List<Point> GetPoints(IWorldObject worldObject)
        {
            List<Point> points = new List<Point>();
            foreach (PolylineGeometry currGeometry in worldObject.Geometries)
            {
                foreach (Point currPoint in currGeometry.Points)
                {
                    points.Add(new Point(currPoint.X + worldObject.X, currPoint.Y + worldObject.Y));
                }
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

        // TODO: Code health: Investigate odd behaviour in Math.Cos
        protected static double Cosine(double rad)
        {
            double cos = 0;

            int i;
            for (i = 0; i < 7; i++)
            {
                cos += Math.Pow(-1, i) * Math.Pow(rad, 2 * i) / Fact(2 * i);
            }

            return cos;
        }

        private static int Fact(int n)
        {
            return n <= 0 ? 1 : n * Fact(n - 1);
        }
    }
}