namespace AutomatedCar.SystemComponents.Packets
{
    using System.Collections.Generic;
    using AutomatedCar.Models;
    using ReactiveUI;

    public abstract class SensorPacket : ReactiveObject, ISensorPacket
    {
        private ICollection<IWorldObject> detectedObjects;
        private ICollection<IWorldObject> relevantObjects;
        private IWorldObject closestObject;

        public SensorPacket()
        {
            this.detectedObjects = new List<IWorldObject>();
            this.relevantObjects = new List<IWorldObject>();
        }

        public ICollection<IWorldObject> DetectedObjects
        {
            get
            {
                return this.detectedObjects;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.detectedObjects, value);
            }
        }

        public ICollection<IWorldObject> RelevantObjects
        {
            get
            {
                return this.relevantObjects;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.relevantObjects, value);
            }
        }

        public IWorldObject ClosestObject
        {
            get
            {
                return this.closestObject;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.closestObject, value);
            }
        }
    }
}