using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.MathR;
using Kirali.Light;
using Kirali.Celestials;

namespace REPT.Objects
{
    public class SolidCube : Explicit
    {
        private double SL = 1.0;
        private Vector3 position = new Vector3(Vector3.Zero());

        public SolidCube(Vector3 Position, double side)
        {
            position = new Vector3(Position);
            SL = side;
        }

        public override double At(double p0, double p1 = 0, double p2 = 0)
        {
            double s2 = SL / 2;
            if    (p0 <= s2 + position.X && -s2 + position.X <= p0 
                && p1 <= s2 + position.Y && -s2 + position.Y <= p1 
                && p2 <= s2 + position.Z && -s2 + position.Z <= p2)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        public override Vector3 Grad(double p0, double p1, double p2)
        {
            if (p0 * p0 > p1 * p1 && p0 * p0 > p2 * p2)
            {
                return (new Vector3(p0, 0, 0).Normalize());
            } else if (p1 * p1 > p2 * p2) {
                return (new Vector3(0, p1, 0).Normalize());
            } else
            {
                return (new Vector3(0, 0, p2).Normalize());
            }
        }
    }
}
