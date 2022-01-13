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

    public class LinearZone : Explicit
    {
        public override double At(double p0, double p1 = 0, double p2 = 0)
        {
            double y = p1;
            if (y > 2 && y < 8)
            {
                return (Math.Floor(y / 2.0) * 2) * (0.33333333 / 4) + 1.4;
                //return 1 + (1 / 3.0) * Math.Pow(Math.E, y / 8.0);
            }
            else if(y < 8)
            {
                return 1;
            }
            else if(y > 10)
            {
                return -1;
            }
            else
            {
                return 1;

            }
        }

        public override Vector3 Grad(double p0, double p1 = 0, double p2 = 0)
        {
            double y = p1;
            return new Vector3(0, 1, 0);
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
            if (r > 2)
            {
                return Math.Pow(Math.E, -((r - 2) / 3)) * 0.5 + 1;
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

    public class SphereContainer : Explicit
    {
        //this sphere-falloff equation will be Spherical in nature, however symmetric in phi and theta
        //ill use xyz for the sake of making things easier later though.


        public override double At(double p0, double p1, double p2)
        {
            Vector3 pos = new Vector3(p0, p1, p2, Vector3.VectorForm.POSITION);
            double r = pos.Length();
            if (r > 9)
            {
                return -1;
            }
            else if(r > 8)
            {
                return 1.343;
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
