using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.MathR;


namespace REPT.Objects
{
    public class LinearFalloff : Explicit
    {
        public override double At(double p0, double p1 = 0, double p2 = 0)
        {
            double x = p0;
            if (x > 0)
            {
                return (Math.Pow(Math.E, -(x) / 2) / 2 + 1);
            }
            else
            {
                return (Math.Pow(Math.E, (x) / 2) / 2 + 1);

            }
        }

        public override Vector3 Grad(double p0, double p1 = 0, double p2 = 0)
        {
            double x = p0;
            if (x > 1)
            {
                return new Vector3(-1, 0, 0);
            }
            else
            {
                return new Vector3(-1, 0, 0);
            }
        }
    }

    public class SphereFalloff : Explicit
    {
        //this sphere-falloff equation will be Spherical in nature, however symmetric in phi and theta
        //ill use xyz for the sake of making things easier later though.


        public override double At(double p0, double p1, double p2)
        {
            Vector3 pos = new Vector3(p0, p1, p2, Vector3.VectorForm.POSITION);
            double r = pos.Length();
            if (true)
            {
                return Math.Pow(Math.E, -(r / 3)) / 2 + 1;
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
            if (r > 1)
            {
                return (new Vector3(pos).Normalize());
            }
            else
            {
                return (new Vector3(pos).Normalize());
            }
        }


    }
}
