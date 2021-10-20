namespace AutomatedCar.Models
{
    using Avalonia.Media;
    using global::AutomatedCar.SystemComponents;

    public interface IAutomatedCar : IWorldObject
    {
        Geometry Geometry { get; set; }

        int Revolution { get; set; }

        Vector Velocity { get; set; }

        VirtualFunctionBus VirtualFunctionBus { get; }

        void Start();

        void Stop();
    }
}