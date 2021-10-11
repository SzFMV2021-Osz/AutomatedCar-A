namespace Tests
{
    using AutomatedCar;
    using AutomatedCar.Models;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class CarControlUnitTest
    {

        private AutomatedCar automatedCar;

        //Dictionary<int, double> TorqueLookupTable

        public CarControlUnitTest()
        {
            automatedCar = new AutomatedCar(100, 100, "car_1_white.png");
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
            Assert.Equal(automatedCar.GenerateEngineForce(), automatedCar.Acceleration.Y);
            Assert.True(automatedCar.Velocity.Y > -10);
            Assert.True(automatedCar.Y < 100);
        }

        [Fact]
        public void ShouldGenerateEngineForce()
        {
            // given
            int expedtedForce = (int)((100 * 1 * 0.7 * 3.42 * 2.66) / 0.34);
            expedtedForce /= 1500;

            for (int i = 0; i < 10; i++)
            {
                automatedCar.IncreaseGasPedalPosition();
            }

            this.automatedCar.Revolution = 2000;

            // when

            int engineForce = (int)(this.automatedCar.GenerateEngineForce());

            // then

            Assert.Equal(100, automatedCar.GasPedalPosition);
            Assert.Equal(2000, automatedCar.Revolution);
            Assert.Equal(engineForce, expedtedForce);
        }

        [Theory]
        [InlineData(3800, 200)]
        [InlineData(2300, 100)]
        [InlineData(5000, 250)]
        [InlineData(3500, 150)]
        public void ShouldLookupTorque(int rpm, int expected)
        {
            // given

            // when

            double torque = this.automatedCar.LookupTorqueCurve(rpm);

            // then

            Assert.Equal(torque, expected);
        }

        [Fact]
        public void ShouldStopCarWhenGasNotPressed()
        {
            // given
            for (int i = 0; i < 4; i++)
            {
                automatedCar.IncreaseGasPedalPosition();
            }
            
            for (int i = 0; i < 4; i++)
            {
                automatedCar.CalculateNextPosition();
            }
            // when
            for (int i = 0; i < 4; i++)
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