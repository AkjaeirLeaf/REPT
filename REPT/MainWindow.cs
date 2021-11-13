using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Kirali.MathR;
using Kirali.Light;
using Kirali.Celestials;

using REPT.Objects;


namespace REPT
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            Test2();
        }

        public void Test2()
        {
            SolidSphere exp = new SolidSphere(Vector3.Zero(), 2);


            Vector3 camerapos = new Vector3(0, 0, 10);
            //Vector3 camerapos = new Vector3(-10, -10, 8);
            Vector3 camerarot = new Vector3(0, 0, 0);
            //Vector3 camerarot = new Vector3(Math.PI / 3, -1 * Math.PI / 4, 0);

            Camera MainCamera = new Camera(camerapos, camerarot, 256, 256);
            //MainCamera.RotateThet(-Math.PI / 3);
            //MainCamera.RotatePhi(Math.PI / 6);

            Bitmap vmp = new Bitmap(MainCamera.Width, MainCamera.Height);

            for (int y = 0; y < vmp.Height; y++)
            {
                for (int x = 0; x < vmp.Width; x++)
                {
                    //we can introduce samples later.
                    int sn = 8;
                    KColor4[] samples = new KColor4[sn];
                    for(int s = 0; s < sn; s++)
                    {
                        samples[s] = MainCamera.Shoot(x, y, exp);
                    }
                    vmp.SetPixel(x, y, KColor4.Average(samples).ToSystemColor());
                }
            }

            vmp.Save("render.png");
            DisplayRenderMain.Image = vmp;
        }

        public void Test1()
        {

            Vector3 posInit = new Vector3(7.8, -12.6, 0, Vector3.VectorForm.POSITION);
            Vector3 dirInit = new Vector3(-4, 6, 0, Vector3.VectorForm.DIRECTION).Normalize();


            LightRay lr = new LightRay(posInit, dirInit);
            Console.WriteLine("Light Ray init position: " + lr.Position.ToString(3));
            Console.WriteLine("Light Ray init direction: " + lr.Direction.ToString(3));
            //LinearFalloff lf_explicit = new LinearFalloff();

            SolidCube lf_explicit = new SolidCube(Vector3.Zero(), 2.0);
            lr.SetMedium(lf_explicit, 0.001);

            Bitmap vmp = new Bitmap(1920 / 2, 1080 / 2);
            double z = 20;

            for (int y = 0; y < vmp.Height; y++)
            {
                double yp = (z * ((double)vmp.Height / vmp.Width)) * (((double)y / vmp.Height) - 0.5);
                for (int x = 0; x < vmp.Width; x++)
                {
                    double xp = z * (((double)x / vmp.Width) - 0.5);
                    double n = lf_explicit.At(xp, yp, 0);
                    if (n > 1 && n < 3)
                    {
                        vmp.SetPixel(x, y, Color.FromArgb((int)(150.0 * (n - 1)), (int)(150.0 * (n - 1)), (int)(255.0 * (n - 1))));
                    }
                    else if (n == -1)
                    {
                        vmp.SetPixel(x, y, Color.FromArgb(255, 70, 90));
                    }
                    else
                    {
                        vmp.SetPixel(x, y, Color.Black);
                    }


                }
            }

            Pen wpen = new Pen(Color.White);

            using (Graphics f = Graphics.FromImage(vmp))
            {
                while (lr.Position.Length() < 35.0)
                {
                    int xc1 = (int)(((lr.Position.X / z) + 0.5) * vmp.Width);
                    int yc1 = (int)((lr.Position.Y / (z * ((double)vmp.Height / vmp.Width)) + 0.5) * vmp.Height);

                    lr.March(0.002, 35);

                    int xc2 = (int)(((lr.Position.X / z) + 0.5) * vmp.Width);
                    int yc2 = (int)((lr.Position.Y / (z * ((double)vmp.Height / vmp.Width)) + 0.5) * vmp.Height);

                    f.DrawLine(wpen, new Point(xc1, yc1), new Point(xc2, yc2));
                }
            }

            vmp.Save("cubew.png");
            DisplayRenderMain.Image = vmp;

        }
    }
}
