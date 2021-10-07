namespace AutomatedCar.SystemComponents.Sensors
{
    using Avalonia;

    public interface ISensor
    {
        Point RelativeLocation { get; set; }
    }
}