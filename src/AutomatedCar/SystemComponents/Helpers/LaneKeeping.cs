namespace AutomatedCar.SystemComponents.Helpers
{
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LaneKeeping : SystemComponent
    {
        private LaneKeepingPacket laneKeepingPacket;

        private ICameraPacket cameraPacket;

        private readonly int MAX_DISENGAGE_DISTANCE = 3;

        public LaneKeeping(VirtualFunctionBus virtualFunctionBus)
            : base(virtualFunctionBus)
        {
            this.laneKeepingPacket = new LaneKeepingPacket();
            virtualFunctionBus.LaneKeepingPacket = laneKeepingPacket;
            this.cameraPacket = virtualFunctionBus.CameraPacket;
        }

        public override void Process()
        {
            if (this.laneKeepingPacket.LaneKeepingStatus == LaneKeepingStatus.Active 
                && !this.laneKeepingPacket.CarCentered)
            {
                this.CenterCar();
                this.ChangeLaneKeepingStatus(this.cameraPacket.Roads);
            }
        }

        public void CenterCar()
        {

        }

        public void ChangeLaneKeepingStatus(IEnumerable<IWorldObject> roads)
        {
            if (roads.ToList()[0].Filename != "road_2lane_straight.png"
                && roads.ToList()[0].Filename != "road_2lane_6left.png"
                && roads.ToList()[0].Filename != "road_2lane_6right.png")
            {
                this.laneKeepingPacket.LaneKeepingStatus = LaneKeepingStatus.Inactive;
                return;
            }

            for (int i = 1; i < roads.Count() && i < this.MAX_DISENGAGE_DISTANCE; i++)
            {
                if (roads.ToList()[i].Filename != "road_2lane_straight.png"
                    && roads.ToList()[i].Filename != "road_2lane_6left.png"
                    && roads.ToList()[i].Filename != "road_2lane_6right.png")
                {
                    this.laneKeepingPacket.LaneKeepingStatus = LaneKeepingStatus.Disengaging;
                    break;
                }
            }
        }
    }
}
