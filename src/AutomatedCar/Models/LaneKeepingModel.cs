namespace AutomatedCar.Models
{
    using global::AutomatedCar.SystemComponents.Helpers;
    using ReactiveUI;

    public class LaneKeepingModel : ReactiveObject
    {
        private LaneKeepingStatus currentLaneKeepingStatus = LaneKeepingStatus.Inactive;
        private bool available;
        private AutomatedCar car;

        public LaneKeepingModel(AutomatedCar car, bool laneKeepingAvailable = true)
        {
            this.car = car;
            this.LaneKeepingAvailibility = laneKeepingAvailable;
        }

        public void SteerCarLeft()
        {
            this.car.TurnLeft();
            this.car.CalculateNextPosition();
        }

        public void SteerCarRight()
        {
            this.car.TurnRight();
            this.car.CalculateNextPosition();
        }

        public void MoveCar(int direction)
        {
            switch (direction)
            {
                case 0:
                    this.car.X += 1;
                    break;
                case 1:
                    this.car.X -= 1;
                    break;
                case 2:
                    this.car.Y += 1;
                    break;
                case 3:
                    this.car.Y -= 1;
                    break;
            }
        }

        public void DisengagingLaneKeeping()
        {
            this.CurrentLaneKeepingStatus = LaneKeepingStatus.Disengaging;
        }

        public void DisengageLaneKeeping()
        {
            this.CurrentLaneKeepingStatus = LaneKeepingStatus.Inactive;
        }

        public void EngageLaneKeeping()
        {
            if (this.available)
            {
                this.CurrentLaneKeepingStatus = LaneKeepingStatus.Active;
            }
        }

        public void ToggleLaneKeeping()
        {
            if (this.currentLaneKeepingStatus == LaneKeepingStatus.Active && this.available)
            {
                this.DisengageLaneKeeping();
            }
            else
            {
                this.EngageLaneKeeping();
            }
        }

        public LaneKeepingStatus CurrentLaneKeepingStatus
        {
            get => this.currentLaneKeepingStatus;
            set => this.RaiseAndSetIfChanged(ref this.currentLaneKeepingStatus, value);
        }

        public bool LaneKeepingAvailibility
        {
            get => this.available;
            set => this.RaiseAndSetIfChanged(ref this.available, value);
        }
    }
}