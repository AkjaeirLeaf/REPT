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

        public double Intensity = 1;

        public LightRay()
        {
            //TODO: Default Constructor
        }

        public LightRay(Vector3 pos, Vector3 dir)
        {
            this.Position = pos;
            this.source = pos;
            this.Direction = dir;
        }


        public void SetMedium(Explicit n_param, double n_sensitivity)
        {
            N = n_param;
            n_Responsive = n_sensitivity;
        }

        public override RayPath March(double minimum, double maximum)
        {
            double range = _lrMarchCast(minimum, n_Responsive, maximum);
            double n1 = N.At(Position); double n2 = N.At(Position + (range * Direction));
            Vector3 newDir = Vector3.Refract(Direction, new Vector3(N.Grad(Position)).Normalize(), n1, n2);
            Direction = newDir;
            Position.Add(Direction * range);
            return this;
        }

        public double _lrMarchCast(double stepLength, double sensitivity, double kill)
        {
            RayPath marchCast = new RayPath(this.Position, this.Direction, stepLength);
            double initN = N.At(marchCast.Position);
            double range = kill;
            while(marchCast.Distance(marchCast.Source) < kill)
            {
                double atpos = N.At(marchCast.Position);
                if (Math.Abs(atpos - initN) >= sensitivity)
                {
                    range = marchCast.Distance(marchCast.Source);
                    break;
                } 
                else
                {
                    marchCast.Step();
                }
            }
            return range;
        }
    }
}
