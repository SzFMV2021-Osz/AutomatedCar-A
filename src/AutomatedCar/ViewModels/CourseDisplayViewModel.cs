namespace AutomatedCar.ViewModels
{
    using AutomatedCar.Models;

    public class CourseDisplayViewModel : ViewModelBase
    {
        public CourseDisplayViewModel(World world)
        {
            this.World = world;
        }

        public World World { get; private set; }
    }
}