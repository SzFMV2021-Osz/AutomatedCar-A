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
    }
}
