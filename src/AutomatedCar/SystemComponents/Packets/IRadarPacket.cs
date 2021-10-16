namespace AutomatedCar.SystemComponents.Packets
{
    using System.Collections.Generic;
    using AutomatedCar.Models;

    public interface IRadarPacket : ISensorPacket
    {
        public IList<WorldObject> ClosingObjects { get; set; }

        public WorldObject ClosestObjectInLane { get; set; }
    }
}