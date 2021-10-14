namespace AutomatedCar.SystemComponents.Packets
{
    using System.Collections.Generic;
    using AutomatedCar.Models;
    using ReactiveUI;

    public sealed class CameraPacket : SensorPacket, ICameraPacket
    {
        private IEnumerable<IWorldObject> roads;

        public CameraPacket()
        {
            this.Roads = new List<IWorldObject>();
        }

        public IEnumerable<IWorldObject> Roads
        {
            get
            {
                return this.roads;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.roads, value);
            }
        }
    }
}