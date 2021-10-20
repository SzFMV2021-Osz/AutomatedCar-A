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

            foreach (var relevantObject in relevantObjects)
            {
                if (this.IsObjectInBrakeDistance(relevantObject, car))
                {
                    this.aebPacket.NeedEmergencyBrakeWarning = true;
                }
                else
                {
                    this.aebPacket.NeedEmergencyBrakeWarning = false;
                }
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
            double error = 1;

            if (this.ObjectDistanceFromCarInTime(worldObject, car) + error <= this.BrakeDistanceInTime(car))
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
            return Math.Pow(car.Speed, 2) / (2 * this.NormalizeDeceleration(car.Speed));
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
        /// Reaches max deceleration at 100 km/h. Normalizes the deceleration between 0 and 9.
        /// </summary>
        /// <param name="speed">Velocity of the car.</param>
        /// <returns>A real number between 0 and 9.</returns>
        private double NormalizeDeceleration(int speed)
        {
            if (speed < 100)
            {
                return MAX_DECELERATION * (speed / 100);
            }
            else
            {
                return MAX_DECELERATION;
            }
        }
    }
}
