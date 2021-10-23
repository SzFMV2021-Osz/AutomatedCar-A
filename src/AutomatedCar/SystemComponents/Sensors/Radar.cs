namespace AutomatedCar.SystemComponents.Sensors
{
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.Packets;

    public sealed class Radar : Sensor
    {
        public Radar(VirtualFunctionBus virtualFunctionBus)
            : base(virtualFunctionBus, 60, 200)
        {
            this.sensorPacket = new RadarPacket();
            virtualFunctionBus.RadarPacket = (IRadarPacket)this.sensorPacket;
        }

        public override void Process()
        {
            IAutomatedCar car = World.Instance.ControlledCar;
            this.CalculateSensorArea(car);
            this.FindObjectsInSensorArea(World.Instance.WorldObjects, car);
            this.FilterRelevantObjects();
            this.FindClosestObject(car);
        }

        protected override bool IsRelevant(IWorldObject worldObject)
        {
            return worldObject.Collideable;
        }

        // TODO for later PR  Meghatározni a legközelebbi, sávon belüli (lateral offset alapján) objektum helyzetét.
        // TODO for later PR Az automata vészfékező számára releváns objektumok(az autó középvonala felé halad, látjuk) kiválogatása és visszaadása.
    }
}