namespace AutomatedCar.Models
{
    public interface IGearbox
    {
        Gear currentGearPosition { get; set; }

        void Downshift();

        void Upshift();
    }
}