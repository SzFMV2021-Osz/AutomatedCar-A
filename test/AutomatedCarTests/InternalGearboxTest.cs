namespace Tests
{
    using AutomatedCar.Models;
    using Xunit;

    public class InternalGearboxTest
    {
        private Gearbox internalGearbox;
        private AutomatedCar automatedCar;

        public InternalGearboxTest()
        {
            automatedCar = new AutomatedCar(100, 100, "car_1_white.png");
            internalGearbox = new Gearbox(automatedCar);
            automatedCar.Gearbox = internalGearbox;
        }

        [Fact]
        public void ShouldInternalUpshift()
        {
            // Arrange
            internalGearbox.CurrentExternalGearPosition = Gear.D;

            // Act
            for (int i = 0; i < 4; i++)
            {
                internalGearbox.InternalUpshift();
            }

            // Assert
            Assert.Equal(4, internalGearbox.CurrentInternalGear);
        }

        [Fact]
        public void ShouldInternalDownshift()
        {
            // Arrange
            internalGearbox.CurrentExternalGearPosition = Gear.D;
            internalGearbox.CurrentInternalGear = 4;

            // Act
            for (int i = 0; i < 4; i++)
            {
                internalGearbox.InternalDownshift();
            }

            // Assert
            Assert.Equal(1, internalGearbox.CurrentInternalGear);
        }

        [Fact]
        public void ShouldUpshiftUponHighRPM()
        {
            // Arrange
            internalGearbox.CurrentExternalGearPosition = Gear.D;
            internalGearbox.CurrentInternalGear = 1;

            // Act
            for (int i = 0; i < 100; i++)
            {
                automatedCar.IncreaseGasPedalPosition();
                automatedCar.CalculateNextPosition();
            }

            // Assert
            Assert.True(internalGearbox.CurrentInternalGear > 1);
        }

        [Fact]
        public void ShouldDownshiftUponLowRPM()
        {
            // Arrange
            internalGearbox.CurrentExternalGearPosition = Gear.D;
            internalGearbox.CurrentInternalGear = 4;

            // Act
            for (int i = 0; i < 100; i++)
            {
                automatedCar.IncreaseBrakePedalPosition();
                automatedCar.CalculateNextPosition();
            }

            // Assert
            Assert.Equal(1, internalGearbox.CurrentInternalGear);
        }

        [Fact]
        public void InternalShiftsToNeutral()
        {
            // Arrange
            internalGearbox.CurrentExternalGearPosition = Gear.D;
            internalGearbox.CurrentInternalGear = 4;

            // Act
            for (int i = 0; i < 4; i++)
            {
                internalGearbox.ExternalDownshift();
            }

            // Assert
            Assert.Equal(0, internalGearbox.CurrentInternalGear);
        }

        [Fact]
        public void ShouldNotShiftInNeutral()
        {
            // Arrange
            internalGearbox.CurrentExternalGearPosition = Gear.N;

            // Act
            for (int i = 0; i < 100; i++)
            {
                automatedCar.IncreaseGasPedalPosition();
                automatedCar.CalculateNextPosition();
            }

            // Assert
            Assert.Equal(0, internalGearbox.CurrentInternalGear);
        }
    }
}
