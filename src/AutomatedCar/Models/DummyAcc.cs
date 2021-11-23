namespace AutomatedCar.Models
{
    using ReactiveUI;

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
            if (!IsAccOn)
            {
                AccSpeed = automatedCar.Speed > minSpeed ? automatedCar.Speed : minSpeed;
            }

            IsAccOn = !IsAccOn;
        }

        public void SwitchDistance()
        {
            AccDistance = AccDistance < maxDistance ? AccDistance + distanceTick : minDistance;
        }

        public void IncreaseSpeed()
        {
            if (AccSpeed < maxSpeed)
            {
                AccSpeed += speedTick;
            }
        }
        public void DecreaseSpeed()
        {
            if (AccSpeed > minSpeed)
            {
                AccSpeed -= speedTick;
            }
        }
    }
}
