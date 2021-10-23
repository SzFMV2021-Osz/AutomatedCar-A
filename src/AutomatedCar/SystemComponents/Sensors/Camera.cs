namespace AutomatedCar.SystemComponents.Sensors
{
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets;

    public sealed class Camera : Sensor
    {
        public Camera(VirtualFunctionBus virtualFunctionBus)
            : base(virtualFunctionBus, 60, 80)
        {
            this.sensorPacket = new CameraPacket();
            virtualFunctionBus.CameraPacket = (ICameraPacket)this.sensorPacket;
        }

        public override void Process()
        {
            IAutomatedCar car = World.Instance.ControlledCar;
            this.CalculateSensorArea(car);
            this.FindObjectsInSensorArea(World.Instance.WorldObjects, car);
            this.FilterRelevantObjects();
            this.FindClosestObject(car);
        }

        // "A kamerára a sávtartóautomatika (LKA) és a táblafelismerő (TSR) épül, így annak a szenzornak az útelemek és a táblák relevánsak."
        protected override bool IsRelevant(IWorldObject worldObject)
        {
            return worldObject.Filename.Contains("Road");
        }
    }
}