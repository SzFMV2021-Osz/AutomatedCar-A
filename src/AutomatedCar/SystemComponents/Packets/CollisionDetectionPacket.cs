namespace AutomatedCar.SystemComponents.Helpers
{
    using ReactiveUI;

    /// <summary>
    /// Stores the collision detection packet data.
    /// </summary>
    public class CollisionDetectionPacket : ReactiveObject, ICollisionDetectionPacket
    {
        private CollisionType typeOfCollision;

        /// <inheritdoc />
        public CollisionType TypeOfCollision
        {
            get
            {
                return this.typeOfCollision;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.typeOfCollision, value);
            }
        }
    }
}