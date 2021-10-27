namespace AutomatedCar.SystemComponents.Packets
{
    using System.Collections.Generic;
    using AutomatedCar.Models;
    using ReactiveUI;

    public sealed class CameraPacket : SensorPacket, ICameraPacket
    {
        private IList<WorldObject> roads;

        public CameraPacket()
        {
            this.Roads = new List<WorldObject>();
        }

        public IList<WorldObject> Roads
        {
            get => this.roads;
            set => this.RaiseAndSetIfChanged(ref this.roads, value);
        }
    }
}