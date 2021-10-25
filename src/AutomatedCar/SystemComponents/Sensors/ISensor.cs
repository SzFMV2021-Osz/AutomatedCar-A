namespace AutomatedCar.SystemComponents.Sensors
{
    using AutomatedCar.Models;
    using Avalonia;

    public interface ISensor
    {
        Point RelativeLocation { get; set; }
    }
}