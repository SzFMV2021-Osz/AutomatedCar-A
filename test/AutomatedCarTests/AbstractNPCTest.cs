namespace Tests
{
    using AutomatedCar.Models;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class AbstractNPCTest
    {
        private class MovementTestDataProvider : IEnumerable<object[]>
        {
            IEnumerator<object[]> GetEnumerator()
            {
                //linear test
                yield return new object[]
                {
                    new List<Vector>()
                    {
                        new Vector(0, 0),
                        new Vector(10, 0),
                        new Vector(10, 20)
                    },
                    new Vector(10, 20),
                    1,
                    30
                };
                //diagonal test
                yield return new object[]
                {
                    new List<Vector>()
                    {
                        new Vector(0, 0),
                        new Vector(3, 4),
                        new Vector(0, 8)
                    },
                    new Vector(0, 8),
                    1,
                    10
                };
            }

            IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator() => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(MovementTestDataProvider))]
        public void MovementTest(List<Vector> path, Vector expectedDestination, int speed, int expectedAmountOfMoves)
        {
            var startTime = DateTime.Now;
            var npc = new NonPlayerCar(0, 0, "") { PathCoordinates = path, Speed = speed, TimeOfLastMove = startTime};
            foreach (var item in Enumerable.Range(0, expectedAmountOfMoves))
            {
                var before = new Vector(npc.X, npc.Y);
                npc.StepObject(startTime + TimeSpan.FromSeconds(item + 1));
                var after = new Vector(npc.X, npc.Y);
                Assert.NotEqual(before, after);
            }
            Assert.Equal(npc.X, expectedDestination.X);
            Assert.Equal(npc.Y, expectedDestination.Y);
        }

    }
}
