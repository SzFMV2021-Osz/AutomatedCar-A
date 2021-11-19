namespace AutomatedCar.SystemComponents.Sensors
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Helpers;
    using AutomatedCar.SystemComponents.Packets;
    using Avalonia;
    using Avalonia.Media;
    using Newtonsoft.Json;

    public abstract class Sensor : SystemComponent
    {
        protected readonly int distance;
        protected ISensorPacket sensorPacket;
        protected WorldObject sensorObject;
        private static readonly IList<ReferencePoint> ReferencePoints = LoadReferencePoints();
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

        public IList<Point> Points
        {
            get => this.DrawTriangle();
        }

        /// <summary>Gets or sets the sensor's location on car.</summary>
        public Point RelativeLocation { get; set; }

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

        protected static PolylineGeometry RotateRawGeometry(PolylineGeometry geometry, System.Drawing.Point rotationPoint, double rotation)
        {
            List<Point> rotatedPoints = new ();
            foreach (Point point in geometry.Points)
            {
                rotatedPoints.Add(RotatePoint(point, rotationPoint, rotation));
            }

            return new PolylineGeometry(rotatedPoints, true);
        }

        protected static PolylineGeometry ShiftGeometryWithWorldCoordinates(PolylineGeometry geometry, int x, int y)
        {
            Points shiftedPoints = new ();

            foreach (Point point in geometry.Points)
            {
                shiftedPoints.Add(new Point((int)(point.X + x), (int)(point.Y + y)));
            }

            return new PolylineGeometry(shiftedPoints, true);
        }

        protected static List<Point> GetPoints(WorldObject worldObject)
        {
            List<Point> points = new() { new Point(worldObject.X, worldObject.Y) };

            Point refPoint = new(0, 0);
            if (ReferencePoints.Any(r => r.Type + ".png" == worldObject.Filename))
            {
                ReferencePoint currRefPoint = ReferencePoints.Where(r => r.Type + ".png" == worldObject.Filename).FirstOrDefault();
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

        protected static double DistanceBetween(Point from, Point to)
        {
            return Math.Sqrt(Math.Pow(from.X - to.X, 2) + Math.Pow(from.Y - to.Y, 2));
        }

        private static Point RotatePoint(Point point, System.Drawing.Point rotationPoint, double rotation)
        {
            Point transformedPoint = new (point.X - rotationPoint.X, point.Y - rotationPoint.Y);

            double sin = Math.Sin(ConvertToRadians(rotation));
            double cos = Math.Cos(ConvertToRadians(rotation));
            Point rotatedPoint = new ((transformedPoint.X * cos) - (transformedPoint.Y * sin), (transformedPoint.X * sin) + (transformedPoint.Y * cos));

            return rotatedPoint;
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

        protected void CalculateSensorData(AutomatedCar car, ObservableCollection<WorldObject> worldObjects)
        {
            this.SetSensor(car);
            this.FindObjectsInSensorArea(worldObjects);
            this.FilterRelevantObjects();
            this.sensorPacket.ClosestObject = FindClosestObject(this.sensorPacket.RelevantObjects, car);
        }

        protected abstract bool IsRelevant(WorldObject worldObject);

        private static double ConvertToRadians(double angle)
        {
            return angle * (Math.PI / 180);
        }

        private static IList<ReferencePoint> LoadReferencePoints()
        {
            string jsonString = new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"AutomatedCar.Assets.reference_points.json")).ReadToEnd();
            return JsonConvert.DeserializeObject<List<ReferencePoint>>(jsonString);
        }

        private PolylineGeometry GetGeometry()
        {
            PolylineGeometry geometry = RotateRawGeometry(this.sensorObject.RawGeometries[0], this.sensorObject.RotationPoint, this.sensorObject.Rotation);

            return ShiftGeometryWithWorldCoordinates(geometry, this.sensorObject.X, this.sensorObject.Y);
        }

        private IList<Avalonia.Point> DrawTriangle()
        {
            int triangleBase = (int)(this.distance * Math.Tan(ConvertToRadians(this.angleOfView / 2)));
            List<Point> points = new ()
            {
                new Point(0 + this.RelativeLocation.X, 0 + this.RelativeLocation.Y),
                new Point(-triangleBase + this.RelativeLocation.X, -this.distance + this.RelativeLocation.Y),
                new Point(triangleBase + this.RelativeLocation.X, -this.distance + this.RelativeLocation.Y),
                new Point(0 + this.RelativeLocation.X, 0 + this.RelativeLocation.Y),
            };

            return points;
        }

        private void SetSensor(AutomatedCar car)
        {
            int triangleBase = (int)(this.distance * Math.Tan(ConvertToRadians(this.angleOfView / 2)));

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
                foreach (Point point in GetPoints(currObject))
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