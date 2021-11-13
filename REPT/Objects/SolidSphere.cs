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
    public class SolidSphere : Explicit
    {
        private double Radius = 1;

        public SolidSphere(Vector3 position, double radius)
        {
            Radius = radius;
        }

        public override double At(double p0, double p1, double p2)
        {
            Vector3 pos = new Vector3(p0, p1, p2, Vector3.VectorForm.POSITION);
            double r = pos.Length();
            if (r < Radius)
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
            Vector3 pos = new Vector3(p0, p1, p2, Vector3.VectorForm.POSITION);
            double r = pos.Length();
            if (r < Radius)
            {
                return (new Vector3(pos).Normalize());
            }
            else
            {
                return (new Vector3(pos).Normalize());
                //return Vector3.Zero();
            }
        }
    }
}
