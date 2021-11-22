namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.SystemComponents.Helpers;

    public interface ILKAPacket
    {
        /// <summary>
        /// Gets or sets wheter the lanekeeping is turned on or off.
        /// </summary>
        public LaneKeepingStatus LaneKeepingStatus { get; set; }

        /// <summary>
        /// Gets a value indicating whether the car is centered in the lane or not.
        /// </summary>
        public bool CarCentered { get; }
    }
}