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
    public class PointTests
    {
        private const float defaultDelta = 0.000001f;
        static readonly Random rng = new Random();

        [TestMethod()]
        public void PointTestFromPosition()
        {
            var position = new Vector3(
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble());

            var g = Point.FromPosition(position);

            Assert.IsTrue(g.Magnitude==1);
            Assert.AreEqual(position, g.Position);                        
        }

        [TestMethod()]
        public void PointTestFromVectorScalar()
        {
            var vector = new Vector3(
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble());
            var scalar = 10*(float)rng.NextDouble();

            var g = new Point(vector, scalar);
            Assert.IsTrue(g.Magnitude == scalar);
            Assert.AreEqual(vector/scalar, g.Position);
        }

        [TestMethod()]
        public void PointTestFromPlane()
        {
            var A = Plane.FromNormalAndOffset(Vector3.UnitX, 2.5f);
            var point = new Vector3(2.5f, 0, 0);
            var g = Point.FromPlane(A);
            Assert.AreEqual(point, g.Position);
        }

        [TestMethod()]
        public void PointTestFromLine()
        {
            var L = Line.Ray(Point.Origin + 2.5f*Vector3.UnitX, Vector3.UnitZ);
            var point = new Vector3(2.5f, 0, 0);
            var g = Point.FromLine(L);
            Assert.AreEqual(point, g.Position);
        }

        [TestMethod()]
        public void MeetTestPlaneAndLine()
        {
            var plane = Plane.FromNormalAndOffset(Vector3.UnitY, 0.5f);
            var line = Line.Ray(Point.Origin, Vector3.UnitX + 0.5f*Vector3.UnitY);
            var point = new Vector3(1, 0.5f, 0);
            var g = Point.Meet(plane, line);
            Assert.AreEqual(point, g.Position);            
        }

        [TestMethod()]
        public void MeetTestThreePlanes()
        {
            var A = Plane.FromNormalAndOffset(Vector3.UnitX, 3);
            var B = Plane.FromNormalAndOffset(Vector3.UnitY, 2);
            var C = Plane.FromNormalAndOffset(Vector3.UnitZ, 1);
            var point = new Vector3(3, 2, 1);
            var g = Point.Meet(A, B, C);

            Assert.AreEqual(point, g.Position);            
        }

        [TestMethod()]
        public void DistanceToPoint()
        {
            var posA = new Vector3(
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble());
            var posB = new Vector3(
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble());

            var dist = Vector3.Distance(posA, posB);

            var A = Point.FromPosition(posA);
            var B = Point.FromPosition(posB);

            Assert.AreEqual(dist, A.DistanceTo(B), defaultDelta);
            
        }

        [TestMethod()]
        public void DistanceToPlane()
        {
            var pos = new Vector3(
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble(),
                10*(float)rng.NextDouble());

            var A = Point.FromPosition(pos);
            var P = Plane.FromNormalAndOffset(Vector3.UnitZ, 10f);

            Assert.AreEqual(pos.Z-10, A.DistanceTo(P));
        }

        [TestMethod()]
        public void DistanceToLine()
        {
            var pos = new Vector3(
                10*(float)rng.NextDouble(),
                0,
                0);

            var A = Point.FromPosition(pos);
            var B = Point.FromPosition(10, 0, 0);
            var L1 = Line.Ray(B, Vector3.UnitZ);
            var L2 = Line.Ray(B, Vector3.UnitY);
            var L3 = Line.Ray(B, Vector3.UnitX);

            Assert.AreEqual(10 - pos.X, A.DistanceTo(L1));
            Assert.AreEqual(10 - pos.X, A.DistanceTo(L2));
            Assert.AreEqual(0, A.DistanceTo(L3));
        }

    }
}