namespace AutomatedCar.SystemComponents.Helpers
{
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets;
    using Avalonia.Media;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LaneKeeping : SystemComponent
    {
        private LaneKeepingPacket laneKeepingPacket;
        private ICameraPacket cameraPacket;
        private AutomatedCar car;

        private readonly int MAX_DISENGAGE_DISTANCE = 3;

        public LaneKeeping(VirtualFunctionBus virtualFunctionBus)
            : base(virtualFunctionBus)
        {
            this.laneKeepingPacket = new LaneKeepingPacket();
            virtualFunctionBus.LaneKeepingPacket = this.laneKeepingPacket;
            this.cameraPacket = virtualFunctionBus.CameraPacket;
        }

        public override void Process()
        {
            this.car = World.Instance.ControlledCar;

            if (this.car.LaneKeepingMod.CurrentLaneKeepingStatus == LaneKeepingStatus.Active)
            {
                this.CenterCar(this.cameraPacket.RelevantObjects);

                IEnumerable<WorldObject> roads = this.cameraPacket.RelevantObjects;
                ;
                //TODO
                //this.ChangeLaneKeepingStatus(this.cameraPacket.RelevantObjects);
            }
        }

        public void CenterCar(IEnumerable<WorldObject> roads)
        {
            WorldObject closest = Closest(roads);

            if (this.car.Speed == 0 || this.car.Gearbox.CurrentExternalGearPosition != Gear.D || closest == null)
            {
                return;
            }

            this.RotateCar(closest.Rotation);
            this.MoveTowards(closest.Geometries, closest);
        }

        public void RotateCar(double closestRoadRotation)
        {
            if (this.car.Rotation > closestRoadRotation + 2 || (closestRoadRotation == 270 && this.car.Rotation < -65))
            {
                this.car.LaneKeepingMod.SteerCarLeft();
            }
            else if (this.car.Rotation < closestRoadRotation - 2)
            {
                this.car.LaneKeepingMod.SteerCarRight();
            }
        }

        public void MoveTowards(ObservableCollection<PolylineGeometry> laneGeometries, WorldObject closest)
        {
            int direction;

            switch (closest.Rotation)
            {
                case 0:
                    if (!CarRotationBetween(-15, 15)) break;

                    direction = this.car.X > laneGeometries[2].Bounds.X - 20 ? 1 : 0;
                    this.car.LaneKeepingMod.MoveCar(direction);

                    break;

                case 90:
                    if (!CarRotationBetween(75, 105)) break;

                    direction = this.car.Y > laneGeometries[2].Bounds.Y - 50 ? 3 : 2;
                    this.car.LaneKeepingMod.MoveCar(direction);

                    break;

                case 180:
                    if (!CarRotationBetween(165, 195)) break;

                    direction = this.car.X > laneGeometries[2].Bounds.X + closest.X + 200 ? 1 : 0;
                    this.car.LaneKeepingMod.MoveCar(direction);

                    break;

                case 270:
                    if (CarRotationBetween(-75, 255)) break;

                    direction = this.car.Y > laneGeometries[2].Bounds.Y + closest.Y ? 3 : 2;
                    this.car.LaneKeepingMod.MoveCar(direction);

                    break;
            }
        }

        public bool CarRotationBetween(int start, int end)
        {
            return this.car.Rotation > start && this.car.Rotation < end;
        }

        public WorldObject Closest(IEnumerable<WorldObject> roads)
        {
            List<int> distances = new List<int>();

            foreach (WorldObject wo in roads)
            {
                distances.Add((int) Math.Sqrt(Math.Pow(this.car.X - wo.X, 2) + Math.Pow(this.car.Y - wo.Y, 2)));
            }

            if (distances.Count() == 0)
            {
                return null;
            }

            return roads.ToList()[distances.IndexOf(distances.Min())];
        }

        public void ChangeLaneKeepingStatus(IEnumerable<WorldObject> roads)
        {
            /*
             "road_2lane_straight.png"
             "road_2lane_6left.png"
            "road_2lane_6right.png"

            World.Instance.ControlledCar.LaneKeepingMod.CurrentLaneKeepingStatus
             */
        }
    }
}
