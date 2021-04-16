using System;
using System.Numerics;

namespace JA.Geometry
{
    public class Line 
    {
        internal Vector3 Vector { get; }
        internal Vector3 Moment { get; }

        protected Line(Line line)
            : this(line.Vector, line.Moment) { }

        internal Line(Vector3 vector, Vector3 moment)
        {
            this.Vector=vector;
            this.Moment=moment;
        }

        public static readonly Line XAxis = new Line(Vector3.UnitX, Vector3.Zero);
        public static readonly Line YAxis = new Line(Vector3.UnitY, Vector3.Zero);
        public static readonly Line ZAxis = new Line(Vector3.UnitZ, Vector3.Zero);
        public static readonly Line InfXY = new Line(Vector3.Zero, Vector3.UnitZ);
        public static readonly Line InfYZ = new Line(Vector3.Zero, Vector3.UnitX);
        public static readonly Line InfZX = new Line(Vector3.Zero, Vector3.UnitY);

        public static Line Ray(Point point, Vector3 direction)
            //tex: Line from point $G=\pmatrix{\vec{g} & g}$ and direction $\hat{e}$:
            // $$\mathbf{line}=\pmatrix{\hat{e}\,g \\ \vec{g} \times \hat{e}} $$
            => new Line(direction * point.Scalar,
                Vector3.Cross(point.Vector, direction));
        
        public static Line Join(Point point_1, Point point_2)
            //tex: Line jointing two points $G_1= \pmatrix{\vec{g}_1 & g_1}$ and $G_1= \pmatrix{\vec{g}_2 & g_2}$
            // $$\mathbf{line} = \pmatrix{ \vec{g}_2 g_1 - \vec{g}_1 g_2 \\ \vec{g}_1 \times \vec{g}_2 }$$
            => new Line(
                point_2.Vector * point_1.Scalar - point_1.Vector * point_2.Scalar,
                Vector3.Cross(point_1.Vector, point_2.Vector));
        
        public static Line Meet(Plane plane_1, Plane plane_2)
            //tex: Line jointing planes $W_1= \pmatrix{\vec{w}_1 & w_1}$ and $G_1= \pmatrix{\vec{w}_2 & w_2}$
            // $$\mathbf{line} = \pmatrix{ \vec{w}_1 \times \vec{w}_2 \\ \vec{w}_2 w_1 - \vec{w}_1 w_2  }$$
            => new Line(
                Vector3.Cross(plane_1.Vector, plane_2.Vector),
                -plane_1.Vector*plane_2.Scalar+plane_2.Vector*plane_1.Scalar);
        
        public Point Along(float travel)
            => Origin + Direction * travel;

        public float Magnitude { get => Vector.Length(); }
        public Vector3 Direction { get => Vector3.Normalize(Vector); }
        public Point Origin
        {
            get => Point.FromLine(this);
        }
        public float Offset { get => Vector3.Cross(Vector,Moment).Length()/Vector.LengthSquared(); }
        public float DistanceTo(Point point)
            => point.DistanceTo(this);
        public float DistanceTo(Line line)
            //tex: Distance between two lines $L_1=\pmatrix{\vec{v}_1 & \vec{m}_1}$ and $L_2=\pmatrix{\vec{v}_2 & \vec{m}_2}$
            // $$ \mathrm{dist} = \frac{\vec{v}_1 \cdot \vec{m}_2 + \vec{v}_2 \cdot \vec{m}_1}{| \vec{v}_1 \times \vec{v}_2|} $$
            => (Vector3.Dot(Vector, line.Moment) + Vector3.Dot(line.Vector, Moment))/Vector3.Cross(Vector, line.Vector).Length();

        public Point Project(Point point) 
            => Along(Vector3.Dot(Direction, point.Position-Origin.Position));

        public Point Project(Line line)
        {
            var norm = Vector3.Cross(Vector, line.Vector);
            var k = Vector3.Cross(line.Vector, norm);
            var plane = Plane.FromPointAndNormal(line.Origin, k);
            return Point.Meet(plane, this);
        }

        public static Line Lerp(Line from, Line to, float amount)
            => new Line((1-amount)*from.Vector + amount*to.Vector,
                (1-amount)*from.Moment + amount * to.Moment);

        public static Line Barycentric(Line[] points, float[] weights)
        {
            if (points.Length != weights.Length)
            {
                throw new ArgumentException("Weights must the same count as Lines", nameof(weights));
            }
            var vec = Vector3.Zero;
            var mom = Vector3.Zero;
            for (int i = 0; i < points.Length; i++)
            {
                vec += weights[i] * points[i].Vector;
                mom += weights[i] * points[i].Moment;
            }
            return new Line(vec, mom);
        }


        #region Formatting
        public override string ToString() => ToString("g");
        public string ToString(string formatting) => ToString(formatting, null);
        public string ToString(string format, IFormatProvider provider)
            => $"Direction={Direction.ToString(format, provider)} Point={Origin.Position.ToString(format, provider)}";
        #endregion
    }


}
