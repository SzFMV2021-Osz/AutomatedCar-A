namespace AutomatedCar.SystemComponents.Packets
{
    using System.Collections.Generic;
    using AutomatedCar.Models;
    using ReactiveUI;

    public class SensorPacket : ReactiveObject
    {
        private IList<WorldObject> detectedObjects;
        private IList<WorldObject> relevantObjects;
        private WorldObject closestObject;

        public SensorPacket()
        {
            this.detectedObjects = new List<WorldObject>();
            this.relevantObjects = new List<WorldObject>();
        }

        public IList<WorldObject> DetectedObjects
        {
            get => this.detectedObjects;
            set => this.RaiseAndSetIfChanged(ref this.detectedObjects, value);
        }

        public IList<WorldObject> RelevantObjects
        {
            get => this.relevantObjects;
            set => this.RaiseAndSetIfChanged(ref this.relevantObjects, value);
        }

        public WorldObject ClosestObject
        {
            get => this.closestObject;
            set => this.RaiseAndSetIfChanged(ref this.closestObject, value);
        }
    }
}