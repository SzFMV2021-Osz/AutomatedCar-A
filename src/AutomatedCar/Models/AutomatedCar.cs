namespace AutomatedCar.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Avalonia.Media;
    using global::AutomatedCar.Helpers;
    using global::AutomatedCar.SystemComponents;
    using global::AutomatedCar.SystemComponents.Helpers;
    using global::AutomatedCar.SystemComponents.Sensors;
    using ReactiveUI;

    public class AutomatedCar : Car
    {
        private const int PEDAL_OFFSET = 16;
        private const int MIN_PEDAL_POSITION = 0;
        private const int MAX_PEDAL_POSITION = 100;
        private const double PEDAL_INPUT_MULTIPLIER = 0.01;
        private const double DRAG = 0.01;
        private const int IDLE_RPM = 800;
        private const int MAX_RPM = 6000;
        private const int NEUTRAL_RPM_MULTIPLIER = 80;
        private const int RPM_DOWNSHIFT_POINT = 1300;
        private const int RPM_UPSHIFT_POINT = 2500;
        private const int WHEELBASE = 300;
        private const int TURNING_OFFSET = 5;
        private const double TURNING_MULTIPLIER = 0.3;

        private static Dictionary<int, double> scalingValueLookupTable = new Dictionary<int, double>()
            {
                { 20, 1.0 },
                { 30, 0.9 },
                { 40, 0.8 },
                { 50, 0.7 },
                { 60, 0.6 },
                { 75, 0.5 },
            };

        private int gasPedalPosition;
        private int brakePedalPosition;
        private int revolution;
        private double carHeading;
        private double turningAngle;

        private VirtualFunctionBus virtualFunctionBus;
        private CollisionDetection collisionDetection;
        private LaneKeepingAssistant laneKeepingAssistant;
        private AutomaticEmergencyBrake automaticEmergencyBrake;

        public AutomatedCar(int x, int y, string filename)
            : base(x, y, filename)
        {
            this.Velocity = new Vector();
            this.Acceleration = new Vector();
            this.virtualFunctionBus = new VirtualFunctionBus();
            this.automaticEmergencyBrake = new AutomaticEmergencyBrake(this.virtualFunctionBus);
            this.collisionDetection = new CollisionDetection(this.virtualFunctionBus);
            this.collisionDetection.OnCollisionWithNpc += this.NpcCollisionEventHandler;
            this.collisionDetection.OnCollisionWithStaticObject += this.ObjectCollisionEventHandler;
            this.Radar = new(this.virtualFunctionBus);
            this.Camera = new(this.virtualFunctionBus);
            this.ZIndex = 10;
            this.Revolution = IDLE_RPM;
            this.Gearbox = new Gearbox(this);
            this.LKAModel = new LKAModel(this);
            this.carHeading = -1.5;
            this.turningAngle = 0;
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

        public IGearbox Gearbox { get; set; }

        public LKAModel LKAModel { get; set; }

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

        public void SetLaneKeepingAssistant()
        {
            this.laneKeepingAssistant = new LaneKeepingAssistant(this.virtualFunctionBus);
        }

        public void CalculateSpeed()
        {
            this.Speed = (int)Math.Sqrt(Math.Pow(this.Velocity.X, 2) + Math.Pow(this.Velocity.Y, 2));
        }

        public void CalculateNextPosition()
        {
            var AEB = this.virtualFunctionBus.AutomaticEmergencyBrakePacket;
            if (AEB.NeedEmergencyBrakeWarning && this.Gearbox.CurrentExternalGearPosition == Gear.D)
            {
                this.gasPedalPosition = MIN_PEDAL_POSITION;
                this.brakePedalPosition = this.BoundPedalPosition((int)Math.Round((AEB.DecelerationRate / 4) * MAX_PEDAL_POSITION, 0));
            }

            double gasInputForce = this.gasPedalPosition * PEDAL_INPUT_MULTIPLIER;
            double brakeInputForce = this.brakePedalPosition * PEDAL_INPUT_MULTIPLIER;
            double slowingForce = this.Speed * DRAG + (this.Speed > 0 ? brakeInputForce : 0);

            Acceleration.Y = gasInputForce;

            Velocity.Y = GetVelocityAccordingToGear(slowingForce);

            CalculateSpeed();
            Turn(turningAngle);
            CalculateRevolutions();
            if (Gearbox.InnerShiftingStatus != Shifting.None)
            {
                this.HandleRpmTransitionWhenShifting();
            }
        }

        public void StraightenWheel()
        {
            if (turningAngle < 0)
            {
                turningAngle += TURNING_OFFSET;
            }
            else if (turningAngle > 0)
            {
                turningAngle -= TURNING_OFFSET;
            }
        }

        public void TurnLeft()
        {
            turningAngle = TURNING_OFFSET;
        }

        public void TurnRight()
        {
            turningAngle = -TURNING_OFFSET;
        }

        private void Turn(double steerAngle)
        {
            double frontWheelX = X + (WHEELBASE / 2 * Math.Cos(carHeading));
            double frontWheelY = Y + (WHEELBASE / 2 * Math.Sin(carHeading));
            double rearWheelX = X - (WHEELBASE / 2 * Math.Cos(carHeading));
            double rearWheelY = Y - (WHEELBASE / 2 * Math.Sin(carHeading));

            double reverseMultiplier = Gearbox.CurrentExternalGearPosition == Gear.R ? -1 : 1;

            double scaling = GetScaleDownValue(Speed);

            frontWheelX += Speed * scaling * TURNING_MULTIPLIER * Math.Cos(carHeading + steerAngle) * reverseMultiplier;
            frontWheelY += Speed * scaling * TURNING_MULTIPLIER * Math.Sin(carHeading + steerAngle) * reverseMultiplier;
            rearWheelX += Speed * scaling * TURNING_MULTIPLIER * Math.Cos(carHeading) * reverseMultiplier;
            rearWheelY += Speed * scaling * TURNING_MULTIPLIER * Math.Sin(carHeading) * reverseMultiplier;

            X = (int)(frontWheelX + rearWheelX) / 2;
            Y = (int)(frontWheelY + rearWheelY) / 2;

            carHeading = Math.Atan2(frontWheelY - rearWheelY, frontWheelX - rearWheelX);

            Rotation = ((carHeading * 180) / Math.PI) + 87;
        }

        private double GetScaleDownValue(int speed)
        {
            int rounded_speed = scalingValueLookupTable.Keys.ToList().OrderBy(x => Math.Abs(speed - x)).First();
            return scalingValueLookupTable[rounded_speed];
        }

        private double GetVelocityAccordingToGear(double slowingForce)
        {
            double velocity = Velocity.Y;

            if (Gearbox.CurrentExternalGearPosition == Gear.D)
            {
                velocity += -(Acceleration.Y - slowingForce);
            }
            else if (Gearbox.CurrentExternalGearPosition == Gear.R && Speed < 20) // Should not reverse with unlimited speed
            {
                velocity += Acceleration.Y - slowingForce;
            }
            else
            {
                if (velocity < 0) // In neutral gear, the car can stop whether it goes forward or backward
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
                RevolutionsHelper.GearCoefficients.FirstOrDefault(x => x.Item1 == this.Gearbox.CurrentInternalGear).Item2;

            if (this.Gearbox.CurrentInternalGear > 0 && this.Revolution < MAX_RPM)
            {
                this.Revolution += (int)Math.Round(this.Speed * revolutionsIncreaseRate);
            }
            else if (this.Gearbox.CurrentInternalGear == 0 && this.Revolution < MAX_RPM)
            {
                this.Revolution += (int)Math.Round(revolutionsIncreaseRate * NEUTRAL_RPM_MULTIPLIER);
            }

            if (this.revolution > RPM_UPSHIFT_POINT && this.Gearbox.CurrentInternalGear != 0)
            {
                this.Gearbox.InternalUpshift();
            }
        }

        private void DecreaseRevolutions()
        {
            double revolutionsDecreaseRate =
               0.15 / RevolutionsHelper.GearCoefficients.FirstOrDefault(x => x.Item1 == this.Gearbox.CurrentInternalGear).Item2;
            var revolutionChange = this.brakePedalPosition > 0
                ? this.brakePedalPosition * revolutionsDecreaseRate
                : Math.Pow(Math.Log(this.Speed + 1) / 20, -1.38) * revolutionsDecreaseRate;
            int newRPM = this.revolution - (int)Math.Round(revolutionChange);
            this.Revolution = Math.Max(newRPM, IDLE_RPM);

            if (this.revolution < RPM_DOWNSHIFT_POINT && Gearbox.CurrentInternalGear > 1)
            {
                Gearbox.InternalDownshift();
            }
        }

        private void HandleRpmTransitionWhenShifting()
        {
            if (Gearbox.InnerShiftingStatus == Shifting.Up)
            {
                this.revolution -= 100;
                if (this.revolution < 1400)
                {
                    Gearbox.InnerShiftingStatus = Shifting.None;
                }
            }

            if (Gearbox.InnerShiftingStatus == Shifting.Down)
            {
                this.revolution += 100;
                if (this.revolution > 2000)
                {
                    Gearbox.InnerShiftingStatus = Shifting.None;
                }
            }
        }

        private int BoundPedalPosition(int number)
        {
            return Math.Max(MIN_PEDAL_POSITION, Math.Min(number, MAX_PEDAL_POSITION));
        }
    }
}