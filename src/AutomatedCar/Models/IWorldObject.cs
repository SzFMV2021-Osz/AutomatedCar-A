namespace AutomatedCar.Models
{
    using System.Collections.ObjectModel;
    using System.Drawing;
    using Avalonia.Media;

    public interface IWorldObject
    {
        bool Collideable { get; set; }

        string Filename { get; set; }

        ObservableCollection<PolylineGeometry> Geometries { get; set; }

        ObservableCollection<PolylineGeometry> RawGeometries { get; set; }

        string RenderTransformOrigin { get; set; }

        double Rotation { get; set; }

        Point RotationPoint { get; set; }

        WorldObjectType WorldObjectType { get; set; }

        int X { get; protected set; }

        int Y { get; protected set; }

        double PreciseX { get; set; }

        double PreciseY { get; set; }

        int ZIndex { get; set; }
    }
}