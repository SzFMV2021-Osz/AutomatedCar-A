namespace Tests
{
    using AutomatedCar.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class AbstractNPCTest
    {
        private AbstractNPC npc;
        [Theory]
        [InlineData(Vector[] {new Vector(0,0), new Vector(1,0)}, new Vector(10, 10) )]
        public void GetDirectionTest(Vector[] path, Vector expectedDirection)
        {

        }
    }
}
