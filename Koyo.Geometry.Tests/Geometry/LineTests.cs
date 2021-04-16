using Microsoft.VisualStudio.TestTools.UnitTesting;
using JA.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace JA.Geometry.Tests
{
    [TestClass()]
    public class LineTests
    {
        private const float defaultDelta = 0.000001f;

        static readonly Random rng = new Random();

        [TestMethod()]
        public void RayTest()
        {
            var point = new Vector3(
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble());
            var along = new Vector3(
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble());

            var L = Line.Ray(point, along);

            Assert.IsTrue(L.Magnitude == along.Length());
            Assert.AreEqual(along, L.Vector);
            var mom = Vector3.Cross(point, along);
            Assert.AreEqual(mom, L.Moment);
            var dir = Vector3.Normalize(along);
            Assert.AreEqual(dir, L.Direction);
            var org = Vector3.Cross(along, mom)/along.LengthSquared();
            Assert.AreEqual(L.Offset, org.Length(), defaultDelta);
            Assert.AreEqual(org.X, L.Origin.Position.X, defaultDelta);
            Assert.AreEqual(org.Y, L.Origin.Position.Y, defaultDelta);
            Assert.AreEqual(org.Z, L.Origin.Position.Z, defaultDelta);
        }

        [TestMethod()]
        public void JoinTestTwoPoints()
        {
            var A = Point.FromPosition(3, 2, 1);
            var B = Point.FromPosition(3, -1, 1);

            var L = Line.Join(A, B);

            Assert.AreEqual(-Vector3.UnitY, L.Direction);
            var org = new Vector3(3, 0, 1);
            Assert.AreEqual(org, L.Origin.Position);            
        }

        [TestMethod()]
        public void MeetTest()
        {
            var A = Plane.FromNormalAndOffset(Vector3.UnitZ, 3);
            var B = Plane.FromNormalAndOffset(Vector3.UnitY, 2);
            var C = Plane.FromNormalAndOffset(Vector3.UnitX, 1);

            var LAB = Line.Meet(A, B);
            var LBC = Line.Meet(B, C);
            var LCA = Line.Meet(C, A);

            Assert.AreEqual(-Vector3.UnitX, LAB.Direction);
            Assert.AreEqual(new Vector3(0, 2, 3), LAB.Origin.Position);

            Assert.AreEqual(-Vector3.UnitZ, LBC.Direction);
            Assert.AreEqual(new Vector3(1, 2, 0), LBC.Origin.Position);

            Assert.AreEqual(-Vector3.UnitY, LCA.Direction);
            Assert.AreEqual(new Vector3(1, 0, 3), LCA.Origin.Position);
        }

        [TestMethod()]
        public void AlongTest()
        {
            var A = Point.FromPosition(3, 2, 1);
            var B = Point.FromPosition(3, -1, 1);

            var L = Line.Join(A, B);


            var f = 10*(float)rng.NextDouble();

            var O = L.Origin;
            var exp = O.Position + f * L.Direction;
            var act = L.Along(f).Position;            

            Assert.AreEqual(exp.X, act.X, defaultDelta);
            Assert.AreEqual(exp.Y, act.Y, defaultDelta);
            Assert.AreEqual(exp.Z, act.Z, defaultDelta);

        }

        [TestMethod()]
        public void DistanceToTestPoint()
        {
            var A = Point.FromPosition(3, 2, 1);
            var B = Point.FromPosition(3, -1, 1);
            var C = Point.FromPosition(5, 1, -1);

            var L = Line.Join(A, B);

            var dist = L.DistanceTo(C);

            var exp = (float)Math.Sqrt(8);

            Assert.AreEqual( exp, dist, defaultDelta );
        }

        [TestMethod()]
        public void DistanceToTestLine()
        {
            var A = Point.FromPosition(3, 2, 1);
            var B = Point.FromPosition(3, -1, 1);
            var W = Line.Ray(Point.FromPosition(2, 3, 1), Vector3.UnitZ);

            var L = Line.Join(A, B);

            var dist = L.DistanceTo(W);

            Assert.AreEqual(-1f, dist);

            var C = L.Project(W);
            var exp = new Vector3(3, 3, 1);

            Assert.AreEqual(exp, C.Position);
        }

        [TestMethod()]
        public void ProjectTest()
        {
            var A = Point.FromPosition(3, 2, 1);
            var B = Point.FromPosition(3, -1, 1);
            var C = Point.FromPosition(5, 1, -1);

            var L = Line.Join(A, B);

            var PC = L.Project(C);

            var exp = new Vector3(3, 1, 1);

            Assert.AreEqual(exp, PC.Position);
        }
    }
}