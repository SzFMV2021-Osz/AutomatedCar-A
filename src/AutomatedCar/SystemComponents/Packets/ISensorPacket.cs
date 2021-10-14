namespace AutomatedCar.SystemComponents.Packets
{
    using System.Collections.Generic;
    using AutomatedCar.Models;

    public interface ISensorPacket
    {
        ICollection<IWorldObject> DetectedObjects { get; set; }

        ICollection<IWorldObject> RelevantObjects { get; set; }

        IWorldObject ClosestObject { get; set; }
    }
}