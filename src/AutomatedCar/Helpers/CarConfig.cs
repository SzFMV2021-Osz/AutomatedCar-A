namespace AutomatedCar.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class CarConfig
    {
        public static int CAR_MASS = 1500;
        public static double TRANSMISSION_EFFICIENCY = 0.7;
        public static double WHEEL_RADIUS = 0.34;
        public static double DIFFERENTIAL_RATIO = 3.42;

        public static Dictionary<int, int> TorqueLookupTable = new Dictionary<int, int>()
        {
            { 1000, 50 },
            { 2000, 100 },
            { 3000, 150 },
            { 4000, 200 },
            { 5000, 250},
        };

        public static Dictionary<int, double> GearRatioLookupTable = new Dictionary<int, double>()
        {
            { 1, 2.66 },
            { 2, 1.78 },
            { 3, 1.30 },
            { 4, 1.00 },
        };
    }
}
