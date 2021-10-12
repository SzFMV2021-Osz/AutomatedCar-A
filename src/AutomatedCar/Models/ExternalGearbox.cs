namespace AutomatedCar.Models
{
    using Avalonia.Media;
    using ReactiveUI;
    using System;
    using SystemComponents;

    public class ExternalGearbox : ReactiveObject
    {
        public enum Gear { P = 0, R = 1, N = 2, D = 3 }
        private Gear currentExternalGear = Gear.P;
        private AutomatedCar automatedCar;

        public ExternalGearbox(AutomatedCar automatedCar)
        {
            this.automatedCar = automatedCar;
        }

        public Gear CurrentExternalGear
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

        public void ExternalUpshift()
        {
            if (CurrentExternalGear != Gear.D)
            {
                if ((automatedCar.Velocity.Y <= 0 && CurrentExternalGear == Gear.N) || automatedCar.Speed == 0 || CurrentExternalGear == Gear.R)
                {
                    CurrentExternalGear += 1;
                }
            }
        }

        public void ExternalDownshift()
        {
            if (CurrentExternalGear != 0)
            {
                if ((automatedCar.Velocity.Y >= 0 && CurrentExternalGear == Gear.N) || automatedCar.Speed == 0 || CurrentExternalGear == Gear.D)
                {
                    CurrentExternalGear -= 1;
                }
            }
        }
    }
}
