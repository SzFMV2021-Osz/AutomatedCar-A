namespace AutomatedCar.SystemComponents.Packets
{
    using System.Collections.Generic;
    using AutomatedCar.Models;
    using ReactiveUI;

    public sealed class RadarPacket : SensorPacket, IRadarPacket
    {
        private IEnumerable<IWorldObject> incomingObjects;
        private IWorldObject closestObjectInLane;

        public RadarPacket()
        {
            this.incomingObjects = new List<IWorldObject>();
        }

        public IEnumerable<IWorldObject> IncomingObjects
        {
            get
            {
                return this.incomingObjects;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.incomingObjects, value);
            }
        }

        public IWorldObject ClosestObjectInLane
        {
            get
            {
                return this.closestObjectInLane;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.closestObjectInLane, value);
            }
        }
    }
}