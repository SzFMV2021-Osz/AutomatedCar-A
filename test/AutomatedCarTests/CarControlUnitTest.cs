namespace Tests
{
    using AutomatedCar;
    using AutomatedCar.Models;
    using System.Linq;
    using Xunit;
    using Xunit.Abstractions;

    public class CarControlUnitTest
    {

        private AutomatedCar automatedCar;
        private readonly ITestOutputHelper output;


        public CarControlUnitTest(ITestOutputHelper output)
        {
            automatedCar = new AutomatedCar(100, 100, "car_1_white.png");
            this.output = output;

        }

        [Fact]
        public void ShouldBoundGasPedalPosition()
        {
            // when
            for (int i = 0; i < 10; i++)
            {
                automatedCar.IncreaseGasPedalPosition();
            }
            
            // then
            Assert.Equal(100, automatedCar.GasPedalPosition);
        }
        
        [Fact]
        public void ShouldBoundBrakePedalPosition()
        {
            // when
            for (int i = 0; i < 10; i++)
            {
                automatedCar.DecreaseBrakePedalPosition();
            }
            
            // then
            Assert.Equal(0, automatedCar.BrakePedalPosition);
        }

        [Fact]
        public void ShouldCalculateNextPositionAndProperties()
        {
            // given
            for (int i = 0; i < 3; i++)
            {
                automatedCar.ExternalGearbox.ExternalUpshift();

            }

            for (int i = 0; i < 10; i++)
            {
                automatedCar.IncreaseGasPedalPosition();
            }

            // when
            for (int i = 0; i < 10; i++)
            {
                automatedCar.CalculateNextPosition();
            }

            // then
            Assert.Equal(100, automatedCar.GasPedalPosition);
            Assert.Equal(1, automatedCar.Acceleration.Y);
            Assert.True(automatedCar.Velocity.Y > -10);

            Assert.True(automatedCar.Y < 100);
        }

        [Fact]
        public void ShouldStopCarWhenGasNotPressed()
        {
            // given
            for (int i = 0; i < 3; i++)
            {
                automatedCar.ExternalGearbox.ExternalUpshift();

            }

            for (int i = 0; i < 3; i++)
            {
                automatedCar.IncreaseGasPedalPosition();
            }
            
            for (int i = 0; i < 3; i++)
            {
                automatedCar.CalculateNextPosition();
            }


            // when
            for (int i = 0; i < 3; i++)
            {
                automatedCar.DecreaseGasPedalPosition();
            }

            Assert.Equal(1, automatedCar.Speed);
            
            for (int i = 0; i < 200; i++)
            {
                automatedCar.CalculateNextPosition();
            }

            // then
            Assert.Equal(0, automatedCar.Speed);
        }
    }
}