using System;
using System.Collections.Generic;
using System.Text;

using Kirali.MathR;

namespace KiraliConsole
{
    public class LinearFalloff : Explicit
    {
        public override double At(double p0, double p1 = 0, double p2 = 0)
        {
            double x = p0;
            if (x > 1)
            {
                return Math.Pow(Math.E, -(x - 1)) + 1;
            }
            else
            {
                return 1;
            }
        }

        public override Vector3 Grad(double p0, double p1 = 0, double p2 = 0)
        {
            double x = p0;
            if (x > 1)
            {
                return (new Vector3(1, 0, 0) * (-1) * Math.Pow(Math.E, -(x - 1)));
            }
            else
            {
                return new Vector3(-1, 0, 0);
            }
        }
    }
}
