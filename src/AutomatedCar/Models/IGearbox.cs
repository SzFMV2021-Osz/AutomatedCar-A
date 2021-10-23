namespace AutomatedCar.Models
{
    public interface IGearbox
    {
        ExternalGearbox.Gear currentGearPosition { get; set; }

        void Downshift();

        void Upshift();
    }
}