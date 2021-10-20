namespace AutomatedCar.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RevolutionsHelper
    {
        // Stores a value pair for each inner gear, with the second value representing the rate at which the RPM should increase in the respective gear
        public static List<Tuple<int, double>> GearCoefficients { get; set; } = new List<Tuple<int, double>>()
        {
            new Tuple<int, double>(0, 1), // this is for N
            new Tuple<int, double>(1, 2.5),
            new Tuple<int, double>(2, 0.7),
            new Tuple<int, double>(3, 0.55),
            new Tuple<int, double>(4, 0.45),
        };
    }
}
