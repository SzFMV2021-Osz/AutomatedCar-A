namespace AutomatedCar.SystemComponents.Packets
{
    using AutomatedCar.SystemComponents.Helpers;
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LaneKeepingPacket : ReactiveObject, ILaneKeepingPacket
    {
        private LaneKeepingStatus laneKeepingEngaged = LaneKeepingStatus.Inactive;
        private bool carCentered = false;

        public LaneKeepingStatus LaneKeepingStatus {
            get => this.laneKeepingEngaged; 
            set => this.RaiseAndSetIfChanged(ref this.laneKeepingEngaged, value);
        }

        public bool CarCentered => this.carCentered;
    }
}
