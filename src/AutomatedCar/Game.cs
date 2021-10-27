namespace AutomatedCar
{
    using System;
    using AutomatedCar.Models;
    using Avalonia.Input;
    
    public delegate void CarFocusHandler();
    
    public class Game : GameBase
    {
        private readonly World world;
        private CarFocusHandler carFocusHandler;
        public Game(World world)
        {
            this.world = world;
        }

        public World World { get => this.world; }

        private Random Random { get; } = new Random();

        public void setCarFocusHandler(CarFocusHandler carFocusHandler)
        {
            this.carFocusHandler = carFocusHandler;
        }

        protected override void Tick()
        {
            if (!Keyboard.IsKeyDown(Key.Up))
            {
                World.Instance.ControlledCar.DecreaseGasPedalPosition();
            }

            if (!Keyboard.IsKeyDown(Key.Down))
            {
                World.Instance.ControlledCar.DecreaseBrakePedalPosition();
            }

            // Move forward all the NPCs.
            //foreach (var item in World.Instance.GetAllNPCs())
            //{
            //    item.StepObject();
            //}
            World.Instance.StepNonPlayerCharacters();

            World.Instance.ControlledCar.CalculateNextPosition();
            this.carFocusHandler.Invoke();
        }
    }
}