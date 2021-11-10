using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirali.MathR
{
    /// <summary>
    /// <tooltip>Vector with three-basis componence, in most cases x, y, and z. Can be used to store information on points and planes as well.</tooltip>
    /// </summary>
    public class Vector3
    {
        private double _x = 0.0;
        private double _y = 0.0;
        private double _z = 0.0;

        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Z { get { return _z; } set { _z = value; } }

        public enum VectorForm
        {
            VECTOR,
            DIRECTION,
            POSITION,
            NORMAL,
            PLANE
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
        /// <param name="z"></param>
        /// <returns></returns>
        public Vector3 Set(double x, double y, double z)
        {
            switch (Form)
            {
                case VectorForm.DIRECTION:
                    double l = Length(x, y, z);
                    X = x/l;
                    Y = y/l;
                    Z = z/l;
                    break;
                default:
                    X = x;
                    Y = y;
                    Z = z;
                    break;
            }
            return this;
        }

        /// <summary>
        /// <tooltip>Sets the vector values to another vector. Use this for DIRECTION type to auto-normalize.</tooltip>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector3 Set(Vector3 vector)
        {
            switch (Form)
            {
                case VectorForm.DIRECTION:
                    double l = Length(vector);
                    X = vector.X / l;
                    Y = vector.Y / l;
                    Z = vector.Z / l;
                    break;
                default:
                    X = vector.X;
                    Y = vector.Y;
                    Z = vector.Z;
                    break;
            }
            return this;
        }

        /// <summary>
        /// <tooltip>Returns a Vector3 object with zero length.</tooltip>
        /// </summary>
        /// <returns></returns>
        public static Vector3 Zero()
        {
            return new Vector3(0, 0, 0);
        }

        /// <summary>
        /// <tooltip>Create a new empty 3D Vector with space for x, y, and z coodrinate values.</tooltip>
        /// </summary>
        public Vector3() { }

        /// <summary>
        /// <tooltip>Create a new 3D Vector with space for x, y, and z coodrinate values.</tooltip>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3(double x, double y, double z)
        {
            Set(x, y, z);
        }

        /// <summary>
        /// <tooltip>Creates a new vector object from a vector.</tooltip>
        /// </summary>
        /// <param name="vector"></param>
        public Vector3(Vector3 vector)
        {
            Set(vector);
        }

        /// <summary>
        /// <tooltip>Creates a new vector object from a vector, specifies purpose of vector object.</tooltip>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="vf"></param>
        public Vector3(double x, double y, double z, VectorForm vf)
        {
            X = x;
            Y = y;
            Z = z;
            Form = vf;
        }

        #endregion VecConstruction

        #region VecOperations
        /// <summary>
        /// <tooltip>Scales the vector coordinate values to a total vector unit lengh of 1.</tooltip>
        /// </summary>
        /// <returns></returns>
        public Vector3 Normalize()
        {
            double m = 1.0 / Math.Sqrt(X * X + Y * Y + Z * Z);
            X *= m;
            Y *= m;
            Z *= m;
            return this;
        }

        /// <summary>
        /// <tooltip>Returns the length of the Vector.</tooltip>
        /// </summary>
        /// <returns></returns>
        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        /// <summary>
        /// <tooltip>Returns the length of the Vector.</tooltip>
        /// </summary>
        /// <returns></returns>
        public static double Length(Vector3 vector)
        {
            return Length(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// <tooltip>Returns the length of the combined vector components.</tooltip>
        /// </summary>
        /// <returns></returns>
        public static double Length(double x, double y, double z)
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        /// <summary>
        /// <tooltip>Returns the distance/difference (double) given two vectors.</tooltip>
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Distance(Vector3 p1, Vector3 p2)
        {
            return (p2 - p1).Length();
        }

        /// <summary>
        /// <tooltip>Add another vector's components to the vector</tooltip>
        /// </summary>
        /// <param name="vector"></param>
        public Vector3 Add(Vector3 vector)
        {
            Set(this + vector);
            return this;
        }

        /// <summary>
        /// <tooltip>Add double value to vector coordinates. Previously the Skew function.</tooltip>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3 Add(double x, double y, double z)
        {
            Set(X + x, Y + y, Z + z);
            return this;
        }

        /// <summary>
        /// <tooltip>Flips orientation of the vector. Previously Inverse.</tooltip>
        /// </summary>
        /// <returns></returns>
        public Vector3 Negate()
        {
            X *= -1;
            Y *= -1;
            Z *= -1;
            return this;
        }

        /// <summary>
        /// <tooltip>Returns string representation of the vector.</tooltip>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Vector: < " + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + " >";
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
            z1 = Math.Round(Z, decimalCutoff);

            return "Vector: < " + x1.ToString() + ", " + y1.ToString() + ", " + z1.ToString() + " >";
        }

        //CONVERSION FORMS
        public static explicit operator Matrix(Vector3 p) => p.ToMatrix();

        /// <summary>
        /// <tooltip>Converts a Vector3 object into a 1x3 matrix.</tooltip>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Matrix ToMatrix()
        {
            Matrix res = new Matrix(1, 3);
            res.Set(0, 0, X);
            res.Set(0, 1, Y);
            res.Set(0, 2, Z);
            return res;
        }

        /// <summary>
        /// <tooltip>Converts a Vector3 object into a 1x3 matrix.</tooltip>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Matrix ToMatrix(Vector3 vector)
        {
            Matrix res = new Matrix(1, 3);
            res.Set(0, 0, vector.X);
            res.Set(0, 1, vector.Y);
            res.Set(0, 2, vector.Z);
            return res;
        }

        //GENERIC OPERANDS

        /// <summary>
        /// <tooltip>Returns the scalar dot-product between two vectors.</tooltip>
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double Dot(Vector3 v1, Vector3 v2)
        {
            return (v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z);
        }

        /// <summary>
        /// <tooltip>Returns the vector cross-product between two vectors.</tooltip>
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            double xr = (v1.Y * v2.Z) - (v1.Z * v2.Y);
            double yr = (v1.Z * v2.X) - (v1.X * v2.Z);
            double zr = (v1.X * v2.Y) - (v1.Y * v2.X);
            return new Vector3(xr, yr, zr);
        }

        /// <summary>
        /// <tooltip>Returns the angle in radians between two given vectors using the Dot-Product relation.</tooltip>
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double Between(Vector3 v1, Vector3 v2)
        {
            double lc = v1.Length() * v2.Length();
            double angle = Math.Acos(Dot(v1, v2) / lc);
            return angle;
        }

        /// <summary>
        /// <tooltip>Returns a vector of the same magnitude as the given vector, bounced off a surface with a given normal vector.</tooltip>
        /// </summary>
        /// <param name="incident"><tooltip>Vector of incident ray.</tooltip></param>
        /// <param name="normal"><tooltip>Normal Vector of incident surface.</tooltip></param>
        /// <returns></returns>
        public static Vector3 Bounce(Vector3 incident, Vector3 normal)
        {
            Matrix rot = Matrix.RotationU(normal, Math.PI);
            Vector3 resultant = (incident.ToMatrix().Flip() * rot).ToVector3();
            return resultant;
        }

        /// <summary>
        /// <tooltip>Returns a vector that has been refracted through a surface of given normal.</tooltip>
        /// </summary>
        /// <param name="incident"><tooltip>Vector of incident ray.</tooltip></param>
        /// <param name="normal"><tooltip>Normal Vector of incident surface.</tooltip></param>
        /// <param name="n1"><tooltip>Refraction index of previous medium.</tooltip></param>
        /// <param name="n2"><tooltip>Refraction index of entered medium.</tooltip></param>
        /// <returns></returns>
        public static Vector3 Refract(Vector3 incident, Vector3 normal, double n1, double n2)
        {
            try
            {
                Vector3 Uaxi = Cross(incident, normal).Normalize();
                Console.WriteLine(Uaxi.ToString());
                double Ainc = Between(new Vector3(normal).Negate(), incident);
                if (Ainc > Math.PI / 2) { Ainc = Between(normal, incident); }

                double outangle = Ainc - Math.Asin(n1/n2 * Math.Sin(Ainc));

                Matrix rot = Matrix.RotationU(Uaxi, outangle);
                Vector3 resultant = (incident.ToMatrix().Flip() * rot).ToVector3();
                return resultant;
            }
            catch
            {
                return Vector3.Zero();
            }
        }

        //vector and scalar mult. 
        public static Vector3 operator *(double scalar, Vector3 vec)
        {
            return new Vector3(vec.X * scalar, vec.Y * scalar, vec.Z * scalar);
        }
        public static Vector3 operator *(Vector3 vec, double scalar)
        {
            return new Vector3(vec.X * scalar, vec.Y * scalar, vec.Z * scalar);
        }

        //two vectors
        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        #endregion VecOperations



    }
}
