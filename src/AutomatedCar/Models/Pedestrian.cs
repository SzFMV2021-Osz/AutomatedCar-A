namespace AutomatedCar.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    public class Pedestrian : AbstractNPC
    {
        public Pedestrian(int x, int y, string filename) : base(x, y, filename, WorldObjectType.Pedestrian)
        {
            var pedJsonName = "NPCPedestrian1CoordinatesPathTestWorld.json";
            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream($"AutomatedCar.Assets.{pedJsonName}"));

            this.PathCoordinates = JsonConvert.DeserializeObject<List<Vector>>(reader.ReadToEnd());
            this.Speed = 10;
        }
    }
}
