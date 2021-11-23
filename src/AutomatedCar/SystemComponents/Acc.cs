namespace AutomatedCar.SystemComponents
{
    using ReactiveUI;
    using Models;
    using System;

    public class Acc : SystemComponent, IDummyAcc
    {
        private const double minDistance = 0.8;
        private const double maxDistance = 1.4;
        private const double distanceTick = 0.2;

        private const int minSpeed = 30;
        private const int maxSpeed = 160;
        private const int speedTick = 10;

        private readonly AutomatedCar Car;

        public Acc(AutomatedCar car, VirtualFunctionBus virtualFunctionBus) : base(virtualFunctionBus)
        {
            this.Car = car;
            this.IsAccOn = false;
            this.AccDistance = minDistance;
            this.AccSpeed = minSpeed;
        }

        public bool IsAccOn { get; set; }
        public double AccDistance { get; set; }
        public int AccSpeed { get; set; }

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

        public override void Process()
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
