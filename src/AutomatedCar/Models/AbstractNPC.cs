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
        public AbstractNPC(int x, int y, string filename, WorldObjectType worldObjectType, bool isRepeatingPath) 
            : base(x, y, filename, 1, true, worldObjectType)
        {
            IsRepeatingPath = isRepeatingPath;
            if (PathCoordinates.Any())
            {
                this.NextTurn = this.PathCoordinates[0]; 
            }
            else
            {
                NextTurn = new Vector() { X = x, Y = y };
            }
        }

        // inhetritdoc..
        public IList<Vector> PathCoordinates { get; set; } = new List<Vector>();

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
        public DateTime TimeOfLastMove { get; set; } = DateTime.Now;

        // inheritdoc..
        public bool IsRepeatingPath { get; set; } = false;

        private double PixelsToMove(DateTime timeOfMovement)
        {
            var timeElapsed = timeOfMovement - this.TimeOfLastMove;
            return this.Speed * timeElapsed.TotalMilliseconds / 1000;
        }

        private double GetDistance(Vector destination)
        {
            var dir = new Vector() { X = destination.X - this.PreciseX, Y = destination.Y - this.PreciseY };
            return dir.GetLength();
        }

        /// Recursive function only modify with extreme caution.
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
                if(PathCoordinates.Count > currentTurnIdx + 1)
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
        public void StepObject(DateTime timeOfMovement)
        {
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
