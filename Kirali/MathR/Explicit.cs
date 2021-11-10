using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirali.MathR
{
    /// <summary>
    /// <tooltip>An explicitly defined function used as a placeholder for certain operations. Override the functions At() and Grad() in an inherited class to make use of Explicit.</tooltip>
    /// </summary>
    public partial class Explicit
    {

        //function setups.
        public virtual double At(double p0, double p1 = 0, double p2 = 0) { return 0; }
        public virtual double At(Vector3 position) { return At(position.X, position.Y, position.Z); }

        //Gradients and Derivatives of your function. 
        public virtual Vector3 Grad(double p0, double p1 = 0, double p2 = 0) { return Vector3.Zero(); }
        public virtual Vector3 Grad(Vector3 position) { return Grad(position.X, position.Y, position.Z); }

    }
}
