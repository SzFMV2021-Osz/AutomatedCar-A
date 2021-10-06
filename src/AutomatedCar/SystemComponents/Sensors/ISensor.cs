using AutomatedCar.Models;
using System.Collections.Generic;

namespace AutomatedCar.SystemComponents.Sensors
{
    public interface ISensor
    {
        List<IWorldObject> RelevantObjects(IEnumerable<IWorldObject> worldObjects, IAutomatedCar car);
    }
}