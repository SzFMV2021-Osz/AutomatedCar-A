namespace AutomatedCar.SystemComponents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Helpers;
    using Avalonia;
    using Avalonia.Media;

    public class CollisionDetection : SystemComponent
    {
        private readonly CollisionDetectionPacket collisionPacket;

        public CollisionDetection(VirtualFunctionBus virtualFunctionBus)
            : base(virtualFunctionBus)
        {
            this.collisionPacket = new CollisionDetectionPacket();
            virtualFunctionBus.CollisionDetectionPacket = this.collisionPacket;
        }

        /// <summary>
        /// Event, which fires when the controlled car collides with an npc.
        /// </summary>
        public event EventHandler OnCollisionWithNpc;

        /// <summary>
        /// Event, which fires when the controlled car collides with a static object (fe.: tree).
        /// </summary>
        public event EventHandler OnCollisionWithStaticObject;

        /// <summary>
        /// Process method to calculate the distance x and y.
        /// </summary>
        public override void Process()
        {
            AutomatedCar car = World.Instance.ControlledCar;

            foreach (WorldObject item in CollideableObjects())
            {
                foreach (Point point in item.Geometries[0].Points)
                {
                    if (GetCarPoints(car).FillContains(new Point(point.X + item.X, point.Y + item.Y)))
                    {
                        this.collisionPacket.TypeOfCollision = this.DetermineCollisionType(item);
                    }
                }
            }
        }

        public static List<WorldObject> CollideableObjects()
        {
            return World.Instance.WorldObjects.Where(x => x.Collideable).ToList();
        }

        public static PolylineGeometry GetCarPoints(AutomatedCar car)
        {
            var carpoints = ((PolylineGeometry)car.Geometry).Points;

            Point[] points = new Point[((PolylineGeometry)car.Geometry).Points.Count];

            for (int i = 0; i < carpoints.Count; i++)
            {
                points[i] = new Point(Math.Abs(carpoints[i].X + car.X), Math.Abs(carpoints[i].Y + car.Y));
            }

            return new PolylineGeometry(points, true);
        }

        public CollisionType DetermineCollisionType(WorldObject collidingObject)
        {
            if (collidingObject is Car)
            {
                this.OnCollisionWithNpc?.Invoke(this, null);
                return CollisionType.NPC;
            }
            else
            {
                this.OnCollisionWithStaticObject?.Invoke(this, null);
                return CollisionType.StaticObject;
            }
        }
    }
}