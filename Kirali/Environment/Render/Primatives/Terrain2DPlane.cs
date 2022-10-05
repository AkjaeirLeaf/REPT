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
    public class Terrain2DPlane : Explicit
    {
        private Vector3 POSITION = Vector3.Zero;
        private Vector3 ROTATION = Vector3.Zero;
        //private double RADIUS = 1;
        public KShader SHADER = new KShader();

        private Vector3 C_dir = new Vector3(0, 0, -1);
        private Vector3 C_thet = new Vector3(1, 0, 0);
        private Vector3 C_phi = new Vector3(0, 1, 0);

        private double HeightMax = 5;
        private double HeightMin = 0;

        public Vector3 position { get { return POSITION; } set { POSITION = value; } }
        //public double radius { get { return RADIUS; } set { RADIUS = value; } }


        public Terrain2DPlane()
        {
            POSITION = new Vector3(0, 0, 0);
            HeightMin = 0;
        }

        public Terrain2DPlane(double Z)
        {
            POSITION = new Vector3(0, 0, Z);
            HeightMin = Z; HeightMax += HeightMin;
        }

        public Terrain2DPlane(double Z, KShader shader)
        {
            POSITION = new Vector3(0, 0, Z);
        }

        public Terrain2DPlane(Vector3 position)
        {
            POSITION = position;
        }

        public Terrain2DPlane(Vector3 position, KShader shader)
        {
            POSITION = position;
            SHADER = shader;
        }


        public Vector3 HitLow(Vector3 initPos, Vector3 rayCast, double zlimiter = 0.0005)
        {
            Vector3 hitpoint = new Vector3(0.0, 0.0, 0.0, Vector3.VectorForm.INFINITY); //Presume the ray never hits.
            if (initPos.Z > POSITION.Z)
            {
                Vector3 raydir = new Vector3(rayCast).Normalize();
                if (Math.Abs(raydir.Z) < zlimiter) { return hitpoint; }
                double vx = raydir.X / raydir.Z;
                double vy = raydir.Y / raydir.Z;
                double vz = raydir.Z;

                double Dz = HeightMin - initPos.Z;
                Vector3 Delta = new Vector3(vx * Dz, vy * Dz, vz * Dz);

                Vector3 hptemp = initPos + Delta;
                if (vz < 0)
                {
                    hitpoint = hptemp;

                    hitpoint.Form = Vector3.VectorForm.POSITION;
                }

                return hitpoint;
            }
            return hitpoint;
        }

        public Vector3 HitHigh(Vector3 initPos, Vector3 rayCast, double zlimiter = 0.0005)
        {
            Vector3 hitpoint = new Vector3(0.0, 0.0, 0.0, Vector3.VectorForm.INFINITY); //Presume the ray never hits.
            if (initPos.Z > POSITION.Z)
            {
                Vector3 raydir = new Vector3(rayCast).Normalize();
                if (Math.Abs(raydir.Z) < zlimiter) { return hitpoint; }
                double vx = raydir.X / raydir.Z;
                double vy = raydir.Y / raydir.Z;
                double vz = raydir.Z;

                double Dz = HeightMax - initPos.Z;
                Vector3 Delta = new Vector3(vx * Dz, vy * Dz, vz * Dz);

                Vector3 hptemp = initPos + Delta;
                if (vz < 0)
                {
                    hitpoint = hptemp;

                    hitpoint.Form = Vector3.VectorForm.POSITION;
                }
            }
            else
            {
                hitpoint = new Vector3(initPos);
                hitpoint.Form = Vector3.VectorForm.POSITION;
            }
            return hitpoint;
        }


        public Vector3 HitStack(Vector3 initPos, Vector3 rayCast, double zlimiter = 0.0005, double resolve = 0.1)
        {
            Vector3 hitpoint = new Vector3(0.0, 0.0, 0.0, Vector3.VectorForm.INFINITY); //Presume the ray never hits.
            Vector3 hitpointMin = new Vector3(0.0, 0.0, 0.0, Vector3.VectorForm.INFINITY); //Presume the ray never hits.
            Vector3 hitpointMax = new Vector3(0.0, 0.0, 0.0, Vector3.VectorForm.INFINITY); //Presume the ray never hits.
            if (initPos.Z <= HeightMin) { return hitpoint; } //Camera is below both planes, don't render anything
            
            Vector3 raydir = new Vector3(rayCast).Normalize();
            if (Math.Abs(raydir.Z) < zlimiter) { return hitpoint; }
            double vx = raydir.X / raydir.Z;
            double vy = raydir.Y / raydir.Z;
            double vz = raydir.Z;

            //bottom intersection
            double Dz_MIN = HeightMax - initPos.Z;
            Vector3 Delta_MIN = new Vector3(vx * Dz_MIN, vy * Dz_MIN, vz * Dz_MIN);

            //top intersection
            double Dz_MAX = HeightMin - initPos.Z;
            Vector3 Delta_MAX = new Vector3(vx * Dz_MAX, vy * Dz_MAX, vz * Dz_MAX);

            if (initPos.Z > HeightMin)
            {
                Vector3 hptemp = initPos + Delta_MAX;
                if (vz < 0)
                {
                    hitpointMax = new Vector3(hptemp);
                    hitpointMax.Form = Vector3.VectorForm.POSITION;
                }

                if (initPos.Z > HeightMax) // Camera is between the two elevation planes
                {
                    if(vz > 0)
                    {
                        hitpointMin = new Vector3(initPos);
                        hitpointMin.Form = Vector3.VectorForm.POSITION;
                    }
                }
                else // Camera is above both the min and max elevation planes
                {
                    if (vz < 0)
                    {
                        hptemp = initPos + Delta_MIN;
                        hitpointMin = new Vector3(hptemp);
                        hitpointMin.Form = Vector3.VectorForm.POSITION;
                    }
                }
            }


            //A basic collision stack search algorithm
            //this particular one divides a chunk in two parts and tests if the point is higher or lower than the terrain in that column,
            //if terrain  is higher, the tests will only continue in the first half.
            //if position is higher, the tests will only continue in the second half. // this causes problems though

            //weaknesses in this algorithm:
            //low angles and locations the ray hits the terrain immediately
            //very sharp terrain.

            bool surfpointfound = false;

            int pow = 1;
            double searchlength = (hitpointMin - hitpointMax).Length();
            double place = searchlength / Math.Pow(2, pow);

            while (!surfpointfound)
            {
                pow++;
                Vector3 testPos = initPos + raydir * place;
                double hei = Hbelow(testPos);
                //test if in reasonable range.
                if(Math.Abs(testPos.Z - hei) < resolve) { hitpoint = new Vector3(testPos); surfpointfound = true; break; }
                if(testPos.Z < hei) //pos is under terrain, move to closer sector.
                {
                    place -= searchlength / Math.Pow(2, pow);
                }
                else //pos is above terrain, move to farther sector.
                {
                    place += searchlength / Math.Pow(2, pow);
                }
            }
            if (surfpointfound)
            {
                hitpoint.Form = Vector3.VectorForm.POSITION;
            }

            return hitpoint;
        }

        public double Hbelow(Vector3 pos)
        {
            //this will be made more terrainlike later, right now it's just a bump tho.

            if(pos.Y < 5 && pos.Y >=0) { return -1 * pos.Y + 5; }
            else if(pos.Y > -5 && pos.Y < 0) { return pos.Y + 5; }
            else { return 0; }
        }

        public Vector3 Normal(Vector3 incoming, Vector3 hitPos, double delta)
        {
            Vector3[] testpositions = Vector3.BumpPlane(Grad(hitPos), incoming, hitPos, delta);

            Vector3 xax = testpositions[4]; //these are already normalized :)
            Vector3 yax = testpositions[5];

            double h0 = Hbelow(testpositions[0]);
            double h1 = Hbelow(testpositions[1]);
            double h2 = Hbelow(testpositions[2]);
            double h3 = Hbelow(testpositions[3]);

            double xam = 0.5 * (h1 - h0 + h2 - h3) / delta;
            double yam = 0.5 * (h0 - h3 + h1 - h2) / delta;

            Vector3 normalr = new Vector3(Grad(hitPos)); //idk if this is right...
            normalr += xax * xam;
            normalr += yax * yam;


            normalr.Normalize();
            return normalr;
        }

        
        //EXPLICIT overrides :/
        public override double At(Vector3 position)
        {
            return At(position.X, position.Y, position.Z);
        }

        public override double At(double p0, double p1, double p2)
        {
            if (p2 == POSITION.Z) { return -1; } else { return 0; }
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
