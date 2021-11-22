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
    using global::AutomatedCar.SystemComponents.Helpers;
    using System.Drawing;

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
        private const int WHEELBASE = 200;
        private const int TURNING_OFFSET = 5;
        private const double TURNING_MULTIPLIER = 0.1;

        private int gasPedalPosition;
        private int brakePedalPosition;
        private int revolution;
        private double carHeading;
        private double turningAngle;

        private VirtualFunctionBus virtualFunctionBus;
        private CollisionDetection collisionDetection;
        private LaneKeeping laneKeeping;

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
            this.Gearbox = new Gearbox(this);
            this.LaneKeepingMod = new LaneKeepingModel(this);
            carHeading = -1.5;
            turningAngle = 0;
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

        public LaneKeepingModel LaneKeepingMod { get; set; }

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
            this.laneKeeping = new LaneKeeping(this.virtualFunctionBus);
        }

        public void LaneKeeping()
        {
            //Set status of Lanekeeping
            if (this.virtualFunctionBus.LaneKeepingPacket.LaneKeepingStatus == LaneKeepingStatus.Active)
            {
                this.virtualFunctionBus.LaneKeepingPacket.LaneKeepingStatus = LaneKeepingStatus.Inactive;
            }
            else
            {
                this.virtualFunctionBus.LaneKeepingPacket.LaneKeepingStatus = LaneKeepingStatus.Active;
            }
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
            double frontX = X + (WHEELBASE / 2 * Math.Cos(carHeading));
            double frontY = Y + (WHEELBASE / 2 * Math.Sin(carHeading));
            double rearX = X - (WHEELBASE / 2 * Math.Cos(carHeading));
            double rearY = Y - (WHEELBASE / 2 * Math.Sin(carHeading));

            double reverseMultiplier = Gearbox.CurrentExternalGearPosition == Gear.R ? -3 : 3;
            
            frontX += Speed * TURNING_MULTIPLIER * Math.Cos(carHeading + steerAngle) * reverseMultiplier;
            frontY += Speed * TURNING_MULTIPLIER * Math.Sin(carHeading + steerAngle) * reverseMultiplier;
            rearX += Speed * TURNING_MULTIPLIER * Math.Cos(carHeading) * reverseMultiplier;
            rearY += Speed * TURNING_MULTIPLIER * Math.Sin(carHeading) * reverseMultiplier;
            
            X = (int)(frontX + rearX) / 2;
            Y = (int)(frontY + rearY) / 2;
            
            carHeading = Math.Atan2(frontY - rearY, frontX - rearX);
            Rotation = ((carHeading * 180) / Math.PI) + 87;
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