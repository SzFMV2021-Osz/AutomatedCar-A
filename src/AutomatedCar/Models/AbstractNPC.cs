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
            var dir = new Vector() { X = this.NextTurn.X - this.X, Y = this.NextTurn.Y - this.Y };
            var len = Math.Sqrt((dir.X * dir.X) + (dir.Y * dir.Y));
            if (len > 0)
            {
                return new Vector() { X = dir.X / len, Y = dir.Y / len };
            }
            else
            {
                return new Vector() { X = 0, Y = 0 };
            }
        }

        // inheritdoc..
        public DateTime TimeOfLastMove { get; set; }

        // inheritdoc..
        public bool IsRepeatingPath { get; set; }

        private double PixelsToMove(DateTime timeOfMovement)
        {
            var timeElapsed = timeOfMovement - this.TimeOfLastMove;
            return this.Speed * timeElapsed.TotalMilliseconds / 1000;
        }

        private double GetDistance(Vector destination)
        {
            var dir = new Vector() { X = destination.X - this.X, Y = destination.Y - this.Y };
            return Math.Sqrt((dir.X * dir.X) + (dir.Y * dir.Y));
        }

        private void MoveForward(double distanceToMove)
        {
            var distanceFromNextTurn = GetDistance(NextTurn);

            if(distanceFromNextTurn > distanceToMove)
            {
                UpdateNpcPosition(distanceToMove);
            }
            else
            {
                var remainingDistanceToTravel = distanceToMove - distanceFromNextTurn;
                UpdateNpcPosition(distanceFromNextTurn);
                var currentTurnIdx = PathCoordinates.IndexOf(NextTurn);
                if(PathCoordinates.Count > currentTurnIdx)
                {
                    NextTurn = PathCoordinates[currentTurnIdx + 1];
                    MoveForward(remainingDistanceToTravel);
                }
                else
                {
                    if(IsRepeatingPath)
                    {
                        NextTurn = PathCoordinates[0];
                        MoveForward(remainingDistanceToTravel);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        private void UpdateNpcPosition(double distanceToMove)
        {
            var dir = GetDirection();
            this.PreciseX += dir.X * distanceToMove;
            this.PreciseY += dir.Y * distanceToMove;
        }

        /// <summary>
        /// Responsible to calculate and set the object's next position.
        /// </summary>
        public void StepObject()
        {
            var timeOfMovement = DateTime.Now;

            MoveForward(PixelsToMove(timeOfMovement));
            SetRotation();

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
