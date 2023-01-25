using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirali.MathR
{
    /// <summary>
    /// <tooltip>Vector with two-basis componence, in most cases x, y, and z. Can be used to store information on points and planes as well.</tooltip>
    /// </summary>
    public class Vector2
    {
        private double _x = 0.0;
        private double _y = 0.0;

        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }

        public enum VectorForm
        {
            VECTOR,
            DIRECTION,
            POSITION,
            NORMAL,
            PLANE,
            INFINITY //only for non-hit calculations
        }
        /// <summary>
        /// <tooltip>
        /// <para>Specifies use of the Vector3 Object in question.</para>
        /// <para>VECTOR: A generic vector with direction and magnitude.</para>
        /// <para>DIRECTION: Vector with no magnitude, only direction. Please use the SET() function to assure normalization.</para>
        /// <para>POSITION: A Point rather than vector. </para>
        /// </tooltip>
        /// </summary>
        public VectorForm Form = VectorForm.VECTOR;

        #region VecConstruction
        /// <summary>
        /// <tooltip>Sets the component values of the Vector. Use this for DIRECTION type to auto-normalize.</tooltip>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector2 Set(double x, double y)
        {
            switch (Form)
            {
                case VectorForm.DIRECTION:
                    double l = Length(x, y);
                    X = x / l;
                    Y = y / l;
                    break;
                default:
                    X = x;
                    Y = y;
                    break;
            }
            return this;
        }

        /// <summary>
        /// <tooltip>Sets the vector values to another vector. Use this for DIRECTION type to auto-normalize.</tooltip>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector2 Set(Vector2 vector)
        {
            switch (Form)
            {
                case VectorForm.DIRECTION:
                    double l = Length(vector);
                    X = vector.X / l;
                    Y = vector.Y / l;
                    break;
                default:
                    X = vector.X;
                    Y = vector.Y;
                    break;
            }
            return this;
        }

        /// <summary>
        /// <tooltip>Returns a Vector3 object with zero length.</tooltip>
        /// </summary>
        /// <returns></returns>
        public static Vector2 Zero { get { return new Vector2(0.0, 0.0); } }

        /// <summary>
        /// <tooltip>Create a new empty 3D Vector with space for x, y, and z coodrinate values.</tooltip>
        /// </summary>
        public Vector2()
        {
            X = 0;
            Y = 0;
        }

        /// <summary>
        /// <tooltip>Create a new 3D Vector with space for x, y, and z coodrinate values.</tooltip>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector2(double x, double y)
        {
            Set(x, y);
        }

        public static Vector2 Mix(Vector2 v1, Vector2 v2, double factor)
        {
            Vector2 vec = new Vector2();

            vec.X = factor * (v2.X - v1.X) + v1.X;
            vec.Y = factor * (v2.Y - v1.Y) + v1.Y;

            return vec;
        }

        /// <summary>
        /// <tooltip>Creates a new vector object from a vector.</tooltip>
        /// </summary>
        /// <param name="vector"></param>
        public Vector2(Vector2 vector)
        {
            Set(vector);
        }

        /// <summary>
        /// <tooltip>Creates a new vector object from a vector, specifies purpose of vector object.</tooltip>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="vf"></param>
        public Vector2(double x, double y, VectorForm vf)
        {
            X = x;
            Y = y;
            Form = vf;
        }

        public static Vector2 Xaxis { get { return new Vector2(1.0, 0.0, VectorForm.DIRECTION); } }
        public static Vector2 Yaxis { get { return new Vector2(0.0, 1.0, VectorForm.DIRECTION); } }

        #endregion VecConstruction

        #region VecOperations
        /// <summary>
        /// <tooltip>Scales the vector coordinate values to a total vector unit lengh of 1.</tooltip>
        /// </summary>
        /// <returns></returns>
        public Vector2 Normalize()
        {
            double m = 1.0 / Math.Sqrt(X * X + Y * Y);
            X *= m;
            Y *= m;
            return this;
        }

        /// <summary>
        /// <tooltip>Returns the length of the Vector.</tooltip>
        /// </summary>
        /// <returns></returns>
        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        /// <summary>
        /// <tooltip>Returns the length of the Vector.</tooltip>
        /// </summary>
        /// <returns></returns>
        public static double Length(Vector2 vector)
        {
            return Length(vector.X, vector.Y);
        }

        /// <summary>
        /// <tooltip>Returns the length of the combined vector components.</tooltip>
        /// </summary>
        /// <returns></returns>
        public static double Length(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        /// <summary>
        /// <tooltip>Returns the distance/difference (double) given two vectors.</tooltip>
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Distance(Vector2 p1, Vector2 p2)
        {
            return (p2 - p1).Length();
        }

        public static Vector2 Average(Vector2[] array)
        {
            int total = 0;

            double xsum = 0;
            double ysum = 0;
            for (int inc = 0; inc < array.Length; inc++)
            {
                if (array[inc].Form != VectorForm.INFINITY)
                {
                    xsum += array[inc].X;
                    ysum += array[inc].Y;
                    total++;
                }
            }
            if (total > 0)
            {
                xsum /= total;
                ysum /= total;

                return new Vector2(xsum, ysum);
            }
            else
            {
                return INFINITY;
            }
        }

        public static Vector2 INFINITY { get { return new Vector2(0.0, 0.0, VectorForm.INFINITY); } }

        
        /// <summary>
        /// <tooltip>Add another vector's components to the vector</tooltip>
        /// </summary>
        /// <param name="vector"></param>
        public Vector2 Add(Vector2 vector)
        {
            Set(this + vector);
            return this;
        }

        /// <summary>
        /// <tooltip>Add double value to vector coordinates. Previously the Skew function.</tooltip>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2 Add(double x, double y)
        {
            Set(X + x, Y + y);
            return this;
        }

        /// <summary>
        /// <tooltip>Flips orientation of the vector. Previously Inverse.</tooltip>
        /// </summary>
        /// <returns></returns>
        public Vector2 Negate()
        {
            X *= -1;
            Y *= -1;
            return this;
        }

        /// <summary>
        /// <tooltip>Returns string representation of the vector.</tooltip>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "< " + X.ToString() + ", " + Y.ToString() + " >";
        }

        /// <summary>
        /// <tooltip>Returns string representation of the vector but limits length of display.</tooltip>
        /// </summary>
        /// <param name="decimalCutoff"></param>
        /// <returns></returns>
        public string ToString(int decimalCutoff)
        {
            double x1, y1, z1;
            x1 = Math.Round(X, decimalCutoff);
            y1 = Math.Round(Y, decimalCutoff);

            return "< " + x1.ToString() + ", " + y1.ToString() + " >";
        }

        //CONVERSION FORMS
        public static explicit operator string(Vector2 p) => p.ToString();
        public static explicit operator Matrix(Vector2 p) => p.ToMatrix();

        /// <summary>
        /// <tooltip>Converts a Vector3 object into a 1x3 matrix.</tooltip>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Matrix ToMatrix()
        {
            Matrix res = new Matrix(1, 2);
            res.Set(0, 0, X);
            res.Set(0, 1, Y);
            return res;
        }

        /// <summary>
        /// <tooltip>Converts a Vector3 object into a 1x3 matrix.</tooltip>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Matrix ToMatrix(Vector2 vector)
        {
            Matrix res = new Matrix(1, 2);
            res.Set(0, 0, vector.X);
            res.Set(0, 1, vector.Y);
            return res;
        }

        //GENERIC OPERANDS

        /// <summary>
        /// <tooltip>Returns the scalar dot-product between two vectors.</tooltip>
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double Dot(Vector2 v1, Vector2 v2)
        {
            return (v1.X * v2.X) + (v1.Y * v2.Y);
        }

        /// <summary>
        /// <tooltip>Returns the angle in radians between two given vectors using the Dot-Product relation.</tooltip>
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double Between(Vector2 v1, Vector2 v2)
        {
            double lc = v1.Length() * v2.Length();
            double angle = Math.Acos(Dot(v1, v2) / lc);
            return angle;
        }

        public static double LesserBetween(Vector2 v1, Vector2 v2)
        {
            double lc = v1.Length() * v2.Length();
            double angle1 = Math.Acos(Dot(v1, v2) / lc);
            double angle2 = Math.Acos(Dot(v1.Negate(), v2) / lc);
            if (angle1 > angle2)
            {
                return angle2;
            }
            else
            {
                return angle1;
            }
        }

        public Vector2 GetClose(Vector2 init, Vector2 target)
        {
            Vector2 dto = target - init; double d = dto.Length();
            dto *= 1.0 / d; dto.Form = VectorForm.DIRECTION;
            Vector2 the = new Vector2(this).Normalize();
            double angle = Math.Acos(Dot(dto, the));
            double range = d * Math.Cos(angle);
            Vector2 Closest = init + the * range;
            return Closest;
        }

        /// <summary>
        /// <tooltip>Returns the point closest to a given target point when following a ray in the given direction.</tooltip>
        /// </summary>
        /// <param name="pointing"></param>
        /// <param name="init"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector2 GetClose(Vector2 pointing, Vector2 init, Vector2 target)
        {
            Vector2 dto = target - init; double d = dto.Length();
            dto *= 1.0 / d; dto.Form = VectorForm.DIRECTION;
            Vector2 the = new Vector2(pointing).Normalize();
            double angle = Math.Acos(Dot(dto, the));
            double range = d * Math.Cos(angle);
            Vector2 Closest = init + the * range;
            return Closest;
        }

        public static Vector2 Rotate(Vector2 vector, double angle)
        {
            Matrix rot = Matrix.Rotation2D(angle);
            return (vector.ToMatrix().Flip() * rot).ToVector2();
        }

       

        //vector and scalar mult. 
        public static Vector2 operator *(double scalar, Vector2 vec)
        {
            return new Vector2(vec.X * scalar, vec.Y * scalar);
        }
        public static Vector2 operator *(Vector2 vec, double scalar)
        {
            return new Vector2(vec.X * scalar, vec.Y * scalar);
        }

        //two vectors
        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2 operator /(double d, Vector2 v1)
        {
            return new Vector2(d / v1.X, d / v1.Y);
        }

        public static bool IsEqual(Vector2 v1, Vector2 v2)
        {
            if (v1.X == v2.X && v1.Y == v2.Y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion VecOperations


        #region VecSpecial



        #endregion VecSpecial
    }
}
