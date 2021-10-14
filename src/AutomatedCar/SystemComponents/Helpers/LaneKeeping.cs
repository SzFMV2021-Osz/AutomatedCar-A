namespace AutomatedCar.SystemComponents.Helpers
{
    using AutomatedCar.SystemComponents.Packets;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LaneKeeping : SystemComponent
    {
        private LaneKeepingPacket laneKeepingPacket;

        public LaneKeeping(VirtualFunctionBus virtualFunctionBus)
            : base(virtualFunctionBus)
        {
            this.laneKeepingPacket = new LaneKeepingPacket();
            virtualFunctionBus.LaneKeepingPacket = laneKeepingPacket;
        }
    }
}
