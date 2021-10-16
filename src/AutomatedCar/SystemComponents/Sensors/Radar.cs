namespace AutomatedCar.SystemComponents.Sensors
{
    using System;
    using System.Collections.Generic;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets;
    using Avalonia;

    public sealed class Radar : Sensor
    {
        public Radar(VirtualFunctionBus virtualFunctionBus)
            : base(virtualFunctionBus, 60, 200)
        {
            this.sensorPacket = new RadarPacket();
            virtualFunctionBus.RadarPacket = (IRadarPacket)this.sensorPacket;
        }

        public override void Process()
        {
            AutomatedCar car = World.Instance.ControlledCar;
            this.CalculateBasicSensorData(car, World.Instance.WorldObjects);
            this.FilterIncoming(car);
        }

        protected override bool IsRelevant(WorldObject worldObject)
        {
            return worldObject.Collideable;
        }

        private void FilterIncoming(AutomatedCar car)
        {
            Point carStartPoint = new Point(car.X, car.Y);
            Point carEndPoint = this.GetEndpoint(carStartPoint, car.Rotation);

            IList<WorldObject> incomingObjects = new List<WorldObject>();
            foreach (WorldObject currObject in this.sensorPacket.RelevantObjects)
            {
                if (!this.IsStationary(currObject))
                {
                    Point objectStartPoint = new Point(currObject.X, currObject.Y);
                    Point objectEndPoint = this.GetEndpoint(objectStartPoint, currObject.Rotation);

                    if (DoIntersect(objectStartPoint, objectEndPoint, carStartPoint, carEndPoint))
                    {
                        incomingObjects.Add(currObject);
                    }
                }
            }

            ((RadarPacket)this.sensorPacket).IncomingObjects = incomingObjects;
        }

        private void FindClosestObjectInLane(AutomatedCar car)
        {
            // TODO: for later PR.
            throw new NotImplementedException();
        }

        // TODO: Code health: Maintain this list with stationary objects. Investigate for options based on speed.
        private bool IsStationary(WorldObject worldObject)
        {
            return worldObject.WorldObjectType == WorldObjectType.Tree || worldObject.WorldObjectType == WorldObjectType.RoadSign;
        }

        private Point GetEndpoint(Point startPoint, double rotation)
        {
            return new Point(startPoint.X + (this.distance * Cosine(rotation)), startPoint.Y + (this.distance * Math.Sin(rotation)));
        }

        private static bool DoIntersect(Point From1, Point To1, Point From2, Point To2)
        {
            int o1 = OrienTation(From1, To1, From2);
            int o2 = OrienTation(From1, To1, To2);
            int o3 = OrienTation(From2, To2, From1);
            int o4 = OrienTation(From2, To2, To1);

            if (o1 != o2 && o3 != o4)
            {
                return true;
            }
            else if (o1 == 0 && OnSegment(From1, From2, To1))
            {
                return true;
            }
            else if (o2 == 0 && OnSegment(From1, To2, To1))
            {
                return true;
            }
            else if (o3 == 0 && OnSegment(From2, From1, To2))
            {
                return true;
            }
            else if (o4 == 0 && OnSegment(From2, To1, To2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static int OrienTation(Point p, Point q, Point r)
        {
            double val = ((q.Y - p.Y) * (r.X - q.X)) - ((q.X - p.X) * (r.Y - q.Y));
            if (val == 0)
            {
                return 0;
            }
            else
            {
                return (val > 0) ? 1 : 2;
            }
        }

        private static bool OnSegment(Point p, Point q, Point r)
        {
            if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) && q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}