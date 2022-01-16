using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.MathR;
using Kirali.Light;
using Kirali.Environment.Procedural;

namespace Kirali.Environment.Render.Primatives
{
    public class Sphere : Explicit
    {
        private Vector3 POSITION = Vector3.Zero;
        private Vector3 ROTATION = Vector3.Zero;
        private double RADIUS = 1;
        public KShader SHADER = new KShader();

        private Vector3 C_dir = new Vector3(0, 0, -1);
        private Vector3 C_thet = new Vector3(1, 0, 0);
        private Vector3 C_phi = new Vector3(0, 1, 0);

        public Vector3 position { get { return POSITION; } set { POSITION = value; } }
        public double radius { get { return RADIUS; } set { RADIUS = value; } }

        public Sphere(double radius)
        {
            POSITION = Vector3.Zero;
            RADIUS = radius;
        }

        public Sphere(Vector3 position, double radius)
        {
            POSITION = position;
            RADIUS = radius;
        }

        public Sphere(Vector3 position, double radius, KShader shader)
        {
            POSITION = position;
            RADIUS = radius;
            SHADER = shader;
        }

        public override double At(Vector3 position)
        {
            return At(position.X, position.Y, position.Z);
        }

        public override double At(double p0, double p1, double p2)
        {
            Vector3 pos = new Vector3(p0, p1, p2, Vector3.VectorForm.POSITION);
            double r = (position - pos).Length();
            if (r < RADIUS)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        public override Vector3 Grad(Vector3 position)
        {
            return Grad(position.X, position.Y, position.Z);
        }

        public override Vector3 Grad(double p0, double p1, double p2)
        {
            Vector3 pos = new Vector3(p0, p1, p2, Vector3.VectorForm.POSITION);
            return (new Vector3(pos - position).Normalize());
        }


        //RENDERING QUICK CALCULATIONS!


        public Vector3 NearHit(Vector3 initPos, Vector3 rayCast)
        {
            Vector3 ClosestPoint = new Vector3(0.0, 0.0, 0.0, Vector3.VectorForm.INFINITY); //Presume the ray never hits.
            double L_close = 0; //entering distance
            double L_far   = 0; //exiting distance
            double d = (position - initPos).Length();
            double cos = Math.Cos(Vector3.Between(rayCast, (position - initPos)));

            //this function will be using a basic quadratic formula approach, as we want to solve for L:
            //L^2 - 2Ldcos + d^2 - R^2 = 0;

            double b = -2 * d * cos;
            double c = (d * d) - (RADIUS * RADIUS);

            if(4 * c > b * b)
            {
                return ClosestPoint; //returns a vector3 that the renderer will recognise as a point at infinity (ray did not hit).
            }
            else
            {
                double v0 = ((-1 * b) - Math.Sqrt(b * b - 4 * c)) / 2;
                double v1 = ((-1 * b) + Math.Sqrt(b * b - 4 * c)) / 2;

                //if(v0 > v1)       { L_close = v1; L_far = v0; }
                //else if (v1 < v0) { L_close = v0; L_far = v1; }
                //else              { L_close = L_far = v0;     } //P.S. this will never happen XD

                Vector3 dir = new Vector3(rayCast).Normalize(); //technically a good programmer would already have normal raycast but just in case
                Vector3 point0 = v0 * dir;
                Vector3 point1 = v1 * dir;

                if (point0.Length() > point1.Length())
                {
                    ClosestPoint = initPos + point1;
                }
                else
                {
                    ClosestPoint = initPos + point0;
                }

                ClosestPoint.Form = Vector3.VectorForm.POSITION;
                return ClosestPoint;
            }
        }

        public void CReset()
        {
            //todo simplify rotation;
            C_dir = new Vector3(0, 0, -1);
            C_thet = new Vector3(1, 0, 0);
            C_phi = new Vector3(0, 1, 0);
        }

        public void RotateThet(double radians)
        {
            ROTATION.X = radians;
            Matrix mat = Matrix.RotationU(C_thet, radians);
            C_dir = (C_dir.ToMatrix().Flip() * mat).ToVector3();
            C_phi = (C_phi.ToMatrix().Flip() * mat).ToVector3();
        }

        public void RotatePhi(double radians)
        {
            ROTATION.Y = radians;
            Matrix mat = Matrix.RotationU(C_phi, radians);
            C_thet = (C_thet.ToMatrix().Flip() * mat).ToVector3();
            C_dir = (C_dir.ToMatrix().Flip() * mat).ToVector3();
        }

        public void RotateR(double radians)
        {
            ROTATION.Z = radians;
            Matrix mat = Matrix.RotationU(C_dir, radians);
            C_thet = (C_thet.ToMatrix().Flip() * mat).ToVector3();
            C_phi = (C_phi.ToMatrix().Flip() * mat).ToVector3();
        }

        //SHADERSSS!!!!

        public Vector3 GetNewMap(Vector3 point)
        {
            Vector3 pointing = new Vector3(point);
            pointing = Vector3.RotateU(pointing, C_thet, ROTATION.X);
            pointing = Vector3.RotateU(pointing, C_phi, ROTATION.Y);
            pointing = Vector3.RotateU(pointing, C_dir, ROTATION.Z);
            return pointing;
        }

        

        public KColor4 GetDiffuseColor(Vector3 point, string colorMode = "")
        {
            KColor4 diffuse = new KColor4(1.0, 1.0, 1.0);
            Vector3 rel = (point - position).Normalize();

            switch (colorMode)
            {
                case "white":
                    return new KColor4(1.0, 1.0, 1.0);
                case "normal_exact":
                    return new KColor4(rel.X, rel.Y, rel.Z);
                case "normal_sharp":
                    return new KColor4(Math.Ceiling(rel.X), Math.Ceiling(rel.Y), Math.Ceiling(rel.Z));
                case "perlin_noise":
                    //can't handle negative values yet for some reason?
                    double scale = 5.0;
                    double persist = 0.5;
                    double p = PerlinNoise.OctavePerlin(scale * (rel.X + RADIUS), scale * (rel.Y + RADIUS), scale * (rel.Z + RADIUS), 4, persist);
                    return new KColor4(p, p, p);
                case "star_demo":
                    return SHADER.Emit(rel);
                case "custom01":
                    double v = Math.Cos(Math.PI * 5.0 * rel.Z);
                    return (new KColor4(v, v, v)) * (new KColor4(Math.Ceiling(rel.X), Math.Ceiling(rel.Y), Math.Ceiling(rel.Z)));
                default:
                    return new KColor4(1.0, 1.0, 1.0);
            }
        }

        //shaders that are more complicated:
        
    }
}
