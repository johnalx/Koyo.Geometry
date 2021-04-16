using System;
using System.Numerics;

namespace JA.Geometry
{
    public class Disk : Plane
    {
        internal Disk(Point center, Vector3 normal, float radius)
            : base(FromPointAndNormal(center, normal))
        {
            this.Center=center;
            this.Radius=radius;
        }

        public Point Center { get; }
        public float Radius { get; }

        public bool Containts(Point point)
        {
            if (DistanceTo(point)<= 1e-8f)
            {
                var d = (point - Center).Length();
                return d<=Radius;
            }
            return false;
        }

        public static bool Intersect(Disk disk_1, Disk disk_2, out Point contactPoint)
        {
            var commonLine = Line.Meet(disk_1, disk_2);
            var p_1 = commonLine.Project(disk_1.Center);
            var p_2 = commonLine.Project(disk_2.Center);
            var ell = p_1.DistanceTo(p_2);
            var h_1 = commonLine.DistanceTo(disk_1.Center);
            var h_2 = commonLine.DistanceTo(disk_2.Center);
            if (Math.Abs(h_1+h_2)>1e-8)
            {
                var t_1 = h_1*ell/(h_1+h_2);
                contactPoint = p_1 - commonLine.Direction * t_1;
                return disk_1.Containts(contactPoint) && disk_2.Containts(contactPoint);
            }
            else
            {
                var t_1 = disk_1.Radius*ell/(disk_1.Radius+disk_2.Radius);
                contactPoint = p_1 - commonLine.Direction * t_1;
                return ell <= disk_1.Radius + disk_2.Radius;
            }
        }
    }


}
