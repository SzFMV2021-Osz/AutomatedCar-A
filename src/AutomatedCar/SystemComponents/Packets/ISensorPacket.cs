namespace AutomatedCar.SystemComponents.Packets
{
    using System.Collections.Generic;
    using AutomatedCar.Models;

    public interface ISensorPacket
    {
        IList<WorldObject> DetectedObjects { get; set; }

        IList<WorldObject> RelevantObjects { get; set; }

        WorldObject ClosestObject { get; set; }
    }
}