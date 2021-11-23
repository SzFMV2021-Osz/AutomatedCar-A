namespace AutomatedCar.SystemComponents
{
    using ReactiveUI;
    using Models;
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Acc : SystemComponent, IAcc, INotifyPropertyChanged
    {
        private const double minDistance = 0.8;
        private const double maxDistance = 1.4;
        private const double distanceTick = 0.2;

        private const int minSpeed = 30;
        private const int maxSpeed = 160;
        private const int speedTick = 10;

        private bool isAccOn;
        private int accSpeed;
        private double accDistance;

        private readonly AutomatedCar Car;

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

        public override void Process()
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
