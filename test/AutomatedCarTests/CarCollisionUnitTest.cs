namespace Tests
{
    using AutomatedCar;
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents;
    using AutomatedCar.SystemComponents.Helpers;
    using Avalonia;
    using Avalonia.Media;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
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
        public void IndentifiesNpcCollisionType()
        {
            Assert.Equal(CollisionType.NPC, collisionDetection.DetermineCollisionType(automatedCar));
        }

        [Fact]
        public void IndentifiesStaticObjectCollisionType()
        {
            Assert.Equal(CollisionType.StaticObject, collisionDetection.DetermineCollisionType(new WorldObject(100, 100, "car_1_white.png")));
        }

        [Fact]
        public void CarPointXCorrect()
        {
            automatedCar.Geometry = MockCarPoints();

            // Expected 151, because car point X is 51 + word position 100
            Assert.Equal(151, collisionDetection.GetCarPoints(automatedCar).Points[0].X);
        }

        [Fact]
        public void CarPointYCorrect()
        {
            automatedCar.Geometry = MockCarPoints();

            // Expected 339, because car point Y is 239 + word position 100
            Assert.Equal(339, collisionDetection.GetCarPoints(automatedCar).Points[0].Y);
        }

        private PolylineGeometry MockCarPoints()
        {
            var points = new List<Point>();
            
            points.Add(new Point(51, 239));

            return new PolylineGeometry(points, false);
        }
    }
}