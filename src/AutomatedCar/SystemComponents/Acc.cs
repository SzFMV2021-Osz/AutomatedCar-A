namespace AutomatedCar.SystemComponents
{
    using ReactiveUI;
    using Models;
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Collections.Generic;
    using System.Linq;

    public class Acc : SystemComponent, IAcc, INotifyPropertyChanged
    {
        enum AccMode
        {
            SpeedKeeping,
            CarFollowing
        }

        private const double minDistance = 0.8;
        private const double maxDistance = 1.4;
        private const double distanceTick = 0.2;
        private double accDistance;

        private const int minSpeed = 30;
        private const int maxSpeed = 160;
        private const int speedTick = 10;
        private int accSpeed;

        private bool isAccOn;

        private readonly AutomatedCar Car;
        private AccMode mode;

        //private Vector objectPositionInLaneT0 = null;
        //private DateTime TimeOfSample { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Acc(AutomatedCar car, VirtualFunctionBus virtualFunctionBus) : base(virtualFunctionBus)
        {
            this.Car = car;
            this.IsAccOn = false;
            this.AccDistance = minDistance;
            this.AccSpeed = minSpeed;
        }

        public bool IsAccOn
        {
            get
            {
                return this.isAccOn;
            }

            set
            {
                if (value != this.isAccOn)
                {
                    this.isAccOn = value;
                    NotifyPropertyChanged(nameof(IsAccOn));
                }
            }
        }

        public double AccDistance
        {
            get
            {
                return this.accDistance;
            }

            set
            {
                if (value != this.accDistance)
                {
                    this.accDistance = value;
                    NotifyPropertyChanged(nameof(AccDistance));
                }
            }
        }

        public int AccSpeed
        {
            get
            {
                return this.accSpeed;
            }

            set
            {
                if (value != this.accSpeed)
                {
                    this.accSpeed = value;
                    NotifyPropertyChanged(nameof(AccSpeed));
                }
            }
        }

        public void ToggleAcc()
        {
            if (!IsAccOn && Car.Speed > minSpeed)
            {
                AccSpeed = Car.Speed;
                mode = AccMode.SpeedKeeping;
                IsAccOn = true;
            }
            else if (IsAccOn)
            {
                IsAccOn = false;
            }
        }

        public void SwitchDistance()
        {
            AccDistance = AccDistance < maxDistance ? AccDistance + distanceTick : minDistance;
        }

        public void IncreaseSpeed()
        {
            var nextSpeed = RoundUpByTick(AccSpeed);
            if (nextSpeed <= maxSpeed)
            {
                AccSpeed = nextSpeed;
            }
            else
            {
                AccSpeed = maxSpeed;
            }
        }
        public void DecreaseSpeed()
        {
            var nextSpeed = RoundDownByTick(AccSpeed);
            if (AccSpeed > minSpeed)
            {
                AccSpeed = nextSpeed;
            }
            else
            {
                AccSpeed = minSpeed;
            }
        }

        private int RoundUpByTick(int speed)
        {
            return (int)Math.Floor((speed + speedTick) / 10.0) * 10;
        }

        private int RoundDownByTick(int speed)
        {
            return (int)Math.Ceiling((speed - speedTick) / 10.0) * 10;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Action AccFunctionalityToExecute => (IsAccOn, mode) switch
        {
            (true, AccMode.CarFollowing) => DoFollowOperation,
            (true, AccMode.SpeedKeeping) => DoSpeedKeeping,
            (false, _) => new Action(() => { }),
            _ => throw new ArgumentException("The AccMode state was invalid!"),
        };

        public override void Process()
        {
            var objInLine = virtualFunctionBus.RadarPacket.ClosestObjectInLane;
            if(objInLine != null)
            {
                mode = AccMode.CarFollowing;
            }
            else
            {
                mode = AccMode.SpeedKeeping;
            }

            AccFunctionalityToExecute();
        }

        public void DoFollowOperation()
        {
            var newTime = DateTime.Now;
            var objInLine = virtualFunctionBus.RadarPacket.ClosestObjectInLane;

            if (objInLine != null)
            {
                var deltaPosition = objInLine.GetLocation() - Car.GetLocation();
                var deltaDistance = deltaPosition.GetLength();
                var distanceInTime = deltaDistance / Car.Speed; //TODO: pix/s or m/s ?? huh

                double deltaTime = accDistance - distanceInTime;
                (var gasPosition, var breakPosition) = deltaTime switch
                {
                    > 0 => AccBreakCar(deltaTime),
                    < 0 => AccAccelerateCar(deltaTime),
                    _ => (Car.GasPedalPosition, Car.BrakePedalPosition),
                };
                Car.GasPedalPosition = gasPosition;
                Car.BrakePedalPosition = breakPosition;
            }
        }

        private (int gasPosition, int breakPosition) AccAccelerateCar(double deltaTime)
        {
            return (0, AutomatedCar.BoundPedalPosition(Car.GasPedalPosition + 1));
        }

        private (int gasPosition, int breakPosition) AccBreakCar(double deltaTime)
        {
            return (AutomatedCar.BoundPedalPosition(Car.BrakePedalPosition + 1), 0);
        }

        public void DoSpeedKeeping()
        {
            
        }
    }
}
