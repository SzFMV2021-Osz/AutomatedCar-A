namespace AutomatedCar.SystemComponents.Sensors
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Helpers;
    using AutomatedCar.SystemComponents.Packets;
    using Avalonia;
    using Avalonia.Media;
    using Newtonsoft.Json;
    using System.Reflection;

    public abstract class Sensor : SystemComponent, ISensor
    {
        protected ISensorPacket sensorPacket;
        private static readonly IList<ReferencePoint> ReferencePoints = LoadReferencePoints();
        private readonly int distance;
        private readonly int angleOfView;
        private WorldObject sensorObject;

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

        protected static double DistanceBetween(Point from, Point to)
        {
            return Math.Sqrt(Math.Pow(from.X - to.X, 2) + Math.Pow(from.Y - to.Y, 2));
        }

        protected static WorldObject FindClosestObject(IList<WorldObject> worldObjects, AutomatedCar car)
        {
            Point carPoint = new (car.X, car.Y);
            WorldObject closestObject = null;

            foreach (WorldObject currObject in worldObjects)
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

        protected void CalculateSensorData(AutomatedCar car, ObservableCollection<WorldObject> worldObjects)
        {
            this.SetSensor(car);

            this.FindObjectsInSensorArea(worldObjects);
            this.FilterRelevantObjects();
            this.sensorPacket.ClosestObject = FindClosestObject(this.sensorPacket.RelevantObjects, car);
        }

        protected abstract bool IsRelevant(WorldObject worldObject);

        private static PolylineGeometry GetRawGeometry(int triangleBase, int distance)
        {
            List<Point> points = new ()
            {
                new Point(triangleBase, distance),
                new Point(0, 0),
                new Point(2 * triangleBase, 0),
                new Point(triangleBase, distance),
            };

            return new PolylineGeometry(points, true);
        }

        private static List<Point> GetPoints(WorldObject worldObject)
        {
            List<Point> points = new () { new Point(worldObject.X, worldObject.Y) };

            Point refPoint = new (0, 0);
            if (ReferencePoints.Any(r => r.Type + ".png" == worldObject.Filename))
            {
                ReferencePoint currRefPoint = ReferencePoints.Where(r => r.Type + ".png" == worldObject.Filename).FirstOrDefault();
                refPoint = new (currRefPoint.X, currRefPoint.Y);
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

        private static IList<ReferencePoint> LoadReferencePoints()
        {
            string jsonString = new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"AutomatedCar.Assets.reference_points.json")).ReadToEnd();
            return JsonConvert.DeserializeObject<List<ReferencePoint>>(jsonString);
        }

        private static double ConvertToRadians(double angle)
        {
            return angle * (Math.PI / 180);
        }

        // TODO: Code health: Investigate odd behaviour in Math.Cos
        private static double Cosine(double rad)
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

        private void SetSensor(AutomatedCar car)
        {
            int triangleBase = (int)(this.distance * Math.Tan(ConvertToRadians(this.angleOfView / 2)));

            if (this.sensorObject == null)
            {
                this.sensorObject = new WorldObject(car.X + car.RotationPoint.X, car.Y + car.RotationPoint.Y, "sensor.png");
                this.sensorObject.RawGeometries.Add(GetRawGeometry(triangleBase, this.distance));
                this.sensorObject.Collideable = false;
                this.sensorObject.Geometries.Add(this.GetGeometry(car));
                World.Instance.WorldObjects.Add(this.sensorObject);
            }
            else
            {
                this.sensorObject.X = car.X + car.RotationPoint.X;
                this.sensorObject.Y = car.Y + car.RotationPoint.Y;
                this.sensorObject.Geometries[0] = this.GetGeometry(car);
            }

            this.sensorObject.RotationPoint = new (triangleBase, this.distance + car.RotationPoint.Y - (int)this.RelativeLocation.Y);
            this.sensorObject.Rotation = car.Rotation;
        }

        private void FindObjectsInSensorArea(ObservableCollection<WorldObject> worldObjects)
        {
            List<WorldObject> detectedObjects = new ();

            foreach (WorldObject currObject in worldObjects)
            {
                foreach (Point point in GetPoints(currObject))
                {
                    if (this.sensorObject.Geometries[0].FillContains(point) && !detectedObjects.Contains(currObject) && !currObject.Filename.Contains("sensor"))
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

        private PolylineGeometry GetGeometry(AutomatedCar car)
        {
            double sin = Math.Sin(ConvertToRadians(car.Rotation));
            double cos = Cosine(ConvertToRadians(car.Rotation));

            Point pointToConvert0 = new (this.sensorObject.RawGeometries[0].Points[0].X - this.sensorObject.RotationPoint.X, this.sensorObject.RawGeometries[0].Points[0].Y - this.sensorObject.RotationPoint.Y);
            Point pointToConvert1 = new (this.sensorObject.RawGeometries[0].Points[1].X - this.sensorObject.RotationPoint.X, this.sensorObject.RawGeometries[0].Points[1].Y - this.sensorObject.RotationPoint.Y);
            Point pointToConvert2 = new (this.sensorObject.RawGeometries[0].Points[2].X - this.sensorObject.RotationPoint.X, this.sensorObject.RawGeometries[0].Points[2].Y - this.sensorObject.RotationPoint.Y);

            Point convertedPoint0 = new ((pointToConvert0.X * cos) - (pointToConvert0.Y * sin), (pointToConvert0.X * sin) + (pointToConvert0.Y * cos));
            Point convertedPoint1 = new ((pointToConvert1.X * cos) - (pointToConvert1.Y * sin), (pointToConvert1.X * sin) + (pointToConvert1.Y * cos));
            Point convertedPoint2 = new ((pointToConvert2.X * cos) - (pointToConvert2.Y * sin), (pointToConvert2.X * sin) + (pointToConvert2.Y * cos));

            Point finalPoint = new (this.sensorObject.RotationPoint.X + this.sensorObject.X - this.sensorObject.RotationPoint.X, this.sensorObject.RotationPoint.Y + this.sensorObject.Y - this.sensorObject.RotationPoint.Y);
            List<Point> points = new ()
            {
                new Point((int)(finalPoint.X + convertedPoint0.X), (int)(finalPoint.Y + convertedPoint0.Y)),
                new Point((int)(finalPoint.X + convertedPoint1.X), (int)(finalPoint.Y + convertedPoint1.Y)),
                new Point((int)(finalPoint.X + convertedPoint2.X), (int)(finalPoint.Y + convertedPoint2.Y)),
                new Point((int)(finalPoint.X + convertedPoint0.X), (int)(finalPoint.Y + convertedPoint0.Y)),
            };

            return new PolylineGeometry(points, true);
        }
    }
}