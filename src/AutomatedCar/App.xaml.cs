namespace AutomatedCar
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Reflection;
    using AutomatedCar.Models;
    using AutomatedCar.ViewModels;
    using AutomatedCar.Views;
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media;
    using Newtonsoft.Json.Linq;

    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream($"AutomatedCar.Assets.worldobject_polygons.json"));
                string json_text = reader.ReadToEnd();
                dynamic stuff = JObject.Parse(json_text);
                var points = new List<Point>();
                foreach (var i in stuff["objects"][0]["polys"][0]["points"])
                {
                    points.Add(new Point(i[0].ToObject<int>(), i[1].ToObject<int>()));
                }

                var geom = new PolylineGeometry(points, false);

                var world = World.Instance;

                world.PopulateFromJSON($"AutomatedCar.Assets.test_world.json");

                var controlledCar = new Models.AutomatedCar(480, 1425, "car_1_white.png");
                controlledCar.Geometry = geom;
                controlledCar.RotationPoint = new System.Drawing.Point(54, 120);
                controlledCar.Geometries = new ObservableCollection<PolylineGeometry>();
                controlledCar.Geometries.Add(new PolylineGeometry(new List<Point> { new Point(36, 240), new Point(36, 180) }, false));
                controlledCar.Geometries.Add(new PolylineGeometry(new List<Point> { new Point(72, 240), new Point(72, 180) }, false));
                world.AddControlledCar(controlledCar);
                controlledCar.Start();

                var controlledCar2 = new Models.AutomatedCar(4250, 1420, "car_1_red.png");
                controlledCar2.Geometry = geom;
                controlledCar2.RotationPoint = new System.Drawing.Point(54, 120);
                controlledCar2.Geometries = new ObservableCollection<PolylineGeometry>();
                controlledCar2.Geometries.Add(new PolylineGeometry(new List<Point> { new Point(36, 240), new Point(36, 180) }, false));
                controlledCar2.Geometries.Add(new PolylineGeometry(new List<Point> { new Point(72, 240), new Point(72, 180) }, false));
                controlledCar2.Rotation = -90;
                world.AddControlledCar(controlledCar2);
                controlledCar2.Start();

                desktop.MainWindow = new MainWindow { DataContext = new MainWindowViewModel(world) };
                ((MainWindow)desktop.MainWindow).FocusCar();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}