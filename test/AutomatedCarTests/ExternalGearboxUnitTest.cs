namespace Tests
{
    using AutomatedCar.Models;
    using Xunit;

    public class ExternalGearboxUnitTest 
    {
        private ExternalGearbox externalGearbox;
        private AutomatedCar automatedCar;

        public ExternalGearboxUnitTest()
        {
            automatedCar = new AutomatedCar(100, 100, "car_1_white.png");
            externalGearbox = new ExternalGearbox(automatedCar);
        }

        [Fact]
        public void ShouldExternalUpshift()
        {
            for (int i = 0; i < 3; i++)
            {
                externalGearbox.Upshift();
            }

            Assert.Equal(ExternalGearbox.Gear.D, externalGearbox.currentGearPosition);
        }

        [Fact]
        public void ShouldExternalDownShift()
        {
            for (int i = 0; i < 3; i++)
            {
                externalGearbox.Upshift();
            }

            for (int i = 0; i < 2; i++)
            {
                externalGearbox.Downshift();
            }

            Assert.Equal(ExternalGearbox.Gear.R, externalGearbox.currentGearPosition);
        }
    }
}
