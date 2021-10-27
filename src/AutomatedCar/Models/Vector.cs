using System;

namespace AutomatedCar.Models
{
    public class Vector : IEquatable<Vector>
    {
        public double X { get; set; }
        public double Y { get; set; }

        public bool Equals(Vector other)
        {
            if (other.X == this.X && other.Y == this.Y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}