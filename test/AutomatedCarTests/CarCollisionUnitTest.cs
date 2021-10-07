namespace Tests
{
    using AutomatedCar;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents;
    using Avalonia.Media;
    using System.Linq;
    using Xunit;

    public class CarCollisionUnitTest
    {

        private AutomatedCar automatedCar;
        private CollisionDetection collisionDetection;

        public CarCollisionUnitTest()
        {
            automatedCar = new AutomatedCar(100, 100, "car_1_white.png");
            collisionDetection = new CollisionDetection(automatedCar.VirtualFunctionBus);
        }

        [Fact]
        public void GetsPolylines()
        {
            PolylineGeometry pg = new PolylineGeometry();

            Assert.Equal(pg.GetType(), collisionDetection.GetCarPoints(automatedCar).GetType());
        }
    }
}