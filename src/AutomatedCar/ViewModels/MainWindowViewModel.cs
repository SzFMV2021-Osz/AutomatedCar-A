namespace AutomatedCar.ViewModels
{
    using AutomatedCar.Models;
    using Avalonia.Media;
    using ReactiveUI;

    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase dashboard;
        private ViewModelBase courseDisplay;
        private World world;
        private Game game;

        public World World
        {
            get => this.world;
            private set => this.RaiseAndSetIfChanged(ref this.world, value);
        }

        public MainWindowViewModel(Game game)
        {
            this.game = game;
            this.CourseDisplay = new CourseDisplayViewModel(game.World);
            this.Dashboard = new DashboardViewModel(game.World);
            this.World = game.World;
        }

        public ViewModelBase CourseDisplay
        {
            get => this.courseDisplay;
            private set => this.RaiseAndSetIfChanged(ref this.courseDisplay, value);
        }

        public ViewModelBase Dashboard
        {
            get => this.dashboard;
            private set => this.RaiseAndSetIfChanged(ref this.dashboard, value);
        }
    }
}