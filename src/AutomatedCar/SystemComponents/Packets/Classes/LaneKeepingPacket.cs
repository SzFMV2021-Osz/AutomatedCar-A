namespace AutomatedCar.SystemComponents.Packets
{
    using ReactiveUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LaneKeepingPacket : ReactiveObject, ILaneKeepingPacket
    {
        private bool laneKeepingEngaged = false;
        private bool carCentered = false;

        public bool LaneKeepingEngaged 
        { 
            get => throw new NotImplementedException(); 
            set => throw new NotImplementedException(); 
        }

        public bool CarCentered => throw new NotImplementedException();
    }
}
