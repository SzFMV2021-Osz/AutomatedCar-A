namespace AutomatedCar.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class EnvironmentalForces
    {
        public static double FRICTION_COEFFICIENT = 0.3;

        public static double CAR_FRONTAL_AREA = 2.2;

        public static double AIR_DENSITIY = 1.29;

        public static double GenerateEngineForce(int revs, int pedalPosition, int innerGear)
        {
            double maxTorqueAtRPM = LookupTorqueCurve(revs);
            double currentTorque = (pedalPosition * maxTorqueAtRPM) / 100;

            double driveForce = (CarConfig.DIFFERENTIAL_RATIO * CarConfig.TRANSMISSION_EFFICIENCY *
                CarConfig.GearRatioLookupTable[innerGear] * currentTorque) / CarConfig.WHEEL_RADIUS;
            driveForce /= CarConfig.CAR_MASS;

            return driveForce;
        }

        public static double CalculateResistance(double speed)
        {
            double airResistance = (FRICTION_COEFFICIENT * CAR_FRONTAL_AREA * AIR_DENSITIY * Math.Pow(speed, 2)) / 2;
            double friction = 30 * airResistance;

            return airResistance + friction;
        }

        public static double LookupTorqueCurve(double rpm)
        {
            int rounded_rpm = CarConfig.TorqueLookupTable.Keys.ToList().OrderBy(x => Math.Abs(rpm - x)).First();
            return CarConfig.TorqueLookupTable[rounded_rpm];
        }
    }
}
