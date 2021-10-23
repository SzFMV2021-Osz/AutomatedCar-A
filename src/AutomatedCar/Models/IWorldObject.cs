namespace AutomatedCar.Models
{
    using Avalonia.Media;
    using System.Collections.ObjectModel;
    using System.Drawing;

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

        int X { get; set; }

        int Y { get; set; }

        int ZIndex { get; set; }
    }
}