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
        private class GetDirectionDataProvider : IEnumerable<object[]>
        {
            IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    new List<Vector>()
                    {
                        new Vector(1, 0),
                        new Vector(2, 0),
                        new Vector(2, 1),
                        new Vector(2, 2),
                        new Vector(3, 2),
                        new Vector(4, 2),
                        new Vector(5, 2),
                    },
                    new List<Vector>()
                    {
                        new Vector(1, 0),
                        new Vector(1, 0),
                        new Vector(0, 1),
                        new Vector(0, 1),
                        new Vector(1, 0),
                        new Vector(1, 0),
                        new Vector(1, 0),
                    }
                };
                yield return new object[]
                {
                    new List<Vector>()
                    {
                        new Vector(100, 100),
                        new Vector(110, 100),
                        new Vector(130, 100),
                        new Vector(130, 124)
                    },
                    new List<Vector>()
                    {
                        new Vector(100/Math.Sqrt(20000), 100/Math.Sqrt(20000)),
                        new Vector(1, 0),
                        new Vector(1, 0),
                        new Vector(0, 1)
                    }
                };
            }

            IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator() => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private AbstractNPC npc;
        [Theory]
        [ClassData(typeof(GetDirectionDataProvider))]
        public void GetDirectionTest(Vector[] path, List<Vector> expectedDirections)
        {
            var npc = new NonPlayerCar(0, 0, "") { PathCoordinates = path, Speed = 1 };
            int i = 0;
            foreach (var item in path)
            {
                npc.NextTurn = item;
                Assert.Equal(npc.GetDirection(), expectedDirections[npc.PathCoordinates.IndexOf(item)]);
                npc.X = (int)item.X;
                npc.Y = (int)item.Y;
            }
        }

        private class MovementTestDataProvider : IEnumerable<object[]>
        {
            IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    new List<Vector>()
                    {
                        new Vector(0, 0),
                        new Vector(10, 10),
                        new Vector(10, 20)
                    },
                    new Vector(10, 20),
                    1,
                };
            }

            IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator() => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(MovementTestDataProvider))]
        public void MovementTest(List<Vector> path, Vector expectedDestination, int speed)
        {
            var npc = new NonPlayerCar(0, 0, "") { PathCoordinates = path, Speed = speed };

        }

    }
}
