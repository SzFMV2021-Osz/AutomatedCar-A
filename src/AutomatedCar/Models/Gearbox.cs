namespace AutomatedCar.Models
{
    using Avalonia.Media;
    using ReactiveUI;
    using System;
    using SystemComponents;

    public enum Gear { P = 0, R = 1, N = 2, D = 3 }
    public enum Shifting { Down, None, Up}

    public class Gearbox : ReactiveObject, IGearbox
    {
        private Gear currentExternalGearPosition = Gear.P;
        private int currentInternalGear = 0;
        private AutomatedCar automatedCar;
        public Shifting InnerShiftingStatus { get; set; } = Shifting.None;

        public Gearbox(AutomatedCar automatedCar)
        {
            this.automatedCar = automatedCar;
        }

        public Gear CurrentExternalGearPosition
        {
            get => this.currentExternalGearPosition;

            set => this.RaiseAndSetIfChanged(ref this.currentExternalGearPosition, value);
        }

        public int CurrentInternalGear 
        {
            get => this.currentInternalGear;
            set => this.RaiseAndSetIfChanged(ref this.currentInternalGear, value);
        }

        public void ExternalUpshift()
        {
            if (currentExternalGearPosition != Gear.D)
            {
                if ((automatedCar.Velocity.Y <= 0 && currentExternalGearPosition == Gear.N) || automatedCar.Speed == 0 || currentExternalGearPosition == Gear.R)
                {
                    CurrentExternalGearPosition += 1;
                }
            }
        }

        public void ExternalDownshift()
        {
            if (currentExternalGearPosition != 0)
            {
                if ((automatedCar.Velocity.Y >= 0 && currentExternalGearPosition == Gear.N) || automatedCar.Speed == 0 || currentExternalGearPosition == Gear.D)
                {
                    CurrentExternalGearPosition -= 1;
                }
            }
        }

        public void InternalDownshift()
        {
            if (currentExternalGearPosition == Gear.D || currentExternalGearPosition == Gear.N)
            {
                currentInternalGear = Math.Max(currentInternalGear--, 0);
                InnerShiftingStatus = Shifting.Down;
            }
        }

        public void InternalUpshift()
        {
            if (currentExternalGearPosition == Gear.D)
            {
                currentInternalGear = Math.Min(currentInternalGear++, 4);
                InnerShiftingStatus = Shifting.Up;
            }
        }
    }
}
