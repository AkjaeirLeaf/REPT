using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.MathR;
using Kirali.Framework;

namespace Kirali.Light
{
    public class Camera
    {
        private int WIDTH = 1920 / 2;
        private int HEIGHT = 1080 / 2;
        private double FOV = Math.PI / 3;

        public int Width { get { return WIDTH; } }
        public int Height { get { return HEIGHT; } }
        public double fov { get { return FOV; } set { FOV = value; } }

        public Vector3 position = new Vector3(Vector3.Zero());
        //rot = thet, phi, r
        public Vector3 rotation = new Vector3(Vector3.Zero());

        private Vector3 C_dir = new Vector3(0, 0, -1);
        private Vector3 C_thet = new Vector3(1, 0, 0);
        private Vector3 C_phi = new Vector3(0, 1, 0);

        public Camera()
        {

        }

        public Camera(Vector3 pos)
        {
            position = pos;
        }

        public Camera(Vector3 pos, Vector3 rot, int width, int height)
        {
            position = pos;
            rotation = rot;
            RotateThet(rotation.X);
            RotatePhi(rotation.Y);
            RotateR(rotation.Z);

            WIDTH = width;
            HEIGHT = height;
        }

        //TEMPORARY TEST SETTINGS
        private Vector3 SunLamp = (new Vector3(3, 4, -7)).Normalize();

        public void CReset()
        {
            //todo simplify rotation;
            C_dir = new Vector3(0, 0, -1);
            C_thet = new Vector3(1, 0, 0);
            C_phi = new Vector3(0, 1, 0);
        }

        public void RotateThet(double radians)
        {
            Matrix mat = Matrix.RotationU(C_thet, radians);
            C_dir = (C_dir.ToMatrix().Flip() * mat).ToVector3();
            C_phi = (C_phi.ToMatrix().Flip() * mat).ToVector3();
        }

        public void RotatePhi(double radians)
        {
            Matrix mat = Matrix.RotationU(C_thet, radians);
            C_thet = (C_thet.ToMatrix().Flip() * mat).ToVector3();
            C_dir = (C_dir.ToMatrix().Flip() * mat).ToVector3();
        }

        public void RotateR(double radians)
        {
            Matrix mat = Matrix.RotationU(C_dir, radians);
            C_thet = (C_thet.ToMatrix().Flip() * mat).ToVector3();
            C_phi = (C_phi.ToMatrix().Flip() * mat).ToVector3();
        }

        public KColor4 Shoot(int xp, int yp, Explicit exp)
        {
            KColor4 kc4 = new KColor4();

            double xpre = (xp + (Kirali.Framework.Random.Double(-1, 1) / 2));
            double ypre = yp + (Kirali.Framework.Random.Double(-1, 1) / 2);

            double xv = (xpre / WIDTH) - 0.5;
            double yv = 0.5 - (ypre / HEIGHT);

            double thetx = (fov) * xv;
            double thety = ((double)Height / Width) * (fov) * yv;

            Vector3 pointing = new Vector3(C_dir);
            pointing = Vector3.RotateU(pointing, C_phi, thetx);
            pointing = Vector3.RotateU(pointing, C_thet, thety);

            //THESE ARE TEMPORARY SETTINGS FOR TESTING
            LightRay lr = new LightRay(position, pointing);
            lr.SetMedium(exp, 0.0001);

            bool hit = false;

            while (lr.Position.Length() < 50.0)
            {
                Vector3 vIn = new Vector3(lr.Direction);
                lr.March(0.002, 50);

                if(lr.hit) { hit = true; }
            }


            if (hit)
            {
                double I = Vector3.Dot(lr.Direction.Negate(), SunLamp);
                //double I = 1;
                kc4 = new KColor4(I, I, I); return kc4;
            }
            else
            {
                kc4 = new KColor4(0, 0, 0);
            }

            return kc4;
        }

    }
}
