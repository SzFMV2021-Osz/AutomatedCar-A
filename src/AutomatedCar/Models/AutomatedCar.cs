namespace AutomatedCar.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Avalonia.Media;
    using global::AutomatedCar.Helpers;
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
        private const int IDLE_RPM = 800;
        private const int NEUTRAL_RPM_MULTIPLIER = 100;
        private const int PIXEL_METER_RATIO = 50;

        private const int CAR_MASS = 1500;
        private const double TRANSMISSION_EFFICIENCY = 0.7;
        private const double WHEEL_RADIUS = 0.34;
        private const double DIFFERENTIAL_RATIO = 3.42;

        private int gasPedalPosition;
        private int brakePedalPosition;
        private int revolution;
        private int innerGear = 1;      //manually set until inner gearbox is implemented
        private int engineForce;
        private int currentTorque;

        private VirtualFunctionBus virtualFunctionBus;
        private ICollection<ISensor> sensors;

        private Dictionary<int, int> TorqueLookupTable = new Dictionary<int, int>()
        {
            { 1000, 50 },
            { 2000, 100 },
            { 3000, 150 },
            { 4000, 200 },
            { 5000, 250},
        };

        private Dictionary<int, double> GearRatioLookupTable = new Dictionary<int, double>()
        {
            { 1, 2.66 },
            { 2, 1.78 },
            { 3, 1.30 },
            { 4, 1.00 },
        };

        public AutomatedCar(int x, int y, string filename)
            : base(x, y, filename)
        {
            this.Velocity = new Vector();
            this.Acceleration = new Vector();
            this.virtualFunctionBus = new VirtualFunctionBus();
            this.sensors = new List<ISensor>();
            this.ZIndex = 10;
            this.Revolution = IDLE_RPM;
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
            double driveForce = GenerateEngineForce();
            double brakeInputForce = this.brakePedalPosition * PEDAL_INPUT_MULTIPLIER;
            double slowingForce = this.Speed * DRAG + (this.Speed > 0 ? brakeInputForce : 0);

            this.Acceleration.Y = driveForce;
            this.Velocity.Y += -(this.Acceleration.Y - slowingForce);
            this.Y += (int)this.Velocity.Y;
            this.CalculateSpeed();
        }

        public double GenerateEngineForce()
        {
            double maxTorqueAtRPM = this.LookupTorqueCurve(this.Revolution);
            double currentTorque = (this.gasPedalPosition * maxTorqueAtRPM) / 100; // The

            double driveForce = (DIFFERENTIAL_RATIO * TRANSMISSION_EFFICIENCY *
                this.GearRatioLookupTable[this.innerGear] * currentTorque) / WHEEL_RADIUS;
            driveForce /= CAR_MASS;

            return driveForce;
        }

        public double LookupTorqueCurve(double rpm)
        {
            int rounded_rpm = TorqueLookupTable.Keys.ToList().OrderBy(x => Math.Abs(rpm - x)).First();
            return TorqueLookupTable[rounded_rpm];
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

        public void CalculateRevolutions()
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
            double revolutionsIncreaseRate = RevolutionsHelper.GearCoefficients.FirstOrDefault(x => x.Item1 == this.innerGear).Item2;
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
            double revolutionsDecreaseRate = RevolutionsHelper.GearCoefficients.FirstOrDefault(x => x.Item1 == this.innerGear).Item2 / 5;
            var revolutionChange = this.brakePedalPosition > 0 ? this.brakePedalPosition * revolutionsDecreaseRate : Math.Pow(Math.Log(this.Speed + 1) / 20, -1.38) * revolutionsDecreaseRate;
            int newRPM = this.revolution - (int)Math.Round(revolutionChange);
            this.Revolution = Math.Max(newRPM, IDLE_RPM);
        }

        private int BoundPedalPosition(int number)
        {
            return Math.Max(MIN_PEDAL_POSITION, Math.Min(number, MAX_PEDAL_POSITION));
        }

        private int ConvertMetresToPixels(double metres)
        {
            return (int)(metres * PIXEL_METER_RATIO);
        }

        private int ConvertPixelsToMeter(double pixels)
        {
            return (int)(pixels / PIXEL_METER_RATIO);
        }
    }
}