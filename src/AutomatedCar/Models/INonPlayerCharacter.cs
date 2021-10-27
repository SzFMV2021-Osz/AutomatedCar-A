namespace AutomatedCar.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// interface for all non players in the simulation.
    /// </summary>
    public interface INonPlayerCharacter
    {
        /// <summary>
        /// Gets or sets the coords for the turning points on the path of the object.
        /// </summary>
        IList<Vector> PathCoordinates { get; set; }

        /// <summary>
        /// Gets or sets the next point the npc turns at. further points can be easily found through the indexOf overload of PathCoordinates.
        /// </summary>
        Vector NextTurn { get; set; }

        /// <summary>
        /// Gets or sets the value determining the path should be done indefinately in a loop.
        /// </summary>
        bool IsRepeatingPath { get; set; }

        /// <summary>
        /// Gets or sets the speed of the npc in pixel per sec.
        /// </summary>
        int Speed { get; set; }

        /// <summary>
        /// Gets or Sets the last time the object moved.
        /// </summary>
        DateTime TimeOfLastMove { get; set; }

        /// <summary>
        /// Facing direction of the object.
        /// </summary>
        /// <returns>return the normalized direction containing vector.</returns>
        Vector GetDirection();
    }
}
