namespace AutomatedCar.SystemComponents.Sensors
{
    using System.Collections.Generic;
    using AutomatedCar.Models;
    using Avalonia;

    public sealed class Camera : Sensor
    {
        public Camera()
            : base(60, 80)
        {
        }

        protected override void FindClosestObject(IEnumerable<IWorldObject> relevantObjects, IAutomatedCar car)
        {
            Point carPoint = new (car.X, car.Y);
            IWorldObject closestObject = null;

            foreach (IWorldObject currObject in relevantObjects)
            {
                if (this.IsRelevant(currObject))
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

        private bool IsRelevant(IWorldObject worldObject)
        {
            switch (worldObject.WorldObjectType)
            {
                case WorldObjectType.RoadSign: return true;
                case WorldObjectType.Crosswalk: return true;
                case WorldObjectType.Road: return true;
                default: return false;
            }
        }
    }
}