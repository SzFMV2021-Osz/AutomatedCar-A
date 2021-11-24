namespace AutomatedCar.SystemComponents.Packets
{
    using ReactiveUI;

    public class AutomaticEmergencyBrakePacket : ReactiveObject, IAutomaticEmergencyBrakePacket
    {
        private bool needEmergencyBrakeWarning;
        private bool mightNotWorkProperlyWarning;
        private double decelerationRate;

        /// <summary>
        /// Gets or Sets the value indicating whether a visual warning is required to the driver when the collison is avoidable.
        /// </summary>
        public bool NeedEmergencyBrakeWarning 
        {
            get
            {
                return this.needEmergencyBrakeWarning;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.needEmergencyBrakeWarning, value);
            }
        }

        /// <summary>
        /// Gets or Sets the value indicating whether the AEB cannot handle all situations above 70 km/h.
        /// </summary>
        public bool MightNotWorkProperlyWarning
        {
            get
            {
                return this.mightNotWorkProperlyWarning;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.mightNotWorkProperlyWarning, value);
            }
        }

        /// <summary>
        /// Gets or Sets the value indicating whether the AEB cannot handle all situations above 70 km/h.
        /// </summary>
        public double DecelerationRate
        {
            get
            {
                return this.decelerationRate;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.decelerationRate, value);
            }
        }
    }
}
