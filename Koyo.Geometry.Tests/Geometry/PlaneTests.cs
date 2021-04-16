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
    public class PlaneTests
    {
        private const float defaultDelta = 0.000001f;

        static readonly Random rng = new Random();
        [TestMethod()]
        public void PlaneTest()
        {
            var vector = new Vector3(
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble());
            var scalar = 10*(float)rng.NextDouble();

            var normal = Vector3.Normalize(vector);
            var dist = -scalar/vector.Length();

            var W = new Plane(vector, scalar);

            Assert.IsTrue(W.Magnitude == vector.Length());
            Assert.AreEqual(normal, W.Normal);
            Assert.AreEqual(dist, W.Offset);
        }

        [TestMethod()]
        public void PlaneTest1()
        {
            var P = Point.FromPosition(3, 2, 1);
            var W = Plane.FromPointAndNormal(P, 5*Vector3.UnitZ);

            var normal = Vector3.UnitZ;
            var dist = 1f;

            Assert.IsTrue(W.Magnitude == 5f);
            Assert.AreEqual(normal, W.Normal);
            Assert.AreEqual(dist, W.Offset);
        }

        [TestMethod()]
        public void FromPointTest()
        {
            var P = Point.FromPosition(3, 2, 1);
            var W = Plane.FromPoint(P);

            Assert.AreEqual(Vector3.Normalize(P.Position), W.Normal);
            var exp = P.Position.Length();
            Assert.AreEqual(exp, W.Offset, defaultDelta);            
        }

        [TestMethod()]
        public void FromLineTest()
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
            var mom = Vector3.Cross(point, along);
            var org = Vector3.Cross(along, mom)/along.LengthSquared();

            var W = Plane.FromLine(L);
            var exp = Vector3.Normalize(org);
            var nor = W.Normal;

            Assert.AreEqual(exp.X, nor.X, defaultDelta);
            Assert.AreEqual(exp.Y, nor.Y, defaultDelta);
            Assert.AreEqual(exp.Z, nor.Z, defaultDelta);

            Assert.AreEqual(org.Length(), W.Offset, defaultDelta);

        }

        [TestMethod()]
        public void FromNormalAndOffsetTest()
        {
            var WX = Plane.FromNormalAndOffset(Vector3.UnitX, 3);
            var WY = Plane.FromNormalAndOffset(Vector3.UnitY, 2);
            var WZ = Plane.FromNormalAndOffset(Vector3.UnitZ, 1);

            Assert.AreEqual(Vector3.UnitX, WX.Normal);
            Assert.AreEqual(Vector3.UnitY, WY.Normal);
            Assert.AreEqual(Vector3.UnitZ, WZ.Normal);

            Assert.IsTrue(WX.Offset == 3f);
            Assert.IsTrue(WY.Offset == 2f);
            Assert.IsTrue(WZ.Offset == 1f);
        }

        [TestMethod()]
        public void JoinTestPointAndLine()
        {
            var G = Point.FromPosition(3, 2, 1);
            var L = Line.Ray(Point.Origin + 2*Vector3.UnitY, Vector3.UnitX);
            var W = Plane.Join(G, L);

            Assert.AreEqual(-Vector3.UnitY, W.Normal);
            Assert.IsTrue(W.Offset == -2f);
        }

        [TestMethod()]
        public void JoinTestThreePoints()
        {
            var A = Point.FromPosition(3, 2, 1);
            var B = Point.FromPosition(3, -1, 1);
            var C = Point.FromPosition(-1, 2, 1);

            var W = Plane.Join(A, B, C);

            Assert.AreEqual(-Vector3.UnitZ, W.Normal);
            Assert.IsTrue(W.Offset == -1f);
        }

        [TestMethod()]
        public void DistanceToTest()
        {
            var vector = new Vector3(
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble());
            var scalar = 10*(float)rng.NextDouble();

            var W = new Plane(vector, scalar);

            var point = new Vector3(
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble());

            var A = Point.FromPosition(point);

            var norm = Vector3.Normalize(vector);
            var ofs = -scalar/vector.Length();

            var dist = Vector3.Dot(norm, point) - ofs;
            var actual = W.DistanceTo(A);
            Assert.AreEqual(actual,dist, defaultDelta);
        }

        [TestMethod()]
        public void ProjectTest()
        {
            var vector = new Vector3(
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble());
            var scalar = 10*(float)rng.NextDouble();

            var W = new Plane(vector, scalar);

            var point = new Vector3(
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble());
            var A = Point.FromPosition(point);

            var dist = W.DistanceTo(A);

            var B = A - dist * W.Normal;

            var act = W.Project(A);            

            Assert.AreEqual(B.Position.X, act.Position.X, defaultDelta);
            Assert.AreEqual(B.Position.Y, act.Position.Y, defaultDelta);
            Assert.AreEqual(B.Position.Z, act.Position.Z, defaultDelta);
        }

    }
}