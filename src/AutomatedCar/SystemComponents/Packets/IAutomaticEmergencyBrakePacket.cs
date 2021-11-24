namespace AutomatedCar.SystemComponents.Packets
{
    using System;
    using Avalonia;

    public interface IAutomaticEmergencyBrakePacket
    {
        public bool NeedEmergencyBrakeWarning { get; set; }

        public bool MightNotWorkProperlyWarning { get; set; }

        public double DecelerationRate { get; set; }
    }
}
