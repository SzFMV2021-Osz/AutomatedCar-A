namespace AutomatedCar.Models
{
    using Avalonia.Media;
    using ReactiveUI;
    using System;
    using SystemComponents;

    public class AutomatedCar : Car
    {
        private const int PEDAL_OFFSET = 16;
        private const int MIN_PEDAL_POSITION = 0;
        private const int MAX_PEDAL_POSITION = 100;
        private const double PEDAL_INPUT_MULTIPLIER = 0.01;
        private const double DRAG = 0.006; // This limits the top speed to 166 km/h

        private int gasPedalPosition;
        private int brakePedalPosition;
        
        private VirtualFunctionBus virtualFunctionBus;

        public AutomatedCar(int x, int y, string filename)
            : base(x, y, filename)
        {
            this.Velocity = new Vector();
            this.Acceleration = new Vector();
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
        public Vector Velocity { get; set; }
        public Vector Acceleration { get; set; }
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

        public void CalculateSpeed()
        {
            Speed = (int) Math.Sqrt(Math.Pow(Velocity.X, 2) + Math.Pow(Velocity.Y, 2));
        }
        
        public void CalculateNextPosition()
        {
            CalculateSpeed();
            double gasInputForce = this.gasPedalPosition * PEDAL_INPUT_MULTIPLIER;
            double brakeInputForce = this.brakePedalPosition * PEDAL_INPUT_MULTIPLIER;
            double slowingForce = Speed * DRAG + (Speed > 0 ? brakeInputForce : 0);
            
            Acceleration.Y = gasInputForce;
            Velocity.Y += -(Acceleration.Y - slowingForce);
            Y += (int)Velocity.Y;
        }
        
        public void IncreaseGasPedalPosition()
        {
            int newPosition = this.gasPedalPosition + PEDAL_OFFSET;
            this.GasPedalPosition = BoundPedalPosition(newPosition);
        }
        
        public void DecreaseGasPedalPosition()
        {
            int newPosition = this.gasPedalPosition - PEDAL_OFFSET;
            this.GasPedalPosition = BoundPedalPosition(newPosition);
        }
        
        public void IncreaseBrakePedalPosition()
        {
            int newPosition = this.brakePedalPosition + PEDAL_OFFSET;
            this.BrakePedalPosition = BoundPedalPosition(newPosition);
        }
        
        public void DecreaseBrakePedalPosition()
        {
            int newPosition = this.brakePedalPosition - PEDAL_OFFSET;
            this.BrakePedalPosition = BoundPedalPosition(newPosition);
        }

        private int BoundPedalPosition(int number)
        {
            return Math.Max(MIN_PEDAL_POSITION, Math.Min(number, MAX_PEDAL_POSITION));
        }
    }
}