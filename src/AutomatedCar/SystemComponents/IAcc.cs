namespace AutomatedCar.SystemComponents
{
    public interface IAcc
    {
        public bool IsAccOn { get; set; }
        public int AccSpeed { get; set; }
        public double AccDistance { get; set; }
        public void ToggleAcc();
        public void SwitchDistance();
        public void IncreaseSpeed();
        public void DecreaseSpeed();
    }
}
