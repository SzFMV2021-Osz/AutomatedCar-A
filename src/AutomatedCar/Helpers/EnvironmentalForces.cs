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

        public static double CalculateAirResistance(double speed)
        {
            return (FRICTION_COEFFICIENT * CAR_FRONTAL_AREA * AIR_DENSITIY * Math.Pow(speed, 2)) / 2;
        }

        public static double CalculateFriction(double speed)
        {
            return 30 * CalculateAirResistance(speed);
        }
    }
}
