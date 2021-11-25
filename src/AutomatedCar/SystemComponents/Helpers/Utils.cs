namespace AutomatedCar.SystemComponents.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using AutomatedCar.Models;
    using Avalonia;
    using Avalonia.Media;
    using Newtonsoft.Json;

    public static class Utils
    {
        public static readonly IList<ReferencePoint> ReferencePoints = LoadReferencePoints();

        public static WorldObject FindClosestObject(IList<WorldObject> worldObjects, AutomatedCar car)
        {
            Point carPoint = new (car.X, car.Y);
            WorldObject closestObject = null;
            List<Point> temp = new ();

            double minDistance = double.MaxValue;
            foreach (WorldObject currObject in worldObjects)
            {
                foreach (Point currPoint in GetPoints(currObject))
                {
                    double currDistance = DistanceBetween(carPoint, currPoint);
                    if (currDistance < minDistance)
                    {
                        minDistance = currDistance;
                        closestObject = currObject;
                    }

                    temp.Add(new Point(currObject.X, currDistance));
                }
            }

            return closestObject;
        }

        public static double DistanceBetween(Point from, Point to)
        {
            return Math.Sqrt(Math.Pow(from.X - to.X, 2) + Math.Pow(from.Y - to.Y, 2));
        }

        public static List<Point> GetPoints(WorldObject worldObject)
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

        public static Point RotatePoint(Point point, System.Drawing.Point rotationPoint, double rotation)
        {
            Point transformedPoint = new (point.X - rotationPoint.X, point.Y - rotationPoint.Y);

            double sin = Math.Sin(Utils.ConvertToRadians(rotation));
            double cos = Math.Cos(Utils.ConvertToRadians(rotation));
            Point rotatedPoint = new ((transformedPoint.X * cos) - (transformedPoint.Y * sin), (transformedPoint.X * sin) + (transformedPoint.Y * cos));

            return rotatedPoint;
        }

        public static PolylineGeometry RotateRawGeometry(PolylineGeometry geometry, System.Drawing.Point rotationPoint, double rotation)
        {
            List<Point> rotatedPoints = new ();
            foreach (Point point in geometry.Points)
            {
                rotatedPoints.Add(Utils.RotatePoint(point, rotationPoint, rotation));
            }

            return new PolylineGeometry(rotatedPoints, true);
        }

        public static PolylineGeometry ShiftGeometryWithWorldCoordinates(PolylineGeometry geometry, int x, int y)
        {
            Points shiftedPoints = new ();

            foreach (Point point in geometry.Points)
            {
                shiftedPoints.Add(new Point((int)(point.X + x), (int)(point.Y + y)));
            }

            return new PolylineGeometry(shiftedPoints, true);
        }

        public static double ConvertToRadians(double angle)
        {
            return angle * (Math.PI / 180);
        }

        private static IList<ReferencePoint> LoadReferencePoints()
        {
            string jsonString = new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"AutomatedCar.Assets.reference_points.json")).ReadToEnd();
            return JsonConvert.DeserializeObject<List<ReferencePoint>>(jsonString);
        }
    }
}