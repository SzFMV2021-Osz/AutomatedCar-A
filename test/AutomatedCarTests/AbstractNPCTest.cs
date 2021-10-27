﻿namespace Tests
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
        private class NPCPathDataProvider : IEnumerable<object[]>
        {
            IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    new List<Vector>()
                    {
                        new Vector(0, 0),
                        new Vector(1, 1)
                    },
                    new Vector(5, 5)
                };
                yield return new object[]
                {
                    new List<Vector>()
                    {
                        new Vector(100, 100),
                        new Vector(110, 110)
                    },
                    new Vector(5, 5)
                };
            }

            IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator() => GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private AbstractNPC npc;
        [Theory]
        [ClassData(typeof(NPCPathDataProvider))]
        public void GetDirectionTest(Vector[] path, Vector expectedDirection)
        {
            var yeppe = path;
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
                        new Vector(10, 0),
                        new Vector(10, 20)
                    },
                    new Vector(10, 20),
                    1,
                    30
                };
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
            var npc = new NonPlayerCar(0, 0, "") { PathCoordinates = path, Speed = speed };
            foreach (var item in Enumerable.Range(0, expectedAmountOfMoves))
            {
                npc.MoveForward(speed);
            }
            Assert.Equal(npc.X, expectedDestination.X);
            Assert.Equal(npc.Y, expectedDestination.Y);
        }

    }
}
