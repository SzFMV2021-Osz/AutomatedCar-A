namespace AutomatedCar.SystemComponents.Helpers
{
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