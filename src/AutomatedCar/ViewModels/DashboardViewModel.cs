namespace AutomatedCar.ViewModels
{
    using AutomatedCar.Models;

    public class DashboardViewModel : ViewModelBase
    {
        private World world;
        public DashboardViewModel(World world)
        {
            this.world = world;
        }

        public Models.AutomatedCar ControlledCar
        {
            get => this.world.ControlledCar;
        }
        public World World
        {
            get => this.world;
        }
    }
}