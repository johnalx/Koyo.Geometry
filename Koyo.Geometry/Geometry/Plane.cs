using System;
using System.Numerics;

namespace JA.Geometry
{
    public class Plane 
    {
        internal Vector3 Vector { get; }
        internal float Scalar { get; }
        protected Plane(Plane plane)
            : this(plane.Vector, plane.Scalar) { }
        internal Plane(Vector3 vector, float scalar)
        {
            this.Vector=vector;
            this.Scalar=scalar;
        }
        public static Plane FromPointAndNormal(Point point, Vector3 normal)
            //tex: Plane through point $G=\pmatrix{\vec{g} & g}$  with normal vector $\hat{n}$:
            // $$\mathbf{plane} = \pmatrix{g\,\hat{n} \\ -\vec{g}\cdot \hat{n}} $$
            => new Plane(point.Scalar*normal, -Vector3.Dot(point.Vector, normal));
        

        public static Plane FromPoint(Point point)
            //tex: Plane through point $G=\pmatrix{\vec{g} & g}$  furthest from the origin:
            // $$\mathbf{plane} = \pmatrix{g\,\vec{g} \\ -|\vec{g}|^2 } $$
            => new Plane(point.Scalar*point.Vector, -point.Vector.LengthSquared());

        public static Plane FromLine(Line line)
            //tex: Plane through line $L=\pmatrix{\vec{v} & \vec{m}}$  furthest from the origin:
            // $$\mathbf{plane} = \pmatrix{\vec{v} \times \vec{m} \\ -\vec{m}\cdot\vec{m}} $$
            => new Plane(Vector3.Cross(line.Vector, line.Moment), -line.Moment.LengthSquared());

        public static readonly Plane XY = new Plane(Vector3.UnitZ,0);
        public static readonly Plane YZ = new Plane(Vector3.UnitX,0);
        public static readonly Plane ZX = new Plane(Vector3.UnitY,0);
        public static readonly Plane Inf = new Plane(Vector3.Zero, 1);

        public static Plane FromNormalAndOffset(Vector3 normal, float offset)
            => new Plane(Vector3.Normalize(normal), -offset);

        public static Plane Join(Point point, Line line)
            //tex: Plane where point $G=\pmatrix{\vec{g} & g}$ and line $L = \pmatrix{\vec{v} & \vec{m}}$ join:
            //$$\mathbf{plane}=\pmatrix{ \vec{v}\times \vec{g} + g\, \vec{m} \\ - \vec{g} \cdot \vec{m}}$$
            => new Plane(
                Vector3.Cross(line.Vector, point.Vector) + point.Scalar * line.Moment,
                -Vector3.Dot(point.Vector, line.Moment));
        
        public static Plane Join(Point point_1, Point point_2, Point point_3)
            //tex: Plane where three point join $G_1$, $G_2$ and $G_3$
            //$$\mathbf{plane}( G_1, \; \mathbf{line}(G_2,\,G_3))$$
            => Join(point_1, Line.Join(point_2, point_3));

        public float Magnitude { get => Vector.Length(); }
        public Vector3 Normal { get => Vector3.Normalize(Vector); }
        public float Offset { get => -Scalar/Vector.Length(); }
        public Point Origin
        {
            get => Point.FromPlane(this);
        }
        public float DistanceTo(Point point)
            => point.DistanceTo(this);

        public Point Project(Point point)
        {
            float t = Vector3.Dot(Normal, point.Position)-Offset;
            return point.Position - Normal*t;
        }

        public static Plane Lerp(Plane from, Plane to, float amount)
            => new Plane((1-amount)*from.Vector + amount*to.Vector,
                (1-amount)*from.Scalar + amount * to.Scalar);

        public static Plane Barycentric(Plane[] points, float[] weights)
        {
            if (points.Length != weights.Length)
            {
                throw new ArgumentException("Weights must the same count as Planes", nameof(weights));
            }
            var vec = Vector3.Zero;
            var scl = 0f;
            for (int i = 0; i < points.Length; i++)
            {
                vec += weights[i] * points[i].Vector;
                scl += weights[i] * points[i].Scalar;
            }
            return new Plane(vec, scl);
        }

        public static Plane operator +(Plane point, float delta)
            => new Plane(point.Vector, point.Scalar - delta * point.Vector.Length());
        public static Plane operator -(Plane point, float delta)
            => new Plane(point.Vector, point.Scalar + delta * point.Vector.Length());

        #region Formatting
        public override string ToString() => ToString("g");
        public string ToString(string formatting) => ToString(formatting, null);
        public string ToString(string format, IFormatProvider provider)
            => $"Normal={Normal.ToString(format, provider)} Offset={Offset.ToString(format, provider)}";
        #endregion
    }


}
