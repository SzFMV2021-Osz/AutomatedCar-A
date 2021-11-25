namespace AutomatedCar.SystemComponents.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Avalonia;
    using Avalonia.Media;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets;

    public class AutomaticEmergencyBrake : SystemComponent
    {
        private const double MAX_DECELERATION = 9;
        private const int MIN_WARNING_SPEED = 70;

        private AutomaticEmergencyBrakePacket aebPacket;

        private WorldObject wo;

        public AutomaticEmergencyBrake(VirtualFunctionBus virtualFunction)
            : base(virtualFunction)
        {
            this.aebPacket = new AutomaticEmergencyBrakePacket();
            this.virtualFunctionBus.AutomaticEmergencyBrakePacket = this.aebPacket;
        }

        public override void Process()
        {
            AutomatedCar car = World.Instance.ControlledCar;
            IList<WorldObject> relevantObjects = this.virtualFunctionBus.RadarPacket.RelevantObjects;
            IList<WorldObject> closingObjects = this.virtualFunctionBus.RadarPacket.ClosingObjects;
            WorldObject closestObject = this.virtualFunctionBus.RadarPacket.ClosestObject;

            if (car.Speed >= MIN_WARNING_SPEED)
            {
                this.aebPacket.MightNotWorkProperlyWarning = true;
            }
            else
            {
                this.aebPacket.MightNotWorkProperlyWarning = false;
            }


            foreach (var closingObject in closingObjects)
            {
                if (relevantObjects.Contains(closingObject) && (closestObject.X == closingObject.X && closestObject.Y == closingObject.Y))
                {
                    IList<Point> points = new List<Point>()
                    {
                        new Point(0, car.RotationPoint.Y),
                        new Point(car.RotationPoint.X + 70, car.RotationPoint.Y),
                        new Point(car.RotationPoint.X + 70, car.RotationPoint.Y - 180),
                        new Point(0, car.RotationPoint.Y - 180),
                        new Point(0, car.RotationPoint.Y),
                    };

                    PolylineGeometry carGeometry = new PolylineGeometry(points, true);

                    PolylineGeometry geometry = Utils.RotateRawGeometry(carGeometry, car.RotationPoint, car.Rotation);
                    geometry = Utils.ShiftGeometryWithWorldCoordinates(geometry, car.X, car.Y);

                    //wo = new WorldObject(car.X, car.Y, "sensor.png");
                    //wo.RawGeometries.Add(carGeometry);
                    //wo.Geometries.Add(geometry);
                    //World.Instance.WorldObjects.Add(wo);

                    foreach (var point in Utils.GetPoints(closingObject))
                    {
                        if (geometry.FillContains(point))
                        {
                            this.aebPacket.NeedEmergencyBrakeWarning = true;
                            this.aebPacket.DecelerationRate = this.NormalizeDeceleration(car.Speed) * 80;
                        }
                    }
                }
            }

            if (car.Speed == 0 || this.virtualFunctionBus.RadarPacket.ClosestObject == null)
            {
                this.aebPacket.NeedEmergencyBrakeWarning = false;
            }
        }


        /// <summary>
        /// Decides that whether the object is in brake distance or not.
        /// Calculates the theoretical time distance between the object and the car
        /// then compares it with the time required for braking.
        /// I added an error constant to the theoretical time distance,
        /// which helps to avoid braking in the very last second.
        /// </summary>
        /// <param name="worldObject">Object on the map.</param>
        /// <param name="car">Controlled car.</param>
        /// <returns>True or False.</returns>
        private bool IsObjectInBrakeDistance(WorldObject worldObject, AutomatedCar car)
        {
            double error = 200;
            double d = this.DistanceFromCar(worldObject, car);
            double r = this.BrakeDistance(car);

            if (d <= r + error)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsDynamicObject(WorldObject worldObject)
        {
            if (worldObject is Pedestrian || worldObject is NonPlayerCar)
            {
                return true;
            }

            return false;
        }

        private bool IsDynamicObjectWillInBrakeDistance(WorldObject worldObject, AutomatedCar car)
        {
            int objectSpeed = (worldObject as AbstractNPC).Speed;
            double x1 = Math.Abs(car.X - worldObject.X) + worldObject.X;
            double y1 = worldObject.Y;

            double x2 = worldObject.X;
            double y2 = Math.Abs(car.Y - worldObject.Y) + worldObject.Y;

            double distanceFromIntersection1 = this.DistanceFromPoint(worldObject, x1, y1);
            double distanceFromIntersectionInTime1 = distanceFromIntersection1 / objectSpeed;

            double distanceFromIntersection2 = this.DistanceFromPoint(worldObject, x2, y2);
            double distanceFromIntersectionInTime2 = distanceFromIntersection2 / objectSpeed;

            if ((distanceFromIntersectionInTime1 <= this.BrakeDistanceInTime(car)) || (distanceFromIntersectionInTime2 <= this.BrakeDistanceInTime(car)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates the distance between an object and the car using Euclidean norm (2-norm).
        /// </summary>
        /// <param name="worldObject">Object on the map.</param>
        /// <param name="car">Controlled car.</param>
        /// <returns>Distance between the object and the car.</returns>
        private double DistanceFromCar(WorldObject worldObject, AutomatedCar car)
        {
            return Math.Sqrt(Math.Pow(worldObject.X - car.X, 2) + Math.Pow(worldObject.Y - car.Y, 2));
        }

        private double DistanceFromPoint(WorldObject worldObject, double x, double y)
        {
            return Math.Sqrt(Math.Pow(worldObject.X - x, 2) + Math.Pow(worldObject.Y - y, 2));
        }

        /// <summary>
        /// Calculates the time required to reach the object. t = s / v.
        /// </summary>
        /// <param name="worldObject">Object on the map.</param>
        /// <param name="car">Controlled car.</param>
        /// <returns>Time that required to reach the object.</returns>
        private double ObjectDistanceFromCarInTime(WorldObject worldObject, AutomatedCar car)
        {
            return this.DistanceFromCar(worldObject, car) / car.Speed;
        }

        /// <summary>
        /// Calculates the braking ditsance. s = v^2 / 2 * a.
        /// </summary>
        /// <param name="car">Controlled car.</param>
        /// <returns>Distance required for braking.</returns>
        private double BrakeDistance(AutomatedCar car)
        {
            return Math.Pow(car.Speed, 2) / (double)(2 * this.NormalizeDeceleration(car.Speed));
        }

        /// <summary>
        /// Calculates the time required for braking. t = s / v.
        /// </summary>
        /// <param name="car">Controlled car.</param>
        /// <returns>Time required for braking.</returns>
        private double BrakeDistanceInTime(AutomatedCar car)
        {
            return this.BrakeDistance(car) / car.Speed;
        }

        /// <summary>
        /// Reaches max deceleration at 70 km/h. Normalizes the deceleration between 0 and 9.
        /// </summary>
        /// <param name="speed">Velocity of the car.</param>
        /// <returns>A real number between 0 and 9.</returns>
        private double NormalizeDeceleration(int speed)
        {
            if (speed < 70)
            {
                return MAX_DECELERATION * ((double)speed / 70);
            }
            else
            {
                return MAX_DECELERATION;
            }
        }
    }
}
