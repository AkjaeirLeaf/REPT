using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirali.MathR
{
    public partial class Explicit
    {

        //function setups.
        public virtual double At(double p0) { return 0; }
        public virtual double At(double p0, double p1) { return 0; }
        public virtual double At(double p0, double p1, double p2) { return 0; }
        public virtual double At(Vector3 position) { return 0; }

        //Gradients and Derivatives of your function. 
        public virtual double Grad(double p0) { return 0; }
        public virtual double Grad(double p0, double p1) { return 0; }
        public virtual Vector3 Grad(double p0, double p1, double p2) { return Vector3.Zero(); }
        public virtual Vector3 Grad(Vector3 position) { return Vector3.Zero(); }

    }
}
