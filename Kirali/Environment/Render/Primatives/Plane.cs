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
    public class Plane : Explicit
    {
        private Vector3 POSITION = Vector3.Zero;
        private Vector3 ROTATION = Vector3.Zero;
        //private double RADIUS = 1;
        public KShader SHADER = new KShader();

        private Vector3 C_dir = new Vector3(0, 0, -1);
        private Vector3 C_thet = new Vector3(1, 0, 0);
        private Vector3 C_phi = new Vector3(0, 1, 0);

        public Vector3 position { get { return POSITION; } set { POSITION = value; } }
        //public double radius { get { return RADIUS; } set { RADIUS = value; } }

        public Plane(double Z)
        {
            POSITION = new Vector3(0, 0, Z);
        }

        public Plane(double Z, KShader shader)
        {
            POSITION = new Vector3(0, 0, Z);
        }

        public Plane(Vector3 position)
        {
            POSITION = position;
        }

        public Plane(Vector3 position, KShader shader)
        {
            POSITION = position;
            SHADER = shader;
        }


        public Vector3 Hit(Vector3 initPos, Vector3 rayCast, double zlimiter = 0.05)
        {
            Vector3 hitpoint = new Vector3(0.0, 0.0, 0.0, Vector3.VectorForm.INFINITY); //Presume the ray never hits.
            Vector3 raydir = new Vector3(rayCast).Normalize();
            if(Math.Abs(raydir.Z) < 0.05) { return hitpoint; }
            double vx = raydir.X / raydir.Z;
            double vy = raydir.Y / raydir.Z;
            double vz = raydir.Z;

            double Dz = POSITION.Z - initPos.Z;
            Vector3 Delta = new Vector3(vx * Dz, vy * Dz, vz * Dz);

            Vector3 hptemp = initPos + Delta;
            if (vz < 0)
            {
                hitpoint = hptemp;

                hitpoint.Form = Vector3.VectorForm.POSITION;
            }
            
            return hitpoint;
        }

        public override double At(Vector3 position)
        {
            return At(position.X, position.Y, position.Z);
        }

        public override double At(double p0, double p1, double p2)
        {
            if(p2 == POSITION.Z) { return -1; } else { return 0; }
        }

        public override Vector3 Grad(Vector3 position)
        {
            return Grad(position.X, position.Y, position.Z);
        }

        public override Vector3 Grad(double p0, double p1, double p2)
        {
            return (new Vector3(0, 0, 1));
        }

        public KColor4 GetDiffuseColor(Vector3 point, string colorMode = "")
        {
            KColor4 diffuse = new KColor4(1.0, 1.0, 1.0);

            switch (colorMode)
            {
                case "white":
                    return new KColor4(1.0, 1.0, 1.0);
                case "checkers":
                    double sinchx = Math.Cos(point.X * 0.5 * Math.PI);
                    double sinchy = Math.Cos(point.Y * 0.5 * Math.PI);

                    double checkers = 0.5 + Math.Ceiling(sinchx * sinchy);
                    return new KColor4(checkers, checkers, checkers);
                default:
                    return new KColor4(1.0, 1.0, 1.0);
            }
        }

    }
}
