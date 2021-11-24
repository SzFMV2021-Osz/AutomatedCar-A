namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.SystemComponents.Helpers;
    using ReactiveUI;

    public class LKAPacket : ReactiveObject, ILKAPacket
    {
        private LaneKeepingStatus laneKeepingEngaged = LaneKeepingStatus.Inactive;

        public LaneKeepingStatus LaneKeepingStatus {
            get => this.laneKeepingEngaged;
            set => this.RaiseAndSetIfChanged(ref this.laneKeepingEngaged, value);
        }

        public bool CarCentered { get; }
    }
}