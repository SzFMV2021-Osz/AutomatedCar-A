namespace AutomatedCar.SystemComponents.Packets
{
    using System.Collections.Generic;
    using AutomatedCar.Models;
    using ReactiveUI;

    public sealed class RadarPacket : SensorPacket, IRadarPacket
    {
        private IList<WorldObject> incomingObjects;
        private WorldObject closestObjectInLane;

        public RadarPacket()
        {
            this.incomingObjects = new List<WorldObject>();
        }

        public IList<WorldObject> ClosingObjects
        {
            get => this.incomingObjects;
            set => this.RaiseAndSetIfChanged(ref this.incomingObjects, value);
        }

        public WorldObject ClosestObjectInLane
        {
            get => this.closestObjectInLane;
            set => this.RaiseAndSetIfChanged(ref this.closestObjectInLane, value);
        }
    }
}