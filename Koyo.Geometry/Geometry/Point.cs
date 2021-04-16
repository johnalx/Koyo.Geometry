using System;
using System.Numerics;

namespace JA.Geometry
{
    public class Point 
    {
        internal Vector3 Vector { get; }
        internal float Scalar { get; }

        protected Point(Point point)
            : this(point.Vector, point.Scalar) { }

        internal Point(Vector3 vector, float scalar)
        {
            this.Vector = vector;
            this.Scalar = scalar;
        }
        public static Point FromPosition(float x, float y, float z)
            => FromPosition(new Vector3(x, y, z));

        public static Point FromPosition(Vector3 position)
            //tex: Point from position $\vec{r}$
            //$$\mathbf{point}=\pmatrix{ \vec{r} \\ 1}$$
            => new Point(position, 1);

        /// <summary>
        /// Point on plane closest to the origin
        /// </summary>
        /// <param name="plane">The plane.</param>
        public static Point FromPlane(Plane plane)
            //tex: Point on plane $W = \pmatrix{\vec{w} & w}$ closest to origin:
            //$$\mathbf{point}=\pmatrix{-w\,\vec{w} \\ \vec{w}\cdot\vec{w} }$$
            => new Point(-plane.Scalar*plane.Vector, plane.Vector.LengthSquared());

        public static Point FromLine(Line line)
            //tex: Point on line $L = \pmatrix{\vec{v} & \vec{m}}$ closest to origin:
            //$$\mathbf{point}=\pmatrix{\vec{v}\times\vec{m} \\ \vec{v}\cdot\vec{v} }$$
            => new Point(Vector3.Cross(line.Vector, line.Moment), line.Vector.LengthSquared());
        

        public static implicit operator Point(Vector3 position)
            => new Point(position, 1);

        public static readonly Point Origin = new Point(Vector3.Zero, 1);
        public static readonly Point InfX = new Point(Vector3.UnitX, 0);
        public static readonly Point InfY = new Point(Vector3.UnitY, 0);
        public static readonly Point InfZ = new Point(Vector3.UnitZ, 0);

        public static Point Meet(Plane plane, Line line)
            //tex: Point where plane $W=\pmatrix{\vec{w} & w}$ and line $L = \pmatrix{\vec{v} & \vec{m}}$ meet:
            //$$\mathbf{point}=\pmatrix{\vec{m}\times\vec{w}+w \,\vec{v} \\ -\vec{w}\cdot\vec{v} }$$
            => new Point(
                Vector3.Cross(line.Moment, plane.Vector)+plane.Scalar*line.Vector,
                -Vector3.Dot(plane.Vector, line.Vector));
        
        public static Point Meet(Plane plane_1, Plane plane_2, Plane plane_3) 
            //tex: Point where three planes meet $W_1$, $W_2$ and $W_3$
            //$$\mathbf{point}( W_1, \; \mathbf{line}(W_2,\,W_3))$$
            => Meet(plane_1, Line.Meet(plane_2, plane_3));

        public float Magnitude { get => Math.Abs(Scalar); }
        public Vector3 Position { get => Vector/Scalar; }
        public float Offset { get => Vector.Length()/Scalar; }
        public float DistanceTo(Point point)
            //tex: Distance from point $G_1=\pmatrix{\vec{g_1} & g_1}$ to point $G_2=\pmatrix{\vec{g_2} & g_2}$
            //$$ \mathrm{dist} = \frac{|g_2 \vec{g}_1 - g_1 \vec{g}_2 |}{g_1 g_2} $$
            => (Scalar*point.Vector - point.Scalar*Vector).Length()/(Scalar*point.Scalar);
        public float DistanceTo(Plane plane)
            //tex: Distance from point $G=\pmatrix{\vec{g_1} & g_1}$ to plane $W=\pmatrix{\vec{w} & w}$
            //$$ \mathrm{dist} = \frac{|\vec{w}\cdot \vec{g} + w\,g |}{g\,|\vec{w}|} $$
            => (Vector3.Dot(plane.Vector, Vector) + Scalar*plane.Scalar)/(Scalar*plane.Vector.Length());

        public float DistanceTo(Line line)
            //tex: Distance from point $G=\pmatrix{\vec{g} & g}$ to line $L=\pmatrix{\vec{v} & \vec{m}}$
            // $$\mathrm{dist} = \frac{\vec{v}\times \vec{g}+g\,\vec{m}}{g\,|\vec{v}|}$$
            => (Vector3.Cross(line.Vector, Vector) + Scalar * line.Moment).Length()/(Scalar*line.Vector.Length());

        public static Point Lerp(Point from, Point to, float amount)
            => new Point((1-amount)*from.Vector + amount*to.Vector,
                (1-amount)*from.Scalar + amount * to.Scalar);

        public static Point Barycentric(Point[] points, float[] weights)
        {
            if (points.Length != weights.Length)
            {
                throw new ArgumentException("Weights must the same count as Points", nameof(weights));
            }
            var vec = Vector3.Zero;
            var scl = 0f;
            for (int i = 0; i < points.Length; i++)
            {
                vec += weights[i] * points[i].Vector;
                scl += weights[i] * points[i].Scalar;
            }
            return new Point(vec, scl);
        }

        public static Point operator + (Point point, Vector3 delta)
            => new Point(point.Vector + point.Scalar*delta, point.Scalar);
        public static Point operator -(Point point, Vector3 delta)
            => new Point(point.Vector - point.Scalar*delta, point.Scalar);

        public static Vector3 operator -(Point point, Point @base)
            => point.Position - @base.Position;

        #region Formatting
        public override string ToString() => ToString("g");
        public string ToString(string formatting) => ToString(formatting, null);
        public string ToString(string format, IFormatProvider provider) 
            => $"Point={Position.ToString(format, provider)}";
        #endregion
    }


}
