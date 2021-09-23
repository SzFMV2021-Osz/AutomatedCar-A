using System;
namespace AutomatedCar.Views
{
    using AutomatedCar.Models;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;

    public class MainWindow : Window
    {
        public MainWindow()
        {

            this.InitializeComponent();
            FocusCar();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Keyboard.Keys.Add(e.Key);
            base.OnKeyDown(e);
            if (Keyboard.IsKeyDown(Key.Up))
            {
                World.Instance.ControlledCar.Y -= 5;
            }

            if (Keyboard.IsKeyDown(Key.Down))
            {
                World.Instance.ControlledCar.Y += 5;
            }

            if (Keyboard.IsKeyDown(Key.Left))
            {
                World.Instance.ControlledCar.X -= 5;
            }

            if (Keyboard.IsKeyDown(Key.Right))
            {
                World.Instance.ControlledCar.X += 5;
            }

            if (Keyboard.IsKeyDown(Key.PageUp))
            {
                World.Instance.ControlledCar.Rotation += 5;
            }

            if (Keyboard.IsKeyDown(Key.PageDown))
            {
                World.Instance.ControlledCar.Rotation -= 5;
            }

            if (Keyboard.IsKeyDown(Key.D1))
            {
                World.Instance.DebugStatus.Enabled = !World.Instance.DebugStatus.Enabled;
            }

            if (Keyboard.IsKeyDown(Key.D2))
            {
                World.Instance.DebugStatus.Camera = !World.Instance.DebugStatus.Camera;
            }

            if (Keyboard.IsKeyDown(Key.D3))
            {
                World.Instance.DebugStatus.Radar = !World.Instance.DebugStatus.Radar;
            }

            if (Keyboard.IsKeyDown(Key.D4))
            {
                World.Instance.DebugStatus.Ultrasonic = !World.Instance.DebugStatus.Ultrasonic;
            }

            if (Keyboard.IsKeyDown(Key.D5))
            {
                World.Instance.DebugStatus.Rotate = !World.Instance.DebugStatus.Rotate;
            }

            if (Keyboard.IsKeyDown(Key.F1))
            {
                new HelpWindow().Show();
                Keyboard.Keys.Remove(Key.F1);
            }
            if (Keyboard.IsKeyDown(Key.F5))
            {
                World.Instance.NextControlledCar();
                Keyboard.Keys.Remove(Key.F5);
            }
            if (Keyboard.IsKeyDown(Key.F6))
            {
                World.Instance.PrevControlledCar();
                Keyboard.Keys.Remove(Key.F5);
            }

            FocusCar();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            Keyboard.Keys.Remove(e.Key);
            base.OnKeyUp(e);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void FocusCar()
        {
            var scrollViewer = this.Get<CourseDisplayView>("courseDisplay").Get<ScrollViewer>("scrollViewer");
            var offsetX = World.Instance.ControlledCar.X - (scrollViewer.Viewport.Width / 2);
            var offsetY = World.Instance.ControlledCar.Y - (scrollViewer.Viewport.Height / 2);
            scrollViewer.Offset = new Avalonia.Vector(offsetX, offsetY);
        }

    }
}