namespace AutomatedCar.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Avalonia;
    using Avalonia.Media;
    using global::AutomatedCar.SystemComponents.Sensors;
    using ReactiveUI;
    using SystemComponents;

    public class AutomatedCar : Car, IAutomatedCar
    {
        private const int PEDAL_OFFSET = 16;
        private const int MIN_PEDAL_POSITION = 0;
        private const int MAX_PEDAL_POSITION = 100;
        private const double PEDAL_INPUT_MULTIPLIER = 0.01;
        private const double DRAG = 0.006; // This limits the top speed to 166 km/h

        private int gasPedalPosition;
        private int brakePedalPosition;

        private VirtualFunctionBus virtualFunctionBus;
        private ICollection<ISensor> sensors;
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
            this.sensors = new List<ISensor>();
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

        public bool InFocus { get; set; }

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
            Radar radar = new ();
            radar.RelativeLocation = new Avalonia.Point(this.Geometry.Bounds.TopRight.X / 2, this.Geometry.Bounds.TopRight.Y);
            this.sensors.Add(radar);

            Camera camera = new ();
            camera.RelativeLocation = new Avalonia.Point(this.Geometry.Bounds.Center.X, this.Geometry.Bounds.Center.Y / 2);
            this.sensors.Add(camera);
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

            this.Acceleration.Y = gasInputForce;
            this.Velocity.Y += -(this.Acceleration.Y - slowingForce);
            this.Y += (int)this.Velocity.Y;
            this.CalculateSpeed();
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

        private int BoundPedalPosition(int number)
        {
            return Math.Max(MIN_PEDAL_POSITION, Math.Min(number, MAX_PEDAL_POSITION));
        }
    }
}