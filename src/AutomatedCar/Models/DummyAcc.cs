namespace AutomatedCar.Models
{
    using ReactiveUI;
    using System;

    public class DummyAcc : ReactiveObject, IDummyAcc
    {
        private const double minDistance = 0.8;
        private const double maxDistance = 1.4;
        private const double distanceTick = 0.2;

        private const int minSpeed = 30;
        private const int maxSpeed = 160;
        private const int speedTick = 10;


        private AutomatedCar automatedCar;
        private bool isAccOn;
        private double accDistance;
        private int accSpeed;

        public DummyAcc(AutomatedCar automatedCar)
        {
            this.automatedCar = automatedCar;
            this.isAccOn = false;
            this.accDistance = minDistance;
            this.accSpeed = minSpeed;
        }

        public bool IsAccOn { get => this.isAccOn; set => this.RaiseAndSetIfChanged(ref this.isAccOn, value); }
        public double AccDistance { get => this.accDistance; set => this.RaiseAndSetIfChanged(ref this.accDistance, value); }
        public int AccSpeed { get => this.accSpeed; set => this.RaiseAndSetIfChanged(ref this.accSpeed, value); }

        public void ToggleAcc()
        {
            if (!IsAccOn && automatedCar.Speed > minSpeed)
            {
                AccSpeed = automatedCar.Speed;
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
    }
}
