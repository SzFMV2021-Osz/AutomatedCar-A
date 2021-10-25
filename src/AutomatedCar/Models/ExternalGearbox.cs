namespace AutomatedCar.Models
{
    using Avalonia.Media;
    using ReactiveUI;
    using System;
    using SystemComponents;

    public enum Gear { P = 0, R = 1, N = 2, D = 3 }

    public class ExternalGearbox : ReactiveObject, IGearbox
    {
        private Gear currentExternalGear = Gear.P;
        private AutomatedCar automatedCar;

        public ExternalGearbox(AutomatedCar automatedCar)
        {
            this.automatedCar = automatedCar;
        }

        public Gear currentGearPosition
        {
            get
            {
                return this.currentExternalGear;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.currentExternalGear, value);
            }
        }

        public void Upshift()
        {
            if (currentGearPosition != Gear.D)
            {
                if ((automatedCar.Velocity.Y <= 0 && currentGearPosition == Gear.N) || automatedCar.Speed == 0 || currentGearPosition == Gear.R)
                {
                    currentGearPosition += 1;
                }
            }
        }

        public void Downshift()
        {
            if (currentGearPosition != 0)
            {
                if ((automatedCar.Velocity.Y >= 0 && currentGearPosition == Gear.N) || automatedCar.Speed == 0 || currentGearPosition == Gear.D)
                {
                    currentGearPosition -= 1;
                }
            }
        }
    }
}
