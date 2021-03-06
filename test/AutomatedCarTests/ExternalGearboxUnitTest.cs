namespace Tests
{
    using AutomatedCar.Models;
    using Xunit;

    public class ExternalGearboxUnitTest 
    {
        private Gearbox externalGearbox;
        private AutomatedCar automatedCar;

        public ExternalGearboxUnitTest()
        {
            automatedCar = new AutomatedCar(100, 100, "car_1_white.png");
            externalGearbox = new Gearbox(automatedCar);
        }

        [Fact]
        public void ShouldExternalUpshift()
        {
            for (int i = 0; i < 3; i++)
            {
                externalGearbox.ExternalUpshift();
            }

            Assert.Equal(Gear.D, externalGearbox.CurrentExternalGearPosition);
        }

        [Fact]
        public void ShouldExternalDownShift()
        {
            for (int i = 0; i < 3; i++)
            {
                externalGearbox.ExternalUpshift();
            }

            for (int i = 0; i < 2; i++)
            {
                externalGearbox.ExternalDownshift();
            }

            Assert.Equal(Gear.R, externalGearbox.CurrentExternalGearPosition);
        }
    }
}
