namespace AutomatedCar.SystemComponents.Sensors
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Helpers;
    using AutomatedCar.SystemComponents.Packets;
    using Avalonia;
    using Avalonia.Media;
    using Newtonsoft.Json;

    public abstract class Sensor : SystemComponent, ISensor
    {
        protected ISensorPacket sensorPacket;
        protected readonly int distance;
        private readonly int angleOfView;
        private static IEnumerable<ReferencePoint> referencePoints = LoadRefPoints();

        public WorldObject SensorObject { get; set; }

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
            World.Instance.WorldObjects.Remove(this.SensorObject);
            this.SetSensor(car);
            World.Instance.WorldObjects.Add(this.SensorObject);

            this.FindObjectsInSensorArea(worldObjects, car);
            this.FilterRelevantObjects();
            this.sensorPacket.ClosestObject = this.FindClosestObject(this.sensorPacket.RelevantObjects, car);
        }

        protected abstract bool IsRelevant(IWorldObject worldObject);

        protected IWorldObject FindClosestObject(IEnumerable<IWorldObject> worldObjects, IAutomatedCar car)
        {
            Point carPoint = new(car.X, car.Y);
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

        public void SetSensor(IAutomatedCar car)
        {
            int triangleBase = (int)(this.distance * Math.Tan(ConvertToRadians(this.angleOfView / 2)));

            if (this.SensorObject == null)
            {
                this.SensorObject = new WorldObject(car.X + car.RotationPoint.X, car.Y + car.RotationPoint.Y, "sensor.png");
                this.SensorObject.RotationPoint = new(triangleBase, this.distance + car.RotationPoint.Y - (int)this.RelativeLocation.Y);
                this.SensorObject.Rotation = car.Rotation;
                this.SensorObject.Collideable = false;
                this.SensorObject.RawGeometries.Add(GetRawGeometry(triangleBase, this.distance));
                this.SensorObject.Geometries.Add(this.GetGeometry(car));
            }
            else
            {
                this.SensorObject.X = car.X + car.RotationPoint.X;
                this.SensorObject.Y = car.Y + car.RotationPoint.Y;
                this.SensorObject.RotationPoint = new(triangleBase, this.distance + car.RotationPoint.Y - (int)this.RelativeLocation.Y);
                this.SensorObject.Rotation = car.Rotation;
                this.SensorObject.Geometries[0] = this.GetGeometry(car);
            }
        }

        private void FindObjectsInSensorArea(IEnumerable<IWorldObject> worldObjects, IAutomatedCar car)
        {
            ICollection<IWorldObject> detectedObjects = new List<IWorldObject>();

            foreach (IWorldObject currObject in worldObjects)
            {
                foreach (Point point in GetPoints(currObject))
                {
                    if (this.SensorObject.Geometries[0].FillContains(point) && !detectedObjects.Contains(currObject) && !currObject.Filename.Contains("sensor"))
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

        private PolylineGeometry GetGeometry(IAutomatedCar car)
        {
            double sin = Math.Sin(ConvertToRadians(car.Rotation));
            double cos = Cosine(ConvertToRadians(car.Rotation));

            Point pointToConvert0 = new(this.SensorObject.RawGeometries[0].Points[0].X - this.SensorObject.RotationPoint.X, this.SensorObject.RawGeometries[0].Points[0].Y - this.SensorObject.RotationPoint.Y);
            Point pointToConvert1 = new(this.SensorObject.RawGeometries[0].Points[1].X - this.SensorObject.RotationPoint.X, this.SensorObject.RawGeometries[0].Points[1].Y - this.SensorObject.RotationPoint.Y);
            Point pointToConvert2 = new(this.SensorObject.RawGeometries[0].Points[2].X - this.SensorObject.RotationPoint.X, this.SensorObject.RawGeometries[0].Points[2].Y - this.SensorObject.RotationPoint.Y);

            Point convertedPoint0 = new((pointToConvert0.X * cos) - (pointToConvert0.Y * sin), (pointToConvert0.X * sin) + (pointToConvert0.Y * cos));
            Point convertedPoint1 = new((pointToConvert1.X * cos) - (pointToConvert1.Y * sin), (pointToConvert1.X * sin) + (pointToConvert1.Y * cos));
            Point convertedPoint2 = new((pointToConvert2.X * cos) - (pointToConvert2.Y * sin), (pointToConvert2.X * sin) + (pointToConvert2.Y * cos));

            Point finalPoint = new(this.SensorObject.RotationPoint.X + this.SensorObject.X - this.SensorObject.RotationPoint.X, this.SensorObject.RotationPoint.Y + this.SensorObject.Y - this.SensorObject.RotationPoint.Y);
            List<Point> points = new()
            {
                new Point((int)(finalPoint.X + convertedPoint0.X), (int)(finalPoint.Y + convertedPoint0.Y)),
                new Point((int)(finalPoint.X + convertedPoint1.X), (int)(finalPoint.Y + convertedPoint1.Y)),
                new Point((int)(finalPoint.X + convertedPoint2.X), (int)(finalPoint.Y + convertedPoint2.Y)),
                new Point((int)(finalPoint.X + convertedPoint0.X), (int)(finalPoint.Y + convertedPoint0.Y)),
            };

            return new PolylineGeometry(points, true);
        }

        private static PolylineGeometry GetRawGeometry(int triangleBase, int distance)
        {
            List<Point> points = new()
            {
                new Point(triangleBase, distance),
                new Point(0, 0),
                new Point(2 * triangleBase, 0),
                new Point(triangleBase, distance),
            };

            return new PolylineGeometry(points, true);
        }

        private static List<Point> GetPoints(IWorldObject worldObject)
        {
            List<Point> points = new List<Point>();
            points.Add(new(worldObject.X, worldObject.Y));

            Point refPoint = new(0, 0);
            if (referencePoints.Any(r => r.Type + ".png" == worldObject.Filename))
            {
                ReferencePoint currRefPoint = referencePoints.Where(r => r.Type + ".png" == worldObject.Filename).FirstOrDefault();
                refPoint = new(currRefPoint.X, currRefPoint.Y);
            }

            foreach (PolylineGeometry currGeometry in worldObject.Geometries)
            {
                foreach (Point currPoint in currGeometry.Points)
                {
                    points.Add(new Point(currPoint.X + worldObject.X - refPoint.X, currPoint.Y + worldObject.Y - refPoint.Y));
                }
            }

            return points;
        }

        private static IEnumerable<ReferencePoint> LoadRefPoints()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\")) + @"Assets\reference_points.json";
            string jsonString = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<ReferencePoint>>(jsonString);
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