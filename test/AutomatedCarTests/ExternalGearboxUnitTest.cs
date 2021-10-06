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
                externalGearbox.ExternalUpshift();
            }

            Assert.Equal(3, externalGearbox.CurrentExternalGear);
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

            Assert.Equal(1, externalGearbox.CurrentExternalGear);
        }
    }
}
