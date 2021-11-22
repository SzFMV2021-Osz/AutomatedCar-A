namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.SystemComponents.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ILaneKeepingPacket
    {
        /// <summary>
        /// Gets or sets wheter the lanekeeping is turned on or off.
        /// </summary>
        public LaneKeepingStatus LaneKeepingStatus { get; set; }

        /// <summary>
        /// Gets wheter the car is centered in the lane or not.
        /// </summary>
        public bool CarCentered { get; }
    }
}
