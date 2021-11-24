namespace AutomatedCar.SystemComponents.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets;
    using Avalonia;
    using Avalonia.Media;

    public class LaneKeepingAssistant : SystemComponent
    {
        private readonly LKAPacket laneKeepingPacket;
        private AutomatedCar car;
        private IList<WorldObject> roads;

        public LaneKeepingAssistant(VirtualFunctionBus virtualFunctionBus)
            : base(virtualFunctionBus)
        {
            this.laneKeepingPacket = new LKAPacket();
            virtualFunctionBus.LaneKeepingPacket = this.laneKeepingPacket;
        }

        public override void Process()
        {
            this.car = World.Instance.ControlledCar;
            this.roads = this.virtualFunctionBus.CameraPacket.Roads;
            this.LKAAvailibility();
            if (this.car.LKAModel.CurrentLaneKeepingStatus != LaneKeepingStatus.Inactive)
            {
                if (this.car.LKAModel.CurrentLaneKeepingStatus != LaneKeepingStatus.Disengaging)
                {
                    this.CenterCar();
                }

                this.ChangeLKAStatus();
            }
        }

        public void CenterCar()
        {
            WorldObject closest = Utils.FindClosestObject(this.roads, this.car);

            if (!(this.car.Speed == 0 || this.car.Gearbox.CurrentExternalGearPosition != Gear.D || closest == null))
            {
                this.RotateCar(closest.Rotation);
                this.MoveTowards(closest.Geometries, closest);
            }
        }

        public void RotateCar(double closestRoadRotation)
        {
            if (this.car.Rotation > closestRoadRotation + 2 || (closestRoadRotation == 270 && this.car.Rotation < -65))
            {
                this.car.LKAModel.SteerCarLeft();
            }
            else if (this.car.Rotation < closestRoadRotation - 2)
            {
                this.car.LKAModel.SteerCarRight();
            }
        }

        public void MoveTowards(ObservableCollection<PolylineGeometry> laneGeometries, WorldObject closest)
        {
            int direction;

            switch (closest.Rotation)
            {
                case 0:
                    if (!this.IsCarRotationBetween(-15, 15)) break;

                    direction = this.car.X > laneGeometries[2].Bounds.X - 20 ? 1 : 0;
                    this.car.LKAModel.MoveCar(direction);

                    break;

                case 90:
                    if (!this.IsCarRotationBetween(75, 105)) break;

                    direction = this.car.Y > laneGeometries[2].Bounds.Y - 50 ? 3 : 2;
                    this.car.LKAModel.MoveCar(direction);

                    break;

                case 180:
                    if (!this.IsCarRotationBetween(165, 195)) break;

                    direction = this.car.X > laneGeometries[2].Bounds.X + closest.X + 200 ? 1 : 0;
                    this.car.LKAModel.MoveCar(direction);

                    break;

                case 270:
                    if (this.IsCarRotationBetween(-75, 255)) break;

                    direction = this.car.Y > laneGeometries[2].Bounds.Y + closest.Y ? 3 : 2;
                    this.car.LKAModel.MoveCar(direction);

                    break;
            }
        }

        public bool IsCarRotationBetween(int start, int end)
        {
            return this.car.Rotation > start && this.car.Rotation < end;
        }

        public void ChangeLKAStatus()
        {
            WorldObject carUnderRoad = this.CarUnderRoad();

            if (carUnderRoad == null)
            {
                this.car.LKAModel.CurrentLaneKeepingStatus = LaneKeepingStatus.Inactive;
            }
            else
            {
                List<WorldObject> roadsExceptCurrent = this.roads.Where(road => road != carUnderRoad).ToList();
                WorldObject closestRoad = Utils.FindClosestObject(roadsExceptCurrent, this.car);
                if (closestRoad != null && !CanLKAWorkOnRoad(closestRoad.Filename))
                {
                    this.car.LKAModel.CurrentLaneKeepingStatus = LaneKeepingStatus.Disengaging;
                }
            }
        }

        private static bool CanLKAWorkOnRoad(string filename)
        {
            return filename == "road_2lane_straight.png" || filename == "road_2lane_6left.png" || filename == "road_2lane_6right.png";
        }

        private WorldObject CarUnderRoad()
        {
            return World.Instance.WorldObjects
                .Where(wo => CanLKAWorkOnRoad(wo.Filename) && this.RoadGeometryContains(wo))
                .FirstOrDefault();
        }

        private bool RoadGeometryContains(WorldObject currentRoad)
        {
            List<Point> points = new ();

            Point refPoint = new (0, 0);
            if (Utils.ReferencePoints.Any(r => r.Type + ".png" == currentRoad.Filename))
            {
                ReferencePoint currRefPoint = Utils.ReferencePoints.Where(r => r.Type + ".png" == currentRoad.Filename).FirstOrDefault();
                refPoint = new (currRefPoint.X, currRefPoint.Y);
            }

            Point startingPoint = currentRoad.Geometries[0].Points.FirstOrDefault();

            foreach (var currPoint in currentRoad.Geometries[0].Points)
            {
                points.Add(new (currPoint.X + refPoint.X + currentRoad.X, currPoint.Y + refPoint.Y + currentRoad.Y));
            }

            foreach (var currPoint in currentRoad.Geometries[2].Points.Reverse())
            {
                points.Add(new (currPoint.X + refPoint.X + currentRoad.X, currPoint.Y + refPoint.Y + currentRoad.Y));
            }

            points.Add(new (startingPoint.X + refPoint.X + currentRoad.X, startingPoint.Y + refPoint.Y + currentRoad.Y));

            PolylineGeometry roadGeometry = new (points, true);

            Point carPoint = new (this.car.X + this.car.RotationPoint.X, this.car.Y + this.car.RotationPoint.Y);

            return roadGeometry.FillContains(carPoint);
        }

        private void LKAAvailibility()
        {
            this.car.LKAModel.LaneKeepingAvailibility = this.CarUnderRoad() != null;
        }
    }
}