namespace AutomatedCar.SystemComponents.Sensors
{
    using AutomatedCar.Models;
    using Avalonia;
    using Avalonia.Media;
    using System;
    using System.Collections.Generic;

    public class Sensor
    {
        private readonly int ANGLEOFVIEW;
        private readonly int DISTANCE;
        protected IWorldObject closestObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sensor"/> class.
        /// </summary>
        /// <param name="angleOfView">Angle of view in degree.</param>
        /// <param name="distance">Distance in meters.</param>
        public Sensor(int angleOfView, int distance)
        {
            if (angleOfView < 0 || angleOfView > 360 || distance < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                this.ANGLEOFVIEW = angleOfView;
                this.DISTANCE = distance * 50;  // 1 meter is 50 pixel
            }
        }

        public List<IWorldObject> RelevantObjects(IEnumerable<IWorldObject> worldObjects, IAutomatedCar car)
        {
            double radius = this.DISTANCE * Math.Tan(this.ConvertToRadians(this.ANGLEOFVIEW / 2));
            double sin = Math.Sin(this.ConvertToRadians(car.Rotation));
            double cos = Math.Cos(this.ConvertToRadians(car.Rotation));

            Point pointToConvert1 = new (-radius, -this.DISTANCE);
            Point pointToConvert2 = new (+radius, -this.DISTANCE);

            Point convertedPoint1 = new ((pointToConvert1.X * cos) - (pointToConvert1.Y * sin), (pointToConvert1.X * sin) + (pointToConvert1.Y * cos));
            Point convertedPoint2 = new ((pointToConvert2.X * cos) - (pointToConvert2.Y * sin), (pointToConvert2.X * sin) + (pointToConvert2.Y * cos));

            List<Point> points = new ()
            {
                new Point(car.X, car.Y),
                new Point(car.X + convertedPoint1.X, car.Y + convertedPoint1.Y),
                new Point(car.X + convertedPoint2.X, car.Y + convertedPoint2.Y),
            };

            PolylineGeometry triangle = new (points, true);

            List<IWorldObject> relevantObjects = new ();
            foreach (IWorldObject currentObj in worldObjects)
            {
                foreach (Point point in GetPoints(currentObj))
                {
                    if (triangle.FillContains(point) && !relevantObjects.Contains(currentObj))
                    {
                        relevantObjects.Add(currentObj);
                    }
                }
            }

            relevantObjects.Remove(car);
            return relevantObjects;
        }

        protected virtual void FindClosestObject(IEnumerable<IWorldObject> relevantObjects, IAutomatedCar car)
        {
        }

        protected double DistanceBetween(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        protected static List<Point> GetPoints(IWorldObject wo)
        {
            Point basePoint = new(wo.X, wo.Y);
            List<Point> points = new();
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

        private double ConvertToRadians(double angle)
        {
            return Math.PI / 180 * angle;
        }
    }
}
