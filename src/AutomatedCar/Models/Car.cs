namespace AutomatedCar.Models
{
    using ReactiveUI;

    public class Car : WorldObject
    {

        private int speed;
        
        public Car(int x, int y, string filename)
            : base(x, y, filename)
        {
        }

        /// <summary>Gets or sets Speed in px/s.</summary>
        public int Speed
        {
            get => speed;
            set => this.RaiseAndSetIfChanged(ref this.speed, value);
        }
    }
}