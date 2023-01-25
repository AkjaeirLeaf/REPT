using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirali.MathR
{
    public class Quaternion
    {
        private double _w = 0.0;
        private double _x = 0.0;
        private double _y = 0.0;
        private double _z = 0.0;

        public double W { get { return _w; } set { _w = value; } }
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public double Z { get { return _z; } set { _z = value; } }

        public static Quaternion ZERO { get { return new Quaternion(); } }

        // CONSTRUCTORS
        public Quaternion()
        {
            _w = 0.0;
            _x = 0.0;
            _y = 0.0;
            _z = 0.0;
        }
        public Quaternion(Quaternion q)
        {
            _w = q._w;
            _x = q._x;
            _y = q._y;
            _z = q._z;
        }
        public Quaternion Copy() { return (Quaternion)MemberwiseClone(); }
        public Quaternion(double w, double x, double y, double z)
        {
            _w = w;
            _x = x;
            _y = y;
            _z = z;
        }
        public Quaternion(double w, Vector3 v)
        {
            W = w; X = v.X; Y = v.Y; Z = v.Z;
        }
        public Quaternion(Vector3 axis, double angleRadian)
        {
            double m = axis.Length();
            if (m > 0.0001)
            {
                double ca = Math.Cos(angleRadian / 2);
                double sa = Math.Sin(angleRadian / 2);
                X = axis.X / m * sa;
                Y = axis.Y / m * sa;
                Z = axis.Z / m * sa;
                W = ca;
            }
            else
            {
                W = 1; X = 0; Y = 0; Z = 0;
            }
        }
        public static Quaternion Identity
        {
            get { return new Quaternion(0, 0, 0, 1); }
        }
        public Quaternion(double yaw, double pitch, double roll)
        {
            //  Roll first, about axis the object is facing, then
            //  pitch upward, then yaw to face into the new heading
            double sr, cr, sp, cp, sy, cy;

            double halfRoll = roll * 0.5f;
            sr = (float)Math.Sin(halfRoll);
            cr = (float)Math.Cos(halfRoll);

            double halfPitch = pitch * 0.5f;
            sp = (float)Math.Sin(halfPitch);
            cp = (float)Math.Cos(halfPitch);

            double halfYaw = yaw * 0.5f;
            sy = (float)Math.Sin(halfYaw);
            cy = (float)Math.Cos(halfYaw);

            Quaternion result = new Quaternion();

            X = cy * sp * cr + sy * cp * sr;
            Y = sy * cp * cr - cy * sp * sr;
            Z = cy * cp * sr - sy * sp * cr;
            W = cy * cp * cr + sy * sp * sr;
        }
        public static Quaternion FromYawPitchRoll(double yaw, double pitch, double roll)
        {
            //  Roll first, about axis the object is facing, then
            //  pitch upward, then yaw to face into the new heading
            double sr, cr, sp, cp, sy, cy;

            double halfRoll = roll * 0.5f;
            sr = (float)Math.Sin(halfRoll);
            cr = (float)Math.Cos(halfRoll);

            double halfPitch = pitch * 0.5f;
            sp = (float)Math.Sin(halfPitch);
            cp = (float)Math.Cos(halfPitch);

            double halfYaw = yaw * 0.5f;
            sy = (float)Math.Sin(halfYaw);
            cy = (float)Math.Cos(halfYaw);

            Quaternion result = new Quaternion();

            result.X = cy * sp * cr + sy * cp * sr;
            result.Y = sy * cp * cr - cy * sp * sr;
            result.Z = cy * cp * sr - sy * sp * cr;
            result.W = cy * cp * cr + sy * sp * sr;

            return result;
        }

        // CALCULATIONS
        public double Magnitude
        {
            get
            {
                return Math.Sqrt(_w * _w + _x * _x + _y * _y + _z * _z);
            }
        }
        public double MagnitudeSquared()
        {
            return X * X + Y * Y + Z * Z + W * W;
        }
        
        public Quaternion SafeNormalize()
        {
            Quaternion r = new Quaternion();
            double d = Magnitude;
            r.W += W / d;
            r.X += X / d;
            r.Y += Y / d;
            r.Z += Z / d;

            return r;
        }
        public void Normalize()
        {
            double d = Magnitude;
            W = W / d;
            X = X / d;
            Y = Y / d;
            Z = Z / d;
        }
        public Quaternion Inverse()
        {
            Quaternion r = new Quaternion();
            double d = W * W + X * X + Y * Y + Z * Z;
            r.W = W / d;
            r.X = -1 * X / d;
            r.Y = -1 * Y / d;
            r.Z = -1 * Z / d;

            return r;
        }
        public Quaternion Conjugate()//idk if same as inv or whatever
        {
            Quaternion r = new Quaternion();
            double d = W * W + X * X + Y * Y + Z * Z;
            r.W = W;
            r.X = -1 * X;
            r.Y = -1 * Y;
            r.Z = -1 * Z;

            return r;
        }
        
        public void RotatePoints(Vector3 pt)
        {
            this.Normalize();
            Quaternion q1 = this.Copy();
            q1.Conjugate();

            Quaternion qNode = new Quaternion(0, pt.X, pt.Y, pt.Z);
            qNode = this * qNode * q1;
            pt.X = qNode.X;
            pt.Y = qNode.Y;
            pt.Z = qNode.Z;
        }
        public void RotatePoints(Vector3[] nodes)
        {
            this.Normalize();
            Quaternion q1 = this.Copy();
            q1.Conjugate();
            for (int i = 0; i < nodes.Length; i++)
            {
                Quaternion qNode = new Quaternion(0, nodes[i].X, nodes[i].Y, nodes[i].Z);
                qNode = this * qNode * q1;
                nodes[i].X = qNode.X;
                nodes[i].Y = qNode.Y;
                nodes[i].Z = qNode.Z;
            }
        }
        public static double Dot(Quaternion quaternion1, Quaternion quaternion2)
        {
            return quaternion1.X * quaternion2.X +
                   quaternion1.Y * quaternion2.Y +
                   quaternion1.Z * quaternion2.Z +
                   quaternion1.W * quaternion2.W;
        }
        public static Quaternion Concatenate(Quaternion value1, Quaternion value2)
        {
            Quaternion ans =  new Quaternion();

            // Concatenate rotation is actually q2 * q1 instead of q1 * q2.
            // So that's why value2 goes q1 and value1 goes q2.
            double q1x = value2.X;
            double q1y = value2.Y;
            double q1z = value2.Z;
            double q1w = value2.W;

            double q2x = value1.X;
            double q2y = value1.Y;
            double q2z = value1.Z;
            double q2w = value1.W;

            // cross(av, bv)
            double cx = q1y * q2z - q1z * q2y;
            double cy = q1z * q2x - q1x * q2z;
            double cz = q1x * q2y - q1y * q2x;

            double dot = q1x * q2x + q1y * q2y + q1z * q2z;

            ans.X = q1x * q2w + q2x * q1w + cx;
            ans.Y = q1y * q2w + q2y * q1w + cy;
            ans.Z = q1z * q2w + q2z * q1w + cz;
            ans.W = q1w * q2w - dot;

            return ans;
        }
        public static Quaternion Negate(Quaternion value)
        {
            Quaternion ans =  new Quaternion();

            ans.X = -value.X;
            ans.Y = -value.Y;
            ans.Z = -value.Z;
            ans.W = -value.W;

            return ans;
        }
        public static Quaternion Add(Quaternion value1, Quaternion value2)
        {
            Quaternion ans = new Quaternion();

            ans.X = value1.X + value2.X;
            ans.Y = value1.Y + value2.Y;
            ans.Z = value1.Z + value2.Z;
            ans.W = value1.W + value2.W;

            return ans;
        }
        public static Quaternion Subtract(Quaternion value1, Quaternion value2)
        {
            Quaternion ans = new Quaternion();

            ans.X = value1.X - value2.X;
            ans.Y = value1.Y - value2.Y;
            ans.Z = value1.Z - value2.Z;
            ans.W = value1.W - value2.W;

            return ans;
        }
        public static Quaternion Multiply(Quaternion value1, Quaternion value2)
        {
            Quaternion ans =  new Quaternion();

            double q1x = value1.X;
            double q1y = value1.Y;
            double q1z = value1.Z;
            double q1w = value1.W;

            double q2x = value2.X;
            double q2y = value2.Y;
            double q2z = value2.Z;
            double q2w = value2.W;

            // cross(av, bv)
            double cx = q1y * q2z - q1z * q2y;
            double cy = q1z * q2x - q1x * q2z;
            double cz = q1x * q2y - q1y * q2x;

            double dot = q1x * q2x + q1y * q2y + q1z * q2z;

            ans.X = q1x * q2w + q2x * q1w + cx;
            ans.Y = q1y * q2w + q2y * q1w + cy;
            ans.Z = q1z * q2w + q2z * q1w + cz;
            ans.W = q1w * q2w - dot;

            return ans;
        }
        public static Quaternion Multiply(Quaternion value1, double value2)
        {
            Quaternion ans = new Quaternion();

            ans.X = value1.X * value2;
            ans.Y = value1.Y * value2;
            ans.Z = value1.Z * value2;
            ans.W = value1.W * value2;

            return ans;
        }
        public static Quaternion Divide(Quaternion value1, Quaternion value2)
        {
            Quaternion ans = new Quaternion();

            double q1x = value1.X;
            double q1y = value1.Y;
            double q1z = value1.Z;
            double q1w = value1.W;

            //-------------------------------------
            // Inverse part.
            double ls = value2.X * value2.X + value2.Y * value2.Y +
                       value2.Z * value2.Z + value2.W * value2.W;
            double invNorm = 1.0f / ls;

            double q2x = -value2.X * invNorm;
            double q2y = -value2.Y * invNorm;
            double q2z = -value2.Z * invNorm;
            double q2w = value2.W * invNorm;

            //-------------------------------------
            // Multiply part.

            // cross(av, bv)
            double cx = q1y * q2z - q1z * q2y;
            double cy = q1z * q2x - q1x * q2z;
            double cz = q1x * q2y - q1y * q2x;

            double dot = q1x * q2x + q1y * q2y + q1z * q2z;

            ans.X = q1x * q2w + q2x * q1w + cx;
            ans.Y = q1y * q2w + q2y * q1w + cy;
            ans.Z = q1z * q2w + q2z * q1w + cz;
            ans.W = q1w * q2w - dot;

            return ans;
        }

        // INTERPOLATION
        public static Quaternion Slerp(Quaternion quaternion1, Quaternion quaternion2, double amount)
        {
            const double epsilon = 1e-6f;

            double t = amount;

            double cosOmega = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y +
                             quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;

            bool flip = false;

            if (cosOmega < 0.0f)
            {
                flip = true;
                cosOmega = -cosOmega;
            }

            double s1, s2;

            if (cosOmega > (1.0f - epsilon))
            {
                // Too close, do straight linear interpolation.
                s1 = 1.0f - t;
                s2 = (flip) ? -t : t;
            }
            else
            {
                double omega = Math.Acos(cosOmega);
                double invSinOmega = (1 / Math.Sin(omega));

                s1 = Math.Sin((1.0f - t) * omega) * invSinOmega;
                s2 = (flip)
                    ? -Math.Sin(t * omega) * invSinOmega
                    : Math.Sin(t * omega) * invSinOmega;
            }

            Quaternion ans = new Quaternion();

            ans.X = s1 * quaternion1.X + s2 * quaternion2.X;
            ans.Y = s1 * quaternion1.Y + s2 * quaternion2.Y;
            ans.Z = s1 * quaternion1.Z + s2 * quaternion2.Z;
            ans.W = s1 * quaternion1.W + s2 * quaternion2.W;

            return ans;
        }
        public static Quaternion Lerp(Quaternion quaternion1, Quaternion quaternion2, double amount)
        {
            double t = amount;
            double t1 = 1.0f - t;

            Quaternion r = new Quaternion();

            double dot = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y +
                         quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;

            if (dot >= 0.0f)
            {
                r.X = t1 * quaternion1.X + t * quaternion2.X;
                r.Y = t1 * quaternion1.Y + t * quaternion2.Y;
                r.Z = t1 * quaternion1.Z + t * quaternion2.Z;
                r.W = t1 * quaternion1.W + t * quaternion2.W;
            }
            else
            {
                r.X = t1 * quaternion1.X - t * quaternion2.X;
                r.Y = t1 * quaternion1.Y - t * quaternion2.Y;
                r.Z = t1 * quaternion1.Z - t * quaternion2.Z;
                r.W = t1 * quaternion1.W - t * quaternion2.W;
            }

            // Normalize it.
            double ls = r.X * r.X + r.Y * r.Y + r.Z * r.Z + r.W * r.W;
            double invNorm = 1.0f / Math.Sqrt(ls);

            r.X *= invNorm;
            r.Y *= invNorm;
            r.Z *= invNorm;
            r.W *= invNorm;

            return r;
        }

        // BOOLEAN
        public bool IsIdentity
        {
            get { return X == 0f && Y == 0f && Z == 0f && W == 1f; }
        }

        // OVERRIDES
        public override string ToString()
        {
            return _w.ToString() + " + " + _x.ToString() + "i + " + _y.ToString() + "j + " + _z.ToString() + "k";
        }
        public static Quaternion operator +(Quaternion q1, Quaternion q2)
        {
            return Quaternion.Add(q1, q2);
        }
        public static Quaternion operator -(Quaternion q1, Quaternion q2)
        {
            return Quaternion.Subtract(q1, q2);
        }
        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            return Quaternion.Multiply(q1, q2);
        }
        public static Quaternion operator *(Quaternion q1, double d)
        {
            return Quaternion.Multiply(q1, d);
        }
        public static Quaternion operator /(Quaternion q1, Quaternion q2)
        {
            return Quaternion.Divide(q1, q2);
        }
        public static Quaternion operator -(Quaternion value)
        {
            Quaternion ans = new Quaternion();

            ans.X = -value.X;
            ans.Y = -value.Y;
            ans.Z = -value.Z;
            ans.W = -value.W;

            return ans;
        }
        public static bool operator ==(Quaternion value1, Quaternion value2)
        {
            return (value1.X == value2.X &&
                    value1.Y == value2.Y &&
                    value1.Z == value2.Z &&
                    value1.W == value2.W);
        }
        public static bool operator !=(Quaternion value1, Quaternion value2)
        {
            return (value1.X != value2.X ||
                    value1.Y != value2.Y ||
                    value1.Z != value2.Z ||
                    value1.W != value2.W);
        }
        public bool Equals(Quaternion other)
        {
            return (X == other.X &&
                    Y == other.Y &&
                    Z == other.Z &&
                    W == other.W);
        }
        public override bool Equals(object obj)
        {
            if (obj is Quaternion)
            {
                return Equals((Quaternion)obj);
            }

            return false;
        }
        public override int GetHashCode()
        {
            return W.GetHashCode() + X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
        }

    }
}
