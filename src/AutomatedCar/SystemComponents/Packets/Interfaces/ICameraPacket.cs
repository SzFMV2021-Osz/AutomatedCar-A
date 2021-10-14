namespace AutomatedCar.SystemComponents.Packets
{
    using System.Collections.Generic;
    using AutomatedCar.Models;

    public interface ICameraPacket : ISensorPacket
    {
        public IEnumerable<IWorldObject> Roads { get; set; }
    }
}