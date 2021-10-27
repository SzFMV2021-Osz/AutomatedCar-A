namespace AutomatedCar.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class Conversions
    {
        private const int MAX_SW_POS = 100;
        private const int MAX_WHEEL_ANGLE = 45;

        public static double MapSWRotationToAngle(int rotation)
        {
            return (rotation * MAX_WHEEL_ANGLE) / MAX_SW_POS;
        }

        public static double MapOrientationToRotationDegree(float carHeading)
        {
            double PI_radians = carHeading / Math.PI;
            double asd = 180 * PI_radians;
            return asd + 90;
        }

        public static float MapRotationDegreeToOrientationValue(double degree)
        {
            return (float)(((degree - 90) / 180) * Math.PI);
        }
    }
}
