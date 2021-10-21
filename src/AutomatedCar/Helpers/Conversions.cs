namespace AutomatedCar.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Conversions
    {
        private const int PIXEL_METER_RATIO = 50;

        public static int ConvertMetresToPixels(double metres)
        {
            return (int)(metres * PIXEL_METER_RATIO);
        }

        public static int ConvertPixelsToMeter(double pixels)
        {
            return (int)(pixels / PIXEL_METER_RATIO);
        }
    }
}
