namespace AutomatedCar.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Vector
    {
        public double X { get; set; }

        public double Y { get; set; }

        public Vector()
        {

        }

        public Vector(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
