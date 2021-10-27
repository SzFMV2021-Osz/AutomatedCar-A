namespace AutomatedCar.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using Avalonia.Media;
    using ReactiveUI;

    public class WorldObject : ReactiveObject
    {
        private int id;
        private int x;
        private int y;

        private double rotation;

        public WorldObject(int x, int y, string filename, int zindex = 1, bool collideable = false, WorldObjectType worldObjectType = WorldObjectType.Other)
        {
            this.X = x;
            this.Y = y;
            this.Filename = filename;
            this.ZIndex = zindex;
            this.Collideable = collideable;
            this.WorldObjectType = worldObjectType;
            this.Id = World.Instance.GetNextId();
        }

        public int ZIndex { get; set; }

        public double Rotation
        {
            get => this.rotation;
            set => this.RaiseAndSetIfChanged(ref this.rotation, value % 360);
        }

        public int X
        {
            get => Convert.ToInt32(this.x);
            set => this.RaiseAndSetIfChanged(ref this.x, Convert.ToDouble(value));
        }

        public int Id { get; private set; }

        public int Y
        {
            get => Convert.ToInt32(this.y);
            set => this.RaiseAndSetIfChanged(ref this.y, Convert.ToDouble(value));
        }

        public double PreciseX { get => this.x; set => this.RaiseAndSetIfChanged(ref this.x, value); }
        public double PreciseY { get => this.y; set => this.RaiseAndSetIfChanged(ref this.y, value); }

        public Point RotationPoint { get; set; }

        public string RenderTransformOrigin { get; set; }

        public ObservableCollection<PolylineGeometry> Geometries { get; set; } = new();

        public ObservableCollection<PolylineGeometry> RawGeometries { get; set; } = new();

        public string Filename { get; set; }

        public bool Collideable { get; set; }

        public WorldObjectType WorldObjectType { get; set; }
    }
}