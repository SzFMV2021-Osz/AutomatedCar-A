namespace AutomatedCar.Models
{
    using Avalonia.Media;
    using ReactiveUI;
    using System;
    using SystemComponents;

    public class AutomatedCar : Car
    {
        private const int pedalOffset = 16;
        private const int minPedalPosition = 0;
        private const int maxPedalPosition = 100;

        private int gasPedalPosition;
        private int brakePedalPosition;
        
        private VirtualFunctionBus virtualFunctionBus;

        public AutomatedCar(int x, int y, string filename)
            : base(x, y, filename)
        {
            this.virtualFunctionBus = new VirtualFunctionBus();
            this.ZIndex = 10;
        }

        public VirtualFunctionBus VirtualFunctionBus { get => this.virtualFunctionBus; }

        public int GasPedalPosition
        {
            get
            {
                return this.gasPedalPosition;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref this.gasPedalPosition, value);
            }
        }
        
        public int BrakePedalPosition
        {
            get
            {
                return this.brakePedalPosition;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref this.brakePedalPosition, value);
            }
        }

        public int Revolution { get; set; }
        public Velocity Velocity { get; set; }
        public Geometry Geometry { get; set; }

        /// <summary>Starts the automated cor by starting the ticker in the Virtual Function Bus, that cyclically calls the system components.</summary>
        public void Start()
        {
            this.virtualFunctionBus.Start();
        }

        /// <summary>Stops the automated cor by stopping the ticker in the Virtual Function Bus, that cyclically calls the system components.</summary>
        public void Stop()
        {
            this.virtualFunctionBus.Stop();
        }

        public void IncreaseGasPedalPosition()
        {
            int newPosition = this.gasPedalPosition + pedalOffset;
            this.GasPedalPosition = BoundPedalPosition(newPosition);
        }
        
        public void DecreaseGasPedalPosition()
        {
            int newPosition = this.gasPedalPosition - pedalOffset;
            this.GasPedalPosition = BoundPedalPosition(newPosition);
        }
        
        public void IncreaseBrakePedalPosition()
        {
            int newPosition = this.brakePedalPosition + pedalOffset;
            this.BrakePedalPosition = BoundPedalPosition(newPosition);
        }
        
        public void DecreaseBrakePedalPosition()
        {
            int newPosition = this.brakePedalPosition - pedalOffset;
            this.BrakePedalPosition = BoundPedalPosition(newPosition);
        }

        private int BoundPedalPosition(int number)
        {
            return Math.Max(minPedalPosition, Math.Min(number, maxPedalPosition));
        }
    }
}