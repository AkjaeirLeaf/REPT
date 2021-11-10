using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.MathR;

namespace Kirali.Light
{
    public class LightRay : RayPath
    {
        private Explicit N = new Explicit();
        private double n_Responsive = 0.002;

        public override RayPath March(double minimum, double maximum)
        {
            double range = _lrMarchCast(minimum, n_Responsive, minimum);
            double n1 = N.At(Position); double n2 = N.At(Position + (range * Direction));
            Vector3 newDir = Vector3.Refract(Direction, N.Grad(Position), n1, n2);
            Direction = newDir;
            Position.Add(Direction * range);
            return this;
        }

        private double _lrMarchCast(double stepLength, double sensitivity, double kill)
        {
            RayPath marchCast = new RayPath(this.Position, this.Direction, stepLength);
            double initN = N.At(marchCast.Position);
            double range = kill;
            while(marchCast.Distance(marchCast.Source) < kill)
            {
                if(Math.Abs(N.At(marchCast.Position) - initN) >= sensitivity)
                {
                    range = marchCast.Distance(marchCast.Source);
                    break;
                } 
                else
                {
                    marchCast.Step();
                }
            }
            return 0;
        }
    }
}
