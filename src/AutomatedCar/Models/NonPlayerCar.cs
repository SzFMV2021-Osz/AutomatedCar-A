namespace AutomatedCar.Models
{
    using global::AutomatedCar.SystemComponents;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    public class NonPlayerCar : AbstractNPC
    {
        public NonPlayerCar(int x, int y, string filename) : base(x, y, filename, WorldObjectType.Car)
        {

        }
        public NonPlayerCar(int x, int y, string filename, string nonPlayerCarJsonName) : base(x, y, filename, WorldObjectType.Car)
        {
            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream($"AutomatedCar.Assets.{nonPlayerCarJsonName}"));

            this.PathCoordinates = JsonConvert.DeserializeObject<List<Vector>>(reader.ReadToEnd());
            this.Speed = 20;
        }
    }
}
