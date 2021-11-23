using System;

namespace AutomatedCar.Models
{
    public class Vector : IEquatable<Vector>
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

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        public static Vector operator -(Vector a, WorldObject b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        public double GetLength()
        {
            return Math.Sqrt((X * X) + (Y * Y));
        }
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