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
            if (Keyboard.IsKeyDown(Key.Up))
            {
                this.world.ControlledCar.Y -= 5;
            }

            if (Keyboard.IsKeyDown(Key.Down))
            {
                this.world.ControlledCar.Y += 5;
            }

            if (Keyboard.IsKeyDown(Key.Left))
            {
                this.world.ControlledCar.X -= 5;
            }

            if (Keyboard.IsKeyDown(Key.Right))
            {
                this.world.ControlledCar.X += 5;
            }

            if (Keyboard.IsKeyDown(Key.PageUp))
            {
                this.world.ControlledCar.Rotation += 5;
            }

            if (Keyboard.IsKeyDown(Key.PageDown))
            {
                this.world.ControlledCar.Rotation -= 5;
            }

            if (Keyboard.IsKeyDown(Key.D1))
            {
                this.world.DebugStatus.Enabled = !this.world.DebugStatus.Enabled;
            }

            if (Keyboard.IsKeyDown(Key.D2))
            {
                this.world.DebugStatus.Camera = !this.world.DebugStatus.Camera;
            }

            if (Keyboard.IsKeyDown(Key.D3))
            {
                this.world.DebugStatus.Radar = !this.world.DebugStatus.Radar;
            }

            if (Keyboard.IsKeyDown(Key.D4))
            {
                this.world.DebugStatus.Ultrasonic = !this.world.DebugStatus.Ultrasonic;
            }

            if (Keyboard.IsKeyDown(Key.D5))
            {
                this.world.DebugStatus.Rotate = !this.world.DebugStatus.Rotate;
            }
        }
    }
}