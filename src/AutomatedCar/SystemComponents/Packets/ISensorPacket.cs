namespace AutomatedCar.SystemComponents.Packets
{
    using System.Collections.Generic;
    using AutomatedCar.Models;

    public interface ISensorPacket
    {
        IWorldObject ClosestObject { get; set; }

        ICollection<IWorldObject> DetectedObjects { get; set; }

        ICollection<IWorldObject> RelevantObjects { get; set; }
    }
}