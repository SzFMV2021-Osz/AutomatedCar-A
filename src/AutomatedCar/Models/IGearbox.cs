namespace AutomatedCar.Models
{
    public interface IGearbox
    {
        Gear CurrentExternalGearPosition { get; set; }

        int CurrentInternalGear { get; set; }

        Shifting InnerShiftingStatus { get; set; }

        void ExternalDownshift();

        void ExternalUpshift();

        void InternalDownshift();

        void InternalUpshift();
    }
}