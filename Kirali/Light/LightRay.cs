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
        private double n_Responsive = 0.001;

        public double Intensity = 1;

        private bool HASHIT = false;
        public bool hit { get { return HASHIT; } }
        public double n_sensiget { get { return n_Responsive; } }

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

        public override RayPath March(double minimum, double maximum, bool raydual = false)
        {
            double Rp12 = 1.0;
            double range = _lrMarchCast(minimum, n_Responsive, maximum);
            double n1 = N.At(Position); double n2 = N.At(Position + (range * Direction));
            Position.Add(Direction * range);
            Vector3 newDir;
            if(n1 != n2)
            {
                HASHIT = true;
                if (n2 > 0 && n1 > 0)
                {
                    try
                    {
                        
                        Vector3 normal = new Vector3(N.Grad(Position)).Normalize();
                        Vector3 Uaxi = Vector3.Cross(Direction, normal);
                        if (Uaxi.Length() != 0)
                        {
                            Uaxi.Normalize();

                            double Ainc = 0.0;
                            double Aout = 0.0;
                            Matrix rot;

                            Ainc = Vector3.Between(new Vector3(normal), Direction);
                            
                            if (Ainc > Math.PI / 2) 
                            {
                                Ainc = Vector3.Between(-1 * normal, Direction);
                                Aout = Math.Asin(n1 / n2 * Math.Sin(Ainc));
                                Console.WriteLine("Ray entered : " + Math.Round(Ainc * 360 / (Math.PI * 2), 2) + " degrees.");

                                rot = Matrix.RotationU(Uaxi, -Aout);
                                newDir = ((-1 * normal).ToMatrix().Flip() * rot).ToVector3();
                                Console.WriteLine("Ray exited  : " + Math.Round(Aout * 360 / (Math.PI * 2), 2) + " degrees.\n");

                                //AMOUNT OF LIGHT REFLECTED
                                Rp12 =0; // RefractPPolar12(Ainc, Aout);

                                //BUMP because the ray might get stuck if we dont
                                Position.Add(minimum * newDir);
                            }
                            else
                            {
                                Aout = Math.Asin(n1 / n2 * Math.Sin(Ainc));
                                Console.WriteLine("Ray entered : " + Math.Round(Ainc * 360 / (Math.PI * 2), 2) + " degrees.");

                                rot = Matrix.RotationU(Uaxi, Aout);
                                newDir = ((normal).ToMatrix().Flip() * rot).ToVector3();
                                Console.WriteLine("Ray exited  : " + Math.Round(Aout * 360 / (Math.PI * 2), 2) + " degrees.\n");

                                //AMOUNT OF LIGHT REFLECTED
                                Rp12 = 0; // RefractPPolar12(Ainc, Aout);

                                //BUMP because the ray might get stuck if we dont
                                Position.Add(minimum * newDir);
                            }

                        }
                        else
                        {
                            newDir = Direction;
                            Rp12 = RefractPPolar12(1.0 / 1000000, 1.0 / 1000000);
                            
                        }

                        if (Double.IsNaN(newDir.X) || Double.IsNaN(newDir.Y) || Double.IsNaN(newDir.Z))
                        {
                            //FOR A "DIRECT HIT"
                            newDir = Vector3.Bounce(Direction, new Vector3(N.Grad(Position)).Normalize());
                            Rp12 = 0.0;
                        }
                        else
                        {
                            //implement partial reflection and intensity

                            
                        }
                    }
                    catch
                    {
                        Vector3 normal = new Vector3(N.Grad(Position)).Normalize();
                        Vector3 Uaxi = Vector3.Cross(Direction, normal);

                        double Ainc = Vector3.Between(new Vector3(normal).Negate(), Direction);
                        if (Ainc > Math.PI / 2) { Ainc = Vector3.Between(normal, Direction); }

                        newDir = Vector3.Bounce(Direction, new Vector3(N.Grad(Position)).Normalize());

                        double Aout = Vector3.Between(new Vector3(normal).Negate(), newDir);
                        if (Aout > Math.PI / 2) { Ainc = Vector3.Between(normal, newDir); }


                        Console.WriteLine("Ray Bounce In  : " + Math.Round(Ainc * 360 / (Math.PI * 2), 2) + " degrees.");
                        Console.WriteLine("Ray Bounce Out : " + Math.Round(Aout * 360 / (Math.PI * 2), 2) + " degrees.\n");

                        //BUMP because the ray gets stuck if we dont
                        Position.Add(minimum * newDir);

                        Rp12 = 0.0;
                    }
                }
                else
                {
                    Vector3 normal = new Vector3(N.Grad(Position)).Normalize();
                    Vector3 Uaxi = Vector3.Cross(Direction, normal);

                    double Ainc = Vector3.Between(new Vector3(normal).Negate(), Direction);
                    if (Ainc > Math.PI / 2) { Ainc = Vector3.Between(normal, Direction); }
                    
                    newDir = Vector3.Bounce(Direction, new Vector3(normal).Normalize());

                    double Aout = Vector3.Between(new Vector3(normal).Negate(), newDir);
                    if (Aout > Math.PI / 2) { Ainc = Vector3.Between(normal, newDir); }


                    Console.WriteLine("Ray Bounce In  : " + Math.Round(Ainc * 360 / (Math.PI * 2), 2) + " degrees.");
                    Console.WriteLine("Ray Bounce Out : " + Math.Round(Aout * 360 / (Math.PI * 2), 2) + " degrees.\n");

                    //BUMP because the ray gets stuck if we dont
                    Position.Add(minimum * newDir);

                    Rp12 = 0.0;
                }
                Direction = new Vector3(newDir);
                if (raydual)
                {
                    Intensity *= (1 - Rp12);
                }
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

        public static double RefractPPolar12(double inang, double outang)
        {
            double fac = Math.Pow((Math.Tan(Math.Abs(inang) - Math.Abs(outang))) / (Math.Tan(Math.Abs(inang) + Math.Abs(outang))), 2);
            return fac;
        }
    }
}
