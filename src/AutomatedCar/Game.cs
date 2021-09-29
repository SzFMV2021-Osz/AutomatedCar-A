namespace AutomatedCar
{
    using System;
    using AutomatedCar.Models;
    using Avalonia.Input;

    public class Game : GameBase
    {
        private readonly World world;

        public Game(World world)
        {
            this.world = world;
        }

        public World World { get => this.world; }

        private Random Random { get; } = new Random();

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
            
            World.Instance.ControlledCar.CalculateNextPosition();
        }
    }
}