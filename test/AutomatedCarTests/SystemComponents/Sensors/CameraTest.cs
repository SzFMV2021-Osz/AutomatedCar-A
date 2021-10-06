namespace Tests.SystemComponents.Sensors
{
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Sensors;
    using Avalonia;
    using Avalonia.Media;
    using Moq;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Xunit;

    public class CameraTest
    {
        static ObservableCollection<PolylineGeometry> NearPointList = new ObservableCollection<PolylineGeometry>()
        {
            new PolylineGeometry(new List<Point> { new Point(100, 0) }, true)
        };

        static ObservableCollection<PolylineGeometry> FarPointList = new ObservableCollection<PolylineGeometry>()
        {
            new PolylineGeometry(new List<Point> { new Point(int.MaxValue, int.MaxValue) }, true)
        };

        Mock<IAutomatedCar> mockCar;
        Mock<IWorldObject> mockObjectInRange;
        Mock<IWorldObject> mockObjectOutOfRange;

        List<IWorldObject> DummyObjects;

        public CameraTest()
        {
            mockCar = new(MockBehavior.Loose);
            mockCar.SetupGet(m => m.X).Returns(100);
            mockCar.SetupGet(m => m.Y).Returns(100);
            mockCar.SetupGet(m => m.Rotation).Returns(0);
            mockCar.SetupGet(m => m.Geometries).Returns(NearPointList);
            mockCar.SetupGet(m => m.RawGeometries).Returns(NearPointList);

            mockObjectInRange = new();
            mockObjectInRange.SetupGet(m => m.Geometries).Returns(NearPointList);
            mockObjectInRange.SetupGet(m => m.RawGeometries).Returns(NearPointList);

            mockObjectOutOfRange = new();
            mockObjectOutOfRange.SetupGet(m => m.Geometries).Returns(FarPointList);
            mockObjectOutOfRange.SetupGet(m => m.RawGeometries).Returns(FarPointList);

            DummyObjects = new List<IWorldObject>
            {
                mockCar.Object,
                mockObjectInRange.Object,
                mockObjectOutOfRange.Object
            };
        }

        [Fact]
        public void CameraDoesNotSeeItself()
        {
            //Given
            Camera camera = new Camera();

            //When
            List<IWorldObject> FilteredObjects = camera.RelevantObjects(DummyObjects, mockCar.Object);

            //Then
            Assert.DoesNotContain(mockCar.Object, FilteredObjects);
        }

        [Fact]
        public void CameraDoesNotSeeObjectsOutOfRange()
        {
            //Given
            Camera camera = new Camera();

            //When
            List<IWorldObject> FilteredObjects = camera.RelevantObjects(DummyObjects, mockCar.Object);

            //Then
            Assert.DoesNotContain(mockObjectOutOfRange.Object, FilteredObjects);
        }
    }
}