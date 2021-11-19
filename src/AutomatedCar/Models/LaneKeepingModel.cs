namespace AutomatedCar.Models
{
    using global::AutomatedCar.SystemComponents.Helpers;
    using ReactiveUI;

    public class LaneKeepingModel : ReactiveObject
    {
        private LaneKeepingStatus currentLaneKeepingStatus = LaneKeepingStatus.Inactive;
        private bool available;

        public LaneKeepingModel(bool laneKeepingAvailable = true)
        {
            this.LaneKeepingAvailibility = laneKeepingAvailable;
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
            this.CurrentLaneKeepingStatus = LaneKeepingStatus.Active;
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