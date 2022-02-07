using System;
using System.Collections.Generic;
using System.Text;

using Kirali.MathR;

namespace KiraliConsole
{
    public class SphereFalloff : Explicit
    {
        //this sphere-falloff equation will be Spherical in nature, however symmetric in phi and theta
        //ill use xyz for the sake of making things easier later though.


        public override double At(double p0, double p1, double p2)
        {
            Vector3 pos = new Vector3(p0, p1, p2, Vector3.VectorForm.POSITION);
            double r = pos.Length();
            if(r > 1)
            {
                return Math.Pow(Math.E, -(r - 1)) + 1;
            }
            else
            {
                return -1;
            }
        }

        public override Vector3 Grad(double p0, double p1, double p2)
        {
            Vector3 pos = new Vector3(p0, p1, p2, Vector3.VectorForm.POSITION);
            double r = pos.Length();
            if (true || r > 1)
            {
                return (new Vector3(pos).Normalize());
            }
            else
            {
                return Vector3.Zero;
            }
        }


    }
}
