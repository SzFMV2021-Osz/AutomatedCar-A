namespace AutomatedCar.SystemComponents.Helpers
{
    using System;
    using Avalonia;

    /// <summary>
    /// Interface for the collision detection events.
    /// </summary>
    public interface ICollisionDetectionPacket
    {
        /// <summary>
        /// Gets the collision type.
        /// </summary>
        public CollisionType TypeOfCollision { get; }
    }
}