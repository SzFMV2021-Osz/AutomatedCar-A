namespace AutomatedCar.Models
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Avalonia.Media;
    using global::AutomatedCar.Helpers;
    using global::AutomatedCar.SystemComponents;
    using global::AutomatedCar.SystemComponents.Sensors;
    using ReactiveUI;

    public class AutomatedCar : Car, IAutomatedCar
    {
        private const int PEDAL_OFFSET = 16;
        private const int MIN_PEDAL_POSITION = 0;
        private const int MAX_PEDAL_POSITION = 100;
        private const double PEDAL_INPUT_MULTIPLIER = 0.01;
        private const double DRAG = 0.006; // This limits the top speed to 166 km/h
        private const int IDLE_RPM = 800;
        private const int NEUTRAL_RPM_MULTIPLIER = 100;

        private int gasPedalPosition;
        private int brakePedalPosition;
        private int revolution;
        private int innerGear = 1; //manually set until inner gearbox is implemented

        private VirtualFunctionBus virtualFunctionBus;
        private CollisionDetection collisionDetection;

        public AutomatedCar(int x, int y, string filename)
            : base(x, y, filename)
        {
            this.Velocity = new Vector();
            this.Acceleration = new Vector();
            this.virtualFunctionBus = new VirtualFunctionBus();
            this.collisionDetection = new CollisionDetection(this.virtualFunctionBus);
            this.collisionDetection.OnCollisionWithNpc += this.NpcCollisionEventHandler;
            this.collisionDetection.OnCollisionWithStaticObject += this.ObjectCollisionEventHandler;
            this.Radar = new (this.virtualFunctionBus);
            this.Camera = new (this.virtualFunctionBus);
            this.ZIndex = 10;
            this.Revolution = IDLE_RPM;
            this.ExternalGearbox = new ExternalGearbox(this);
        }

        public VirtualFunctionBus VirtualFunctionBus { get => this.virtualFunctionBus; }

        public Radar Radar { get; private set; }

        public Camera Camera { get; private set; }

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

        public bool InFocus { get; set; }

        public int Revolution
        {
            get
            {
                return this.revolution;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref this.revolution, value);
            }
        }

        public IGearbox ExternalGearbox { get; set; }

        public Vector Velocity { get; set; }

        public Vector Acceleration { get; set; }

        public Geometry Geometry { get; set; }

        /// <summary>Starts the automated car by starting the ticker in the Virtual Function Bus, that cyclically calls the system components.</summary>
        public void Start()
        {
            this.virtualFunctionBus.Start();
        }

        /// <summary>Stops the automated car by stopping the ticker in the Virtual Function Bus, that cyclically calls the system components.</summary>
        public void Stop()
        {
            this.virtualFunctionBus.Stop();
        }

        public void NpcCollisionEventHandler(object o, EventArgs args)
        {
            Debug.WriteLine($"Collision with an NPC!");
        }

        public void ObjectCollisionEventHandler(object o, EventArgs args)
        {
            Debug.WriteLine($"Collision with a static object!");
        }

        public void SetSensors()
        {
            this.Radar.RelativeLocation = new Avalonia.Point(this.Geometry.Bounds.TopRight.X / 2, this.Geometry.Bounds.TopRight.Y);
            this.Camera.RelativeLocation = new Avalonia.Point(this.Geometry.Bounds.Center.X, this.Geometry.Bounds.Center.Y / 2);
        }

        public void CalculateSpeed()
        {
            this.Speed = (int)Math.Sqrt(Math.Pow(this.Velocity.X, 2) + Math.Pow(this.Velocity.Y, 2));
        }

        public void CalculateNextPosition()
        {
            double gasInputForce = this.gasPedalPosition * PEDAL_INPUT_MULTIPLIER;
            double brakeInputForce = this.brakePedalPosition * PEDAL_INPUT_MULTIPLIER;
            double slowingForce = this.Speed * DRAG + (this.Speed > 0 ? brakeInputForce : 0);

            Acceleration.Y = gasInputForce;

            Velocity.Y = GetVelocityAccordingToGear(slowingForce);

            Y += (int)Velocity.Y;
            CalculateSpeed();
            CalculateRevolutions();
        }

        private double GetVelocityAccordingToGear(double slowingForce)
        {
            double velocity = Velocity.Y;

            if (ExternalGearbox.currentGearPosition == Models.ExternalGearbox.Gear.D)
            {
                velocity += -(Acceleration.Y - slowingForce);
            }
            else if (ExternalGearbox.currentGearPosition == Models.ExternalGearbox.Gear.R)
            {
                velocity += Acceleration.Y - slowingForce;
            }
            else
            {
                if (velocity < 0) //In neutral gear, the car can stop whether it goes forward or backward
                {
                    velocity += slowingForce;
                }
                else
                {
                    velocity -= slowingForce;
                }
            }

            return velocity;
        }

        public void IncreaseGasPedalPosition()
        {
            int newPosition = this.gasPedalPosition + PEDAL_OFFSET;
            this.GasPedalPosition = this.BoundPedalPosition(newPosition);
        }

        public void DecreaseGasPedalPosition()
        {
            int newPosition = this.gasPedalPosition - PEDAL_OFFSET;
            this.GasPedalPosition = this.BoundPedalPosition(newPosition);
        }

        public void IncreaseBrakePedalPosition()
        {
            int newPosition = this.brakePedalPosition + PEDAL_OFFSET;
            this.BrakePedalPosition = this.BoundPedalPosition(newPosition);
        }

        public void DecreaseBrakePedalPosition()
        {
            int newPosition = this.brakePedalPosition - PEDAL_OFFSET;
            this.BrakePedalPosition = this.BoundPedalPosition(newPosition);
        }

        private void CalculateRevolutions()
        {
            if (this.gasPedalPosition > 0)
            {
                this.IncreaseRevolutions();
            }
            else
            {
                this.DecreaseRevolutions();
            }
        }

        private void IncreaseRevolutions()
        {
            double revolutionsIncreaseRate =
                RevolutionsHelper.GearCoefficients.FirstOrDefault(x => x.Item1 == this.innerGear).Item2;
            if (this.innerGear > 0)
            {
                this.Revolution += (int)Math.Round(this.Speed * revolutionsIncreaseRate);
            }
            else
            {
                this.Revolution += (int)Math.Round(revolutionsIncreaseRate * NEUTRAL_RPM_MULTIPLIER);
            }
        }

        private void DecreaseRevolutions()
        {
            double revolutionsDecreaseRate =
                RevolutionsHelper.GearCoefficients.FirstOrDefault(x => x.Item1 == this.innerGear).Item2 / 5;
            var revolutionChange = this.brakePedalPosition > 0
                ? this.brakePedalPosition * revolutionsDecreaseRate
                : Math.Pow(Math.Log(this.Speed + 1) / 20, -1.38) * revolutionsDecreaseRate;
            int newRPM = this.revolution - (int)Math.Round(revolutionChange);
            this.Revolution = Math.Max(newRPM, IDLE_RPM);
        }

        private int BoundPedalPosition(int number)
        {
            return Math.Max(MIN_PEDAL_POSITION, Math.Min(number, MAX_PEDAL_POSITION));
        }
    }
}