using JA.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JA
{
    class Program
    {
        static void Main(string[] args)
        {
            var disk_1 = new Disk(Point.Origin, Vector3.UnitZ, 1);
            var disk_2 = new Disk(
                Point.Origin + Vector3.UnitY,
                Vector3.UnitY,
                0.6f);
            if (Disk.Intersect(disk_1, disk_2, out Point contactPoint))
            {
                Debug.WriteLine($"Crash at {contactPoint.Position}");
            }
        }
    }
}
