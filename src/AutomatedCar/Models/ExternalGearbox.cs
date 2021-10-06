namespace AutomatedCar.Models
{
    using Avalonia.Media;
    using ReactiveUI;
    using System;
    using SystemComponents;

    public class ExternalGearbox : ReactiveObject
    {
        private int currentExternalGear = 0;  //0: P, 1: R, 2: N, 3: D
        private AutomatedCar automatedCar;

        public ExternalGearbox(AutomatedCar automatedCar)
        {
            this.automatedCar = automatedCar;
        }

        public int CurrentExternalGear
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
            if (CurrentExternalGear != 3)
            {
                if ((automatedCar.Velocity.Y <= 0 && CurrentExternalGear == 2) || automatedCar.Speed == 0 || CurrentExternalGear == 1)
                {
                    CurrentExternalGear += 1;
                }
            }
        }

        public void ExternalDownshift()
        {
            if (CurrentExternalGear != 0)
            {
                if ((automatedCar.Velocity.Y >= 0 && CurrentExternalGear == 2) || automatedCar.Speed == 0 || CurrentExternalGear == 3)
                {
                    CurrentExternalGear -= 1;
                }
            }
        }
    }
}
