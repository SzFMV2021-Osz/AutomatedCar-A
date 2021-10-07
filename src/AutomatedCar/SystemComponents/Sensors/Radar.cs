namespace AutomatedCar.SystemComponents.Sensors
{
    using System.Collections.Generic;
    using AutomatedCar.Models;
    using Avalonia;

    public sealed class Radar : Sensor
    {
        public Radar()
            : base(60, 200)
        {
        }

        protected override void FindClosestObject(IEnumerable<IWorldObject> relevantObjects, IAutomatedCar car)
        {
            Point carPoint = new (car.X, car.Y);
            IWorldObject closestObject = null;

            foreach (IWorldObject currObject in relevantObjects)
            {
                if (currObject.Collideable)
                {
                    double minDistance = double.MaxValue;
                    foreach (Point currPoint in GetPoints(currObject))
                    {
                        double currDistance = this.DistanceBetween(carPoint, currPoint);
                        if (currDistance < minDistance)
                        {
                            minDistance = currDistance;
                            closestObject = currObject;
                        }
                    }
                }
            }

            this.closestObject = closestObject;
        }
    }
}