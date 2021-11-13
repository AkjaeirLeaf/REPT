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

        private bool HASHIT = false;
        public bool hit { get { return HASHIT; } }

        public LightRay()
        {
            //TODO: Default Constructor
        }

        public LightRay(Vector3 pos, Vector3 dir)
        {
            this.Position = new Vector3(pos);
            this.source = new Vector3(pos);
            this.Direction = new Vector3(dir);
        }


        public void SetMedium(Explicit n_param, double n_sensitivity)
        {
            N = n_param;
            n_Responsive = n_sensitivity;
        }

        public override RayPath March(double minimum, double maximum)
        {
            double range = _lrMarchCast(minimum, n_Responsive, maximum);
            Position.Add(Direction * range);
            double n1 = N.At(Position); double n2 = N.At(Position + (range * Direction));
            Vector3 newDir;
            if(n1 != n2)
            {
                HASHIT = true;
                if (n2 > 0 && n1 > 0)
                {
                    newDir = Vector3.Refract(Direction, new Vector3(N.Grad(Position)).Normalize(), n1, n2);
                    if(Double.IsNaN(newDir.X) || Double.IsNaN(newDir.Y) || Double.IsNaN(newDir.Z))
                    {
                        newDir = Vector3.Bounce(Direction, new Vector3(N.Grad(Position)).Normalize());
                    }
                }
                else
                {
                    newDir = Vector3.Bounce(Direction, new Vector3(N.Grad(Position)).Normalize());
                }
                Direction = new Vector3(newDir);
            }

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
