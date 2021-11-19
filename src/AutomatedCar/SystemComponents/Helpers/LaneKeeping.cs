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
            AutomatedCar car = World.Instance.ControlledCar;
            if (car.LaneKeepingMod.CurrentLaneKeepingStatus == LaneKeepingStatus.Active)
            {
                this.CenterCar(this.cameraPacket.RelevantObjects, car);

                IEnumerable<WorldObject> roads = this.cameraPacket.RelevantObjects;
                ;
                //TODO
                //this.ChangeLaneKeepingStatus(this.cameraPacket.RelevantObjects);
            }
        }

        public void CenterCar(IEnumerable<WorldObject> roads, AutomatedCar car)
        {
            WorldObject closest = Closest(roads, car);
            ;
            if (car.Speed != 0 && car.Gearbox.CurrentExternalGearPosition == Gear.D && closest != null)
            {
               
                if (car.Rotation > closest.Rotation + 2)
                {
                    car.TurnLeft();
                    car.CalculateNextPosition();
                }
                else if (car.Rotation < closest.Rotation - 2)
                {
                    car.TurnRight();
                    car.CalculateNextPosition();
                }
                
                if (closest.Rotation == 90)
                {
                    if (car.Rotation < 105 && car.Rotation > 75 && car.Y > closest.Geometries[2].Bounds.Y - 50)
                    {
                        car.Y -= 1;
                    }
                    else if (car.Rotation < 105 && car.Rotation > 75 && car.Y < closest.Geometries[2].Bounds.Y - 50)
                    {
                        car.Y += 1;
                    }
                    else if (car.Rotation > 255 && car.Rotation < -75 && car.Y > closest.Geometries[1].Bounds.Y)
                    {
                        car.Y -= 1;
                    }
                    else if (car.Rotation > 255 && car.Rotation < -75 && car.Y < closest.Geometries[1].Bounds.Y)
                    {
                        car.Y += 1;
                    }
                }
                else if (closest.Rotation == 0) 
                {
                    if (car.Rotation < 15 && car.Rotation > -15 && car.X > closest.Geometries[2].Bounds.X)
                    {
                        car.X -= 1;
                    }
                    else if (car.Rotation < 15 && car.Rotation > -15 && car.X < closest.Geometries[2].Bounds.X)
                    {
                        car.X += 1;
                    }
                    else if (car.Rotation > 165 && car.Rotation < 195 && car.X > closest.Geometries[1].Bounds.X)
                    {
                        car.X -= 1;
                    }
                    else if (car.Rotation > 165 && car.Rotation < 195 && car.X < closest.Geometries[1].Bounds.X)
                    {
                        car.X += 1;
                    }
                }
            }
        }

        public WorldObject Closest(IEnumerable<WorldObject> roads, AutomatedCar car)
        {
            List<int> distances = new List<int>();

            foreach (WorldObject wo in roads)
            {
                distances.Add((int) Math.Sqrt(Math.Pow(car.X - wo.X, 2) + Math.Pow(car.Y - wo.Y, 2)));
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
