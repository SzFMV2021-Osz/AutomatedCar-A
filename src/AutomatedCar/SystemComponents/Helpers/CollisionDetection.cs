namespace AutomatedCar.SystemComponents
{
    using Avalonia;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Avalonia.Media;
    using AutomatedCar.SystemComponents.Helpers;

    public class CollisionDetection : SystemComponent
    {
        private CollisionDetectionPacket collisionPacket;

        public CollisionDetection(VirtualFunctionBus virtualFunctionBus) : base(virtualFunctionBus)
        {
            this.collisionPacket = new CollisionDetectionPacket();
            virtualFunctionBus.CollisionDetectionPacket = collisionPacket;
        }

        /// <summary>
        /// Process method to calculate the distance x and y.
        /// </summary>
        public override void Process()
        {
            AutomatedCar car = World.Instance.ControlledCar;

            foreach (var item in CollideableObjects())
            {
                foreach (var point in item.Geometries[0].Points)
                {
                    if (GetCarPoints(car).FillContains(new Point(point.X + item.X, point.Y + item.Y)))
                    {
                        //TODO: Alert 
                        bool collision = true;

                    }
                }
            }
        }

        public List<WorldObject> CollideableObjects()
        {
            return World.Instance.WorldObjects.Where(x => x.Collideable).ToList();
        }

        public PolylineGeometry GetCarPoints(AutomatedCar car)
        {
            var carpoints = ((PolylineGeometry)car.Geometry).Points;

            Point[] points = new Point[((PolylineGeometry)car.Geometry).Points.Count()];

            for (int i = 0; i < carpoints.Count(); i++)
            {
                points[i] = new Point(Math.Abs(carpoints[i].X + car.X), Math.Abs(carpoints[i].Y + car.Y));
            }

            return new PolylineGeometry(points, true);
        }
    }
}
