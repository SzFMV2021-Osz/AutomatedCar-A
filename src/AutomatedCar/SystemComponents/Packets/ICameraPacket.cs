namespace AutomatedCar.SystemComponents.Packets
{
    using System.Collections.Generic;
    using AutomatedCar.Models;

    public interface ICameraPacket : ISensorPacket
    {
        public IList<WorldObject> Roads { get; set; }
    }
}