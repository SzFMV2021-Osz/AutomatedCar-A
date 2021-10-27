namespace AutomatedCar.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Implements the common parts of npc car and pedestrian.
    /// </summary>
    public abstract class AbstractNPC : WorldObject, INonPlayerCharacter
    {
        public AbstractNPC(int x, int y, string filename, WorldObjectType worldObjectType) : base(x, y, filename, 1, true, worldObjectType)
        {
            this.NextTurn = this.PathCoordinates[2];
        }

        // inhetritdoc..
        public IList<Vector> PathCoordinates { get; set; }

        // inhetritdoc..
        public Vector NextTurn { get; set; }

        // inhetritdoc..
        public int Speed { get; set; }

        // inhetritdoc..
        public Vector GetDirection()
        {
            return new Vector() { X = this.NextTurn.X - this.X, Y = this.NextTurn.Y - this.Y };
        }

        // inheritdoc..
        public DateTime TimeOfLastMove { get; set; }

        private int PixelsToMove(DateTime timeOfMovement)
        {
            var timeElapsed = timeOfMovement - this.TimeOfLastMove;
            return this.Speed * (Convert.ToInt32(timeElapsed.TotalMilliseconds) / 1000);
        }

        /// <summary>
        /// Responsible to calculate and set the object's next position.
        /// </summary>
        public void StepObject()
        {
            SetRotation();
            var timeOfMovement = DateTime.Now;
            var pixelsToMove = this.PixelsToMove(timeOfMovement);

            //TODO: algo

            this.TimeOfLastMove = timeOfMovement;
        }

        /// <summary>
        /// Set the rotation in degrees based on the direction
        /// </summary>
        /// <returns>Returns the rotation in degrees</returns>
        public void SetRotation(){
            var direction = GetDirection();
            this.Rotation = Math.Atan2(direction.Y, direction.X) * (180/Math.PI) + 90;
        }
    }
}
