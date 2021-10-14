namespace AutomatedCar.SystemComponents.Packets
{
    using System.Collections.Generic;
    using AutomatedCar.Models;

    public interface IRadarPacket : ISensorPacket
    {
        public IEnumerable<IWorldObject> IncomingObjects { get; set; }

        public IWorldObject ClosestObjectInLane { get; set; }
    }
}