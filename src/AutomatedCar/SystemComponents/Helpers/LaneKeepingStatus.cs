namespace AutomatedCar.SystemComponents.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This type stores the lane keeping statuses.
    /// </summary>
    public enum LaneKeepingStatus
    {
        /// <summary>
        /// The lane keeping is active.
        /// </summary>
        Active,

        /// <summary>
        /// Currently disengaging the lane keeping assistant.
        /// </summary>
        Disengaging,

        /// <summary>
        /// The lane keeping is currently inactive.
        /// </summary>
        Inactive,
    }
}
