namespace AutomatedCar.SystemComponents
{
    using AutomatedCar.SystemComponents.Helpers;
    using System.Collections.Generic;

    public class VirtualFunctionBus : GameBase
    {
        private List<SystemComponent> components = new List<SystemComponent>();
        public ICollisionDetectionPacket CollisionDetectionPacket { get; set; }
        public void RegisterComponent(SystemComponent component)
        {
            this.components.Add(component);
        }

        protected override void Tick()
        {
            foreach (SystemComponent component in this.components)
            {
                component.Process();
            }
        }
    }
}