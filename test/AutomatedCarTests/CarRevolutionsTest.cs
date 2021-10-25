namespace Tests
{
    using AutomatedCar;
    using AutomatedCar.Models;
    using System.Linq;
    using Xunit;

    public class CarRevolutionsTest
    {
        private AutomatedCar car;

        public CarRevolutionsTest()
        {
            car = new AutomatedCar(100, 100, "car_1_white.png");
        }

        [Fact]
        public void ShouldIncreaseRpm()
        {
            // Arrange
            int rpmBefore = car.Revolution;
            car.ExternalGearbox.Upshift();

            // Act
            for (int i = 0; i < 10; i++)
            {
                car.IncreaseGasPedalPosition();
            }

            car.CalculateNextPosition();

            // Assert
            Assert.True(car.Revolution > rpmBefore);
        }

        [Fact]
        public void ShouldDecreaseRpmWhenBraking()
        {
            // Arrange
            int rpmBefore = car.Revolution = 1500;

            // Act
            for (int i = 0; i < 10; i++)
            {
                car.IncreaseBrakePedalPosition();
            }

            car.CalculateNextPosition();

            // Assert
            Assert.True(car.Revolution < rpmBefore);
        }

        [Fact]
        public void ShouldDecreaseRpmWhenRolling()
        {
            // Arrange
            int rpmBefore = car.Revolution = 1500;
            car.Speed = 50;

            // Act
            for (int i = 0; i < 10; i++)
            {
                car.Speed--;
            }

            car.CalculateNextPosition();

            // Assert
            Assert.True(car.Revolution < rpmBefore);
        }

        [Fact]
        public void ShouldReachIdle()
        {
            // Arrange 
            car.Revolution = 1500;

            // Act
            for (int i = 0; i < 300; i++)
            {
                car.IncreaseBrakePedalPosition();
            }

            int reachedRPM = car.Revolution;

            car.IncreaseBrakePedalPosition();

            // Assert
            Assert.Equal(car.Revolution, reachedRPM); // so that the RPM reached a low, and it's not changing
            Assert.True(car.Revolution > 0); // and this low is the idle RPM, so it's greater than 0
        }
    }
}