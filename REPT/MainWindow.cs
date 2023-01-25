using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using Kirali.MathR;
using Kirali.Light;
using Kirali.Framework;
using Kirali.Celestials;
using Kirali.Environment.Shaders;
using Kirali.Environment.Render.Primatives;
using Kirali.Storage;
using Kirali.REGS;

using REPT.Objects;


namespace REPT
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            //Test25();
            //Test26();
            Test25();
        }

        public void Test27()
        {
            Triangle3D t = new Triangle3D();
            t.Transform(new Vector3(0, 0, 1));
            Vector3 p    = new Vector3(0, 0, 10);
            Vector3 inc  = new Vector3(0, 0, -1);

            bool hits = t.RayDoesIntersect(p, inc);

            Console.WriteLine(hits);
        }

        //bake population survey voxel densities of galaxy.
        public void Test26()
        {
            double size = 1.0; //EXAMETERS
            Vector3 pos = Vector3.Zero;
            RGalaxy kjianoaa = RGalaxy.FromFolder("D:\\Desktop Control\\VisualStudio\\V C# Net Projects\\REGS\\REGS\\bin\\Debug\\kjianoaa",
                true, true);

            SystemSearchParameter SSP = new SystemSearchParameter();
            SSP.ChunkSize = size;
            SSP.X_chunk = pos.X;
            SSP.Y_chunk = pos.Y;
            SSP.Z_chunk = pos.Z;

            kjianoaa.LoadSystemPoints(SSP);
            double temp = kjianoaa.AvgSystemPrimaryTemp();


            int[] classCounts = new int[100];
            for (int st = 0; st < kjianoaa.system_points.Length; st++)
            {
                classCounts[kjianoaa.system_points[st].spectralClass]++;
            }
            string sve = "";
            for (int pl = 0; pl < classCounts.Length; pl++)
            {
                sve += classCounts[pl] + "\n";
            }

            File.WriteAllText("classCounter", sve);

            Console.WriteLine("Ready,,,,aiuegkhljffff.");

        }

        public void Test25()
        {
            DateTime dtStart = DateTime.Now;
            int w = 5;
            int h = 5;
            Vector3[] ps = new Vector3[w * h];
            for(int k = 0; k < h; k++)
            {
                for (int l = 0; l < w; l++)
                {
                    ps[k * w + l] = new Vector3(l, k + 7, Kirali.Framework.Random.Double(0, 2));
                }
            }
            Triangle3DMesh mesh = Triangle3DMesh.GridAutoMesh(w, h, ps);
            
            //tri.Scale(1);
            //tri.Transform(new Vector3(0, 15, 0));
            //mesh.AddTriangle(tri);

            //Vector3 camerapos = new Vector3(0, -12, 7.0);

            Vector3 camerapos = new Vector3(0, -12, 7);
            Vector3 camerarot = new Vector3(0, 0, 0);

            Vector3 LAMP_POS = new Vector3(0, 15.0, 5); double LAMP_LUM = 3;
            KColor4 LAMP_COLOR = KColor4.WHITE; //new KColor4(6.4, 2.1, 4.0);
            int dres = 8;
            Camera MainCamera = new Camera(camerapos, camerarot, 1920 / dres, 1080 / dres);


            MainCamera.RotateThet(-Math.PI / 2.2);
            //WMainCamera.RotatePhi(Math.PI / 6);
            //MainCamera.RotateR(Math.PI * (1 + 0.15));

            KColorImage renderImageRaw = new KColorImage(MainCamera.Width, MainCamera.Height);
            renderImageRaw.Fill(KColor4.BLACK);
            Bitmap vmp = new Bitmap(MainCamera.Width, MainCamera.Height);
            int samplesMax = 2;


            //no need for angle limiter because the plane hit function has it built in

            for (int y = 0; y < vmp.Height; y++)
            {
                for (int x = 0; x < vmp.Width; x++)
                {
                    int sn = samplesMax;
                    KColor4[] samples = new KColor4[sn];
                    bool dosetPoint = true;

                    for (int s = 0; s < sn; s++)
                    {
                        double bright = 0;
                        Vector3 currentCameraVector = MainCamera.GetRayCast(x, y);

                        //double betlamp = Vector3.Between(LAMP_POS - camerapos, currentCameraVector);
                        //double bet = Vector3.Between(sphere.position - camerapos, currentCameraVector);

                        Vector3 hitClose;
                        if (mesh.DoesCollide(MainCamera.position, currentCameraVector))
                        {
                            samples[s] = KColor4.WHITE;
                            Triangle3D t = mesh.FirstHitTriangle(camerapos, currentCameraVector, out hitClose, out _);
                        }
                        /*
                        //Triangle3D t = mesh.FirstHitTriangle(camerapos, currentCameraVector, out hitClose, out _);
                        if (hitClose.Form != Vector3.VectorForm.INFINITY && hitClose.Form == Vector3.VectorForm.POSITION)
                        {
                            //bool doesHit = mesh.RayDoesIntersect(camerapos, currentCameraVector);
                            //bright = LAMP_LUM / Math.Pow((LAMP_POS - hitClose).Length(), 2) * Math.Abs(Vector3.Dot((LAMP_POS - hitClose).Normalize(), plane.Normal));

                            samples[s] = KColor4.WHITE;
                            //samples[s] = plane.GetDiffuseColor(hitClose, "colorwheel") * ((new KColor4(1, 1, 1) * 0.4) + (LAMP_COLOR * bright));
                        }
                        else
                        {
                            dosetPoint = false;
                            //samples[s] = new KColor4(0.0, 0.0, 0.0, 1.0);
                        }
                        */
                    }
                    if (dosetPoint)
                    {
                        renderImageRaw.SetPoint(x, y, KColor4.Average(samples));
                    }
                }
            }
            //KColorImage bloomIm = renderImageRaw.GetBloomMapped(15, 0.0005);

            vmp = renderImageRaw.ToSystemBitmap();

            TimeSpan renderTimeElapse = DateTime.Now - dtStart;

            vmp.Save("fastrenders\\rendertriangles_" + samplesMax + "iter_" + Math.Round(renderTimeElapse.TotalMilliseconds) / 1000.0 + "sec.png");
            DisplayRenderMain.Image = vmp;
        }

        //Load Galaxy and also edit/add celestial object, I'll add the MUSHROOM object
        public void Test24()
        {
            RGalaxy kjianoaa = RGalaxy.FromFolder("D:\\Desktop Control\\VisualStudio\\V C# Net Projects\\REGS\\REGS\\bin\\Debug\\kjianoaa",
                true, true);
            //BE CAREFUL!
            //While loading without starpoints is fast, and allows one to load and edit star systems,
            //you can ONLY EDIT RELEASED SYSTEMS with limited functionality without causing
            //inconsistencies. ITS ALWAYS BETTER TO FULLY LOAD STARPOINTS unless doing a silly
            //little debug test like this:

            //OKOK nevermind I do need to because CelestialObject.RunAll_Update requires starpoints to be loaded.

            StarSystemData ssystem;
            REGS_STD_ERROR err = (REGS_STD_ERROR)kjianoaa.LoadReleasedSystem("Mushroom", out ssystem);
            //THIS ^^^ is an EXACT MATCH.
            //Check if success:
            if(err == REGS_STD_ERROR.FOUND_RELEASED_SYSTEM)
            {
                Console.WriteLine("Success!!! System Found!");
                CelestialObject MushroomStar = new CelestialObject("Mushroom", "Mushroom", CobjectTypes.star);
                MushroomStar.RunAll_Update(kjianoaa, ssystem);
                //Add more shit


                //SAVE
                ssystem.AddCelestialObject(MushroomStar);
                kjianoaa.SaveSystemData(ssystem);
            }
        }

        //Star Rendering Test.
        public void Test23()
        {
            double Temp     = 14000;
            double Size     = 11.27 * 10E9;
            double Distance = 158 * 10E17;
            KColor4 approx = KColor4.visibleBlackBodyApprox(Temp);

            Bitmap disp = new Bitmap(1920 / 4, 1080 / 4);
            double f = (Size * Size) / (Distance * Distance);
            KColor4 safe = approx.SafeDeExpose(f); Console.WriteLine(approx.IntensityRGB()); Console.WriteLine(f);
            using (Graphics gh = Graphics.FromImage(disp))
            {
                //gh.DrawRectangle(new Pen(Color.Black), new Rectangle(new Point(0, 0), disp.Size));
                gh.FillRectangle(new SolidBrush(safe.ToSystemColor()), new Rectangle(new Point(0, 0), disp.Size));
            }
            //disp.SetPixel(disp.Width / 2, disp.Height / 2, safe.ToSystemColor());

            DisplayRenderMain.Image = disp;
            disp.Save("startest.png");
        }

        //BBdy test
        public void Test22()
        {
            double wlmin = 1;
            double wlmax = 20000;
            Bitmap disp = new Bitmap(1920 / 1, 1080 / 1);
            using (Graphics gh = Graphics.FromImage(disp))
            {
                //SPECT DRAW
                double T = wlmin;
                double wlch = (wlmax - wlmin) / disp.Width;
                Pen ps;

                //SCATTER FUNCTION DRAW
                Point lastPos0R = new Point(0, 0);
                Point lastPos0G = new Point(0, 0);
                Point lastPos0B = new Point(0, 0);
                Point lastPos1R = new Point(0, 0);
                Point lastPos1G = new Point(0, 0);
                Point lastPos1B = new Point(0, 0);
                double I_Min = 0;
                double I_Max = 10E17;
                double I_range = I_Max - I_Min;
                Pen r_pen = new Pen(Color.Red);
                Pen g_pen = new Pen(Color.Green);
                Pen b_pen = new Pen(Color.Blue);


                //Dat DRW
                SolidBrush datapointbr = new SolidBrush(Color.Black);



                for (int x = 0; x < disp.Width; x++)
                {
                    //spectrum draw
                    T = wlmin + x * wlch;
                    KColor4 approx = KColor4.visibleBlackBodyApprox(T);
                    KColor4 safe = approx.SafeDeExpose(3 / 10E17);
                    ps = new Pen(safe.ToSystemColor());
                    gh.DrawLine(ps, new Point(x, 0), new Point(x, disp.Height));

                    if (true)
                    {
                        double I1_Red   = approx.R;
                        double I1_Green = approx.G;
                        double I1_Blue  = approx.B;


                        Point newPos1R = new Point(x, (int)(disp.Height * (1.0 - (I1_Red - I_Min) / I_range)));
                        Point newPos1G = new Point(x, (int)(disp.Height * (1.0 - (I1_Green - I_Min) / I_range)));
                        Point newPos1B = new Point(x, (int)(disp.Height * (1.0 - (I1_Blue - I_Min) / I_range)));

                        if (x == disp.Width / 2)
                        {
                            Console.WriteLine(I1_Red);
                            Console.WriteLine(I1_Green);
                            Console.WriteLine(I1_Blue);
                        }

                        try
                        {
                            gh.DrawLine(r_pen, lastPos1R, newPos1R);
                            gh.DrawLine(g_pen, lastPos1G, newPos1G);
                            gh.DrawLine(b_pen, lastPos1B, newPos1B);
                        }
                        catch { }
                        lastPos1R = newPos1R;
                        lastPos1G = newPos1G;
                        lastPos1B = newPos1B;
                    }
                }


            }


            DisplayRenderMain.Image = disp;
            disp.Save("testrenderspec22.png");
        }

        //BBdy test
        public void Test21()
        {
            double wlmin = 200;
            double wlmax = 1200;
            Bitmap disp = new Bitmap(1920 / 1, 1080 / 1);
            using (Graphics gh = Graphics.FromImage(disp))
            {
                //SPECT DRAW
                double wl = wlmin;
                double wlch = (wlmax - wlmin) / disp.Width;
                Pen ps;

                //SCATTER FUNCTION DRAW
                Point lastPos0R = new Point(0, 0);
                Point lastPos1R = new Point(0, 0);
                
                double I_Min = 0;
                double I_Max = 10;
                double I_range = I_Max - I_Min;
                Pen r_pen = new Pen(Color.Red);
                Pen g_pen = new Pen(Color.Green);
                Pen b_pen = new Pen(Color.Blue);


                //Dat DRW
                SolidBrush datapointbr = new SolidBrush(Color.Black);



                for (int x = 0; x < disp.Width; x++)
                {
                    //spectrum draw
                    wl = wlmin + x * wlch;
                    ps = new Pen(KColor4.fullspecWavelengthRGB(wl / 1000).ToSystemColor());
                    //gh.DrawLine(ps, new Point(x, 0), new Point(x, disp.Height));

                    if (true)
                    {
                        double I1_Bv = Kirali.Light.KColor4.Bv_L(wl, 6000) / 10E13;


                        Point newPos1R = new Point(x, (int)(disp.Height * (1.0 - (I1_Bv - I_Min) / I_range)));

                        if (x == disp.Width / 2)
                        {
                            Console.WriteLine(I1_Bv);
                        }

                        try
                        {
                            gh.DrawLine(r_pen, lastPos1R, newPos1R);
                        }
                        catch { }
                        lastPos1R = newPos1R;
                    }
                }


            }


            DisplayRenderMain.Image = disp;
            disp.Save("testrenderspec22.png");
        }

        //BBdy test
        public void Test20()
        {
            double wlmin = 200;
            double wlmax = 1200;
            Bitmap disp = new Bitmap(1920 / 1, 1080 / 1);
            using (Graphics gh = Graphics.FromImage(disp))
            {
                //SPECT DRAW
                double wl = wlmin;
                double wlch = (wlmax - wlmin) / disp.Width;
                Pen ps;

                //SCATTER FUNCTION DRAW
                Point lastPos0R = new Point(0, 0);
                Point lastPos0G = new Point(0, 0);
                Point lastPos0B = new Point(0, 0);
                Point lastPos1R = new Point(0, 0);
                Point lastPos1G = new Point(0, 0);
                Point lastPos1B = new Point(0, 0);
                double I_Min = 0;
                double I_Max = 10E14;
                double I_range = I_Max - I_Min;
                Pen r_pen = new Pen(Color.Red);
                Pen g_pen = new Pen(Color.Green);
                Pen b_pen = new Pen(Color.Blue);


                //Dat DRW
                SolidBrush datapointbr = new SolidBrush(Color.Black);



                for (int x = 0; x < disp.Width; x++)
                {
                    //spectrum draw
                    wl = wlmin + x * wlch;
                    ps = new Pen(KColor4.fullspecWavelengthRGB(wl/1000).ToSystemColor());
                    //gh.DrawLine(ps, new Point(x, 0), new Point(x, disp.Height));

                    if (true)
                    {
                        double I1_Red   = Kirali.Light.KColor4.R_FullS(wl, 6000);
                        double I1_Green = Kirali.Light.KColor4.G_FullS(wl, 6000);
                        double I1_Blue  = Kirali.Light.KColor4.B_FullS(wl, 6000);


                        Point newPos1R = new Point(x, (int)(disp.Height * (1.0 - (I1_Red   - I_Min) / I_range)));
                        Point newPos1G = new Point(x, (int)(disp.Height * (1.0 - (I1_Green - I_Min) / I_range)));
                        Point newPos1B = new Point(x, (int)(disp.Height * (1.0 - (I1_Blue  - I_Min) / I_range)));

                        if(x == disp.Width / 2)
                        {
                            Console.WriteLine(I1_Red);
                            Console.WriteLine(I1_Green);
                            Console.WriteLine(I1_Blue);
                        }

                        try
                        {
                            gh.DrawLine(r_pen, lastPos1R, newPos1R);
                            gh.DrawLine(g_pen, lastPos1G, newPos1G);
                            gh.DrawLine(b_pen, lastPos1B, newPos1B);
                        }
                        catch { }
                        lastPos1R = newPos1R;
                        lastPos1G = newPos1G;
                        lastPos1B = newPos1B;
                    }
                }

                
            }


            DisplayRenderMain.Image = disp;
            disp.Save("testrenderspec22.png");
        }

        //plane render test, preparation for terrain implementation
        public void Test19()
        {
            DateTime dtStart = DateTime.Now;
            Triangle3D plane = new Triangle3D();
            plane.Scale(5);
            plane.Transform(new Vector3(0, 15, 0));

            Vector3 camerapos = new Vector3(0, -12, 7);
            //Vector3 camerapos = new Vector3(0, -12, 7.0);

            Vector3 camerarot = new Vector3(0, 0, 0);

            Vector3 LAMP_POS = new Vector3(0, 15.0, 5); double LAMP_LUM = 3;
            KColor4 LAMP_COLOR = KColor4.WHITE; //new KColor4(6.4, 2.1, 4.0);
            int dres = 2;
            Camera MainCamera = new Camera(camerapos, camerarot, 1920 / dres, 1080 / dres);


            MainCamera.RotateThet(-Math.PI / 2.2);
            //WMainCamera.RotatePhi(Math.PI / 6);
            //MainCamera.RotateR(Math.PI * (1 + 0.15));

            KColorImage renderImageRaw = new KColorImage(MainCamera.Width, MainCamera.Height);
            renderImageRaw.Fill(KColor4.BLACK);
            Bitmap vmp = new Bitmap(MainCamera.Width, MainCamera.Height);
            int samplesMax = 2;


            //no need for angle limiter because the plane hit function has it built in

            for (int y = 0; y < vmp.Height; y++)
            {
                for (int x = 0; x < vmp.Width; x++)
                {
                    int sn = samplesMax;
                    KColor4[] samples = new KColor4[sn];
                    bool dosetPoint = true;

                    for (int s = 0; s < sn; s++)
                    {
                        double bright = 0;
                        Vector3 currentCameraVector = MainCamera.GetRayCast(x, y);

                        //double betlamp = Vector3.Between(LAMP_POS - camerapos, currentCameraVector);
                        //double bet = Vector3.Between(sphere.position - camerapos, currentCameraVector);

                        Vector3 hitClose = plane.Hit(camerapos, currentCameraVector);
                        if (hitClose.Form != Vector3.VectorForm.INFINITY && hitClose.Form == Vector3.VectorForm.POSITION)
                        {
                            bright = LAMP_LUM / Math.Pow((LAMP_POS - hitClose).Length(), 2) * Math.Abs(Vector3.Dot((LAMP_POS - hitClose).Normalize(), plane.Normal));

                            //samples[s] = sphere.SHADER.Diffuse(sphere.GetNewMap(hitClose)) * bright;
                            samples[s] = plane.GetDiffuseColor(hitClose, "colorwheel") * ((new KColor4(1, 1, 1) * 0.4) + (LAMP_COLOR * bright));
                        }
                        else
                        {
                            dosetPoint = false;
                            //samples[s] = new KColor4(0.0, 0.0, 0.0, 1.0);
                        }
                    }
                    if (dosetPoint)
                    {
                        renderImageRaw.SetPoint(x, y, KColor4.Average(samples));
                    }
                }
            }
            //KColorImage bloomIm = renderImageRaw.GetBloomMapped(15, 0.0005);

            vmp = renderImageRaw.ToSystemBitmap();

            TimeSpan renderTimeElapse = DateTime.Now - dtStart;

            vmp.Save("fastrenders\\rendertriangles_" + samplesMax + "iter_" + Math.Round(renderTimeElapse.TotalMilliseconds) / 1000.0 + "sec.png");
            DisplayRenderMain.Image = vmp;
        }

        //plane render test, preparation for terrain implementation
        public void Test18()
        {
            DateTime dtStart = DateTime.Now;
            Plane plane = new Plane(0);

            Vector3 camerapos = new Vector3(0, -12, 7.0);

            Vector3 camerarot = new Vector3(0, 0, 0);

            Vector3 LAMP_POS = new Vector3(0, 15.0, 5); double LAMP_LUM = 10;
            KColor4 LAMP_COLOR = new KColor4(6.4, 2.1, 4.0);
            int dres = 4;
            Camera MainCamera = new Camera(camerapos, camerarot, 1920 / dres, 1080 / dres);


            MainCamera.RotateThet(-Math.PI / 2.2);
            //WMainCamera.RotatePhi(Math.PI / 6);
            //MainCamera.RotateR(Math.PI * (1 + 0.15));

            KColorImage renderImageRaw = new KColorImage(MainCamera.Width, MainCamera.Height);
            renderImageRaw.Fill(KColor4.BLACK);
            Bitmap vmp = new Bitmap(MainCamera.Width, MainCamera.Height);
            int samplesMax = 1;


            //no need for angle limiter because the plane hit function has it built in

            for (int y = 0; y < vmp.Height; y++)
            {
                for (int x = 0; x < vmp.Width; x++)
                {
                    int sn = samplesMax;
                    KColor4[] samples = new KColor4[sn];
                    bool dosetPoint = true;

                    for (int s = 0; s < sn; s++)
                    {
                        double bright = 0;
                        Vector3 currentCameraVector = MainCamera.GetRayCast(x, y);

                        //double betlamp = Vector3.Between(LAMP_POS - camerapos, currentCameraVector);
                        //double bet = Vector3.Between(sphere.position - camerapos, currentCameraVector);

                        Vector3 hitClose = plane.Hit(camerapos, currentCameraVector, 0.005);
                        if (hitClose.Form != Vector3.VectorForm.INFINITY && hitClose.Form == Vector3.VectorForm.POSITION)
                        {
                            bright = LAMP_LUM / Math.Pow((LAMP_POS - hitClose).Length(), 2) * Math.Abs(Vector3.Dot((LAMP_POS - hitClose).Normalize(), plane.Grad(hitClose)));

                            //samples[s] = sphere.SHADER.Diffuse(sphere.GetNewMap(hitClose)) * bright;
                            samples[s] = plane.GetDiffuseColor(hitClose, "white") * ((new KColor4(1, 1, 1) * 0.4) + (LAMP_COLOR * bright));
                        }
                        else
                        {
                            dosetPoint = false;
                            //samples[s] = new KColor4(0.0, 0.0, 0.0, 1.0);
                        }
                    }
                    if (dosetPoint)
                    {
                        renderImageRaw.SetPoint(x, y, KColor4.Average(samples));
                    }
                }
            }
            //KColorImage bloomIm = renderImageRaw.GetBloomMapped(15, 0.0005);

            vmp = renderImageRaw.ToSystemBitmap();

            TimeSpan renderTimeElapse = DateTime.Now - dtStart;

            vmp.Save("fastrenders\\renderplane_" + samplesMax + "iter_" + Math.Round(renderTimeElapse.TotalMilliseconds) / 1000.0 + "sec.png");
            DisplayRenderMain.Image = vmp;
        }

        //do save optical storage data
        public void Test17()
        {
            OpticalStorage osf_R_OPL;

            double R_planet = 6371000; //meters

            int angledivs = 100;
            double dtheta = Math.PI / 2 / (double)angledivs;
            double dwavelength = 0.05;
            double wlmin = 0.3; double wlmax = 0.85; //visible only
            //double wlmin = 0.2; double wlmax = 5.2; //ira & irb, visible, and UV.

            int wlct = (int)((wlmax - wlmin) / dwavelength);

            //begin WRITE.
            int wlindex = 0;

            int[] wvlts = new int[wlct];
            //double[] 
            for (double wl = wlmin; wl < wlmax - dwavelength; wl += dwavelength)
            {
                int wlNM = (int)(1000 * wl); wvlts[wlindex] = wlNM;
                wlindex++;
            }
            wlindex = 0;

            int dist_upper = 1000;
            int dist_block_ct = 100;
            osf_R_OPL = new OpticalStorage(wvlts, OpticalAngleRangeMethod.ANGLES_HALF1PI, angledivs, dist_block_ct, dist_upper);

            for (double wl = wlmin; wl < wlmax - dwavelength; wl += dwavelength)
            {
                double[] opl_R_dat = new double[angledivs];
                int angint = 0;
                for (double theta = 0; theta < Math.PI / 2; theta += dtheta)
                {
                    //get view angle: 
                    double rayang = Math.PI - theta; //orientation of integration is Z = direction of SUN
                    double a = Atmospherics.std_OpticalPathLength(10.0 + R_planet, rayang, wl, 2, 70000, R_planet);
                    try { opl_R_dat[angint] = a; } catch { }
                    angint++;
                }
                osf_R_OPL.Add_R_OPPATHDEPTH(wlindex, opl_R_dat);

                double[] opl_R_Height = new double[dist_block_ct];
                int distint = 0;
                for (int H_travel = 0; H_travel < dist_upper; H_travel += dist_upper / dist_block_ct)
                {
                    double rayang = Math.PI / 2; //orientation of integration is Z = direction of SUN
                    double a = Atmospherics.std_OpticalPathLength(2.0 + 1000 * H_travel + R_planet, rayang, wl, 2, 70000, R_planet);
                    try { opl_R_Height[distint] = a; } catch { }
                    distint++;
                }
                osf_R_OPL.Add_R_DCLOSEST(wlindex, opl_R_Height);

                wlindex++;
            }

            osf_R_OPL.SerializeWriteObject("ose_space_AhitDclose.optstor");
        }

        //test using a polynomial interpolation method
        public void Test16()
        {
            double[] xs = new double[] {   1,   2,   3,   4,   5,   6,   7 };
            double[] ys = new double[] { 2.0, 5.0, 1.0, 2.0, 2.5, 3.0, 4.0 };

            double xpos = 4.5;

            Console.WriteLine("(" + xpos + ", " + Interpolate.PolynomialSeries(xpos, xs, ys) + ")");
        }

        //plot the temperature with pressure gradient in the background
        public void Test15()
        {
            double top  = 400; double bottom = 100;
            double left = 0;   double right  = 100;

            Bitmap bmp = new Bitmap(1920, 1080);

            using(Graphics g = Graphics.FromImage(bmp))
            {

                //draw gradient
                for (int x = 0; x < bmp.Width; x++)
                {
                    double pres = Atmospherics.std_airPressureApprox(1000 * ((double)x / bmp.Width) * (right - left) + left);
                    double basep = 1013.25;
                    Pen presp = new Pen(new KColor4(pres * 0.7 / basep, pres * 0.7 / basep, pres * 1.0 / basep, 1.0).ToSystemColor());
                    g.DrawLine(presp, new Point(x, bmp.Height), new Point(x, 0));
                }
                Pen spen = new Pen(Color.Blue, 1.0f);
                for (int h = 0; h < bmp.Height; h += bmp.Height / 15)
                {
                    g.DrawLine(spen, new Point(0, h), new Point(bmp.Width, h));
                }
                //draw temp
                int last = -1;
                int lastst = -1;
                for (int x = 0; x < bmp.Width; x++)
                {
                    //new model
                    double xposconvert = ((double)x / bmp.Width) * (right - left) + left;
                    double tempst = Interpolate.FourPointSeries(xposconvert, Atmospherics.std_atm1976_heights, Atmospherics.std_atm1976_tempers);
                    int temppos1 = bmp.Height - (int)(bmp.Height * (tempst - bottom) / (top - bottom));
                    Pen tempean  = new Pen(Color.DarkRed, 4.0f);
                    Pen tempenu  = new Pen(Color.Magenta, 4.0f);
                    if (lastst == -1) { last = temppos1; }
                    g.DrawLine(tempenu, new Point(x - 1, lastst - 1), new Point(x, temppos1 - 1));
                    g.DrawLine(tempean, new Point(x - 1, lastst), new Point(x, temppos1));
                    lastst = temppos1;

                    //old model
                    double temp = Atmospherics.std_roughAltTemp(xposconvert);
                    int temppos = bmp.Height - (int)(bmp.Height * (temp - bottom) / (top - bottom));
                    tempean = new Pen(Color.Green, 4.0f);
                    tempenu = new Pen(Color.LightGreen, 4.0f);
                    if (last == -1) { last = temppos; }
                    g.DrawLine(tempenu, new Point(x - 1, last-1), new Point(x, temppos-1));
                    g.DrawLine(tempean, new Point(x - 1, last), new Point(x, temppos));
                    last = temppos;
                }
                Pen mpen = new Pen(Color.Black, 4.0f);
                for (int h = 0; h <= bmp.Height; h += bmp.Height / 15)
                {
                    g.DrawLine(mpen, new Point(0, h), new Point(bmp.Width/35, h));
                }
                Pen Bpen = new Pen(Color.Black, 8.0f);
                for (int h = 0; h <= bmp.Height; h += bmp.Height / 3)
                {
                    g.DrawLine(Bpen, new Point(0, h), new Point(bmp.Width / 25, h));
                }
                Pen hmpen = new Pen(Color.White, 4.0f);
                for (int h = 0; h <= bmp.Width; h += bmp.Width / 20)
                {
                    g.DrawLine(hmpen, new Point(h, bmp.Height), new Point(h, bmp.Height - bmp.Height / 30));
                }
                Pen hBpen = new Pen(Color.White, 8.0f);
                for (int h = 0; h <= bmp.Width; h += bmp.Width / 4)
                {
                    g.DrawLine(hBpen, new Point(h, bmp.Height), new Point(h, bmp.Height - bmp.Height / 20));
                }
                bmp.Save("tempgraph1.png");
                DisplayRenderMain.Image = bmp;
            }
        }

        //intensity of light hitting the planet straight on, plotted as a function of wavelength
        public void Test14()
        {
            double T_sun = 5778; //kelvin
            double R_sun = 695700000; //meters
            double R_planet = 6371000; //meters
            double R_orbit = 1.496E+11; //meters

            double lpart = Physics.lumPartial(T_sun);

            //bake table of bbody intensity:
            //start at arb value 0.1 um
            //upper is harder bc the falloff is so slow, most activity is low enough around 4 um



            //                                                  \/ this is to account for planet "normal"
            //integrate: 4 pi R_planet^2 * from (0 to pi / 2) * cos(theta) * I(wavelength) dtheta

            //I(wl) = (sun wl intensity from table) (transmittance) (distance falloff ratio)
            //transmittance = e ^ -OPL

            double falloffHandle = (R_sun * R_sun) / (R_orbit * R_orbit);


            double dtheta = Math.PI / 2 / 100.0;
            double accumulateSurf = 0;

            double dwavelength = 0.0002;
            double wlmin = 0.2; double wlmax = 2.2;
            int wlct = (int)((wlmax - wlmin) / dwavelength);


            //create quick wavelength table
            double[] wlIntStorage = new double[wlct];

            for (int wlc = 0; wlc < wlct; wlc++)
            {
                double currwl = ((double)wlc * dwavelength) + wlmin;
                wlIntStorage[wlc] = Physics.planckLaw(T_sun, currwl, 'u');
            }

            double LastSpotTotal = -1;
            //distance adjustment:
            //double dchange = R_orbit - (Math.Cos(theta) * R_planet);
            double last = -1;
            int wlindex = 1;

            Bitmap bmp = new Bitmap(wlct, (wlct / 2));

            double accumulateWL = 0;
            double rayang = Math.PI - 0; //orientation of integration is Z = direction of SUN
            using(Graphics g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(Point.Empty, bmp.Size));
                Pen mpen = new Pen(Color.White, 12.0f);
                Pen spen = new Pen(Color.White,  5.0f);
                for (int h = 0; h < bmp.Height; h += bmp.Height / 20)
                {
                    g.DrawLine(spen, new Point(0, h), new Point(bmp.Width, h));
                }
                for (double wl = wlmin + dwavelength; wl < wlmax - dwavelength; wl += dwavelength)
                {
                    Pen pwl = new Pen(KColor4.fullspecWavelengthRGB(wl).ToSystemColor());
                    double wlintens = wlIntStorage[wlindex];

                    double a = 0;
                    if (last != -1) { a = last; }
                    else
                    {
                        a = falloffHandle * wlIntStorage[wlindex - 1]
                            * Math.Pow(Math.E, -1 * Atmospherics.std_OpticalPathLength(10.0 + R_planet, rayang, wl, 2, 30000, R_planet));
                    }

                    double b = falloffHandle * wlintens * Math.Pow(Math.E, -1 * Atmospherics.std_OpticalPathLength(10.0 + R_planet, rayang, wl + dwavelength, 2, 30000, R_planet));

                    last = b;
                    //area of a trapezoid approximation to integrate: (a+b)dtheta / 2

                    double area0 = 0.5 * dwavelength * (a + b) / 10E+5; 
                    accumulateWL += area0;
                    g.DrawLine(new Pen(Color.AliceBlue), new Point(wlindex - 1, bmp.Height),
                        new Point(wlindex - 1, bmp.Height - (int)(bmp.Height * (falloffHandle * wlIntStorage[wlindex] / 2000000000))));
                    g.DrawLine(pwl, new Point(wlindex - 1, bmp.Height), 
                        new Point(wlindex - 1, bmp.Height - (int)(bmp.Height * ((a / 2000000000)))));
                    wlindex++;
                }
                if (LastSpotTotal != -1)
                {
                    double area = Math.Cos(0) * 0.5 * (dtheta * (LastSpotTotal + accumulateWL));
                    accumulateSurf += area;
                }
                else { }

                //draw lines
                Pen wpen = new Pen(Color.White, 14.0f);
                g.DrawLine(wpen, new Point(0, 0), new Point(bmp.Width / 40, 0));
                g.DrawLine(wpen, new Point(0, bmp.Height/2), new Point(bmp.Width / 40, bmp.Height/2));
                g.DrawLine(wpen, new Point(0, bmp.Height), new Point(bmp.Width / 40, bmp.Height));
                for (int h = 0; h < bmp.Height; h+= bmp.Height / 20)
                {
                    g.DrawLine(mpen, new Point(0, h), new Point(bmp.Width / 80, h));
                }
            }
            LastSpotTotal = accumulateWL;
            DisplayRenderMain.Image = bmp;
            Console.WriteLine(LastSpotTotal);
            bmp.Save("plotsolvtransmitpower.png");
        }

        //full integration of transmittance over the planet surface
        public void Test13()
        {
            //do spherical integration over entire planet surface.
            //first I need a quick table of wavelength intensity per square surface meter of blackbody? maybe
            //since this problem can be arranged to be spherically symmetric, we only need one integral for the surface component
            double T_sun = 5778; //kelvin
            double R_sun = 695700000; //meters
            double R_planet = 6371000; //meters
            double R_orbit = 1.496E+11; //meters

            double lpart = Physics.lumPartial(T_sun);

            //bake table of bbody intensity:
            //start at arb value 0.1 um
            //upper is harder bc the falloff is so slow, most activity is low enough around 4 um



            //                                                  \/ this is to account for planet "normal"
            //integrate: 4 pi R_planet^2 * from (0 to pi / 2) * cos(theta) * I(wavelength) dtheta

            //I(wl) = (sun wl intensity from table) (transmittance) (distance falloff ratio)
            //transmittance = e ^ -OPL

            double falloffHandle = (R_sun * R_sun) / (R_orbit * R_orbit);


            double dtheta = Math.PI / 2 / 100.0;
            double accumulateSurf = 0;

            double dwavelength = 0.005;
            double wlmin = 0.2; double wlmax = 5.2;
            int wlct = (int)((wlmax - wlmin) / dwavelength);


            //create quick wavelength table
            double[] wlIntStorage = new double[wlct];

            for (int wlc = 0; wlc < wlct; wlc++)
            {
                double currwl = ((double)wlc * dwavelength) + wlmin;
                wlIntStorage[wlc] = Physics.planckLaw(T_sun, currwl, 'u');
            }

            double LastSpotTotal = -1;
            //begin integrate.
            for (double theta = 0; theta < Math.PI / 2; theta += dtheta)
            {
                //get view angle: 
                double rayang = Math.PI - theta; //orientation of integration is Z = direction of SUN

                //distance adjustment:
                //double dchange = R_orbit - (Math.Cos(theta) * R_planet);
                double last = -1;
                int wlindex = 1;
                
                double accumulateWL = 0;
                for (double wl = wlmin + dwavelength; wl < wlmax - dwavelength; wl += dwavelength)
                {
                    double wlintens = wlIntStorage[wlindex];

                    double a = 0;
                    if (last != -1) { a = last; }
                    else
                    {
                        a = wlIntStorage[wlindex - 1]
                            * Math.Pow(Math.E, -1 * Atmospherics.std_OpticalPathLength(10.0 + R_planet, rayang, wl, 2, 30000, R_planet)); 
                    }

                    double b = wlintens * Math.Pow(Math.E, -1 * Atmospherics.std_OpticalPathLength(10.0 + R_planet, rayang, wl + dwavelength, 2, 30000, R_planet));

                    last = b;

                    //area of a trapezoid approximation to integrate: (a+b)dtheta / 2
                    
                    //accumulate += area;
                    accumulateWL += 0.5 * dwavelength * (a + b) / 10E+5;
                    wlindex++;
                }
                if(LastSpotTotal != -1)
                {
                    double area = Math.Cos(theta) * 0.5 * (dtheta * (LastSpotTotal + accumulateWL));
                    accumulateSurf += area;
                }
                else { }
                LastSpotTotal = accumulateWL;
            }

            double totalPow = accumulateSurf * 4 * R_planet * R_planet * falloffHandle;

            Console.WriteLine(totalPow);
            //Console.WriteLine(lpart);
        }

        //test of Plank's Law function
        public void Test12()
        {
            //Console.WriteLine(Physics.lumPartial(5778));
            //Console.WriteLine(Atmospherics.std_OpticalPathLength())
        }
        
        //bake equirectangular map of shader
        public void Test11()
        {
            double p_radius = 6371.0;
            Sphere sphere = new Sphere(new Vector3(0.0, 0.0, 0.0), p_radius);
            sphere.SHADER = new PlanetShader(p_radius);
            int dres = 4;
            KColorImage bake = new KColorImage(2048 / dres, 1024 / dres);
            for (int y = 0; y < bake.height; y++)
            {
                //double theta = ((double)y / bake.height + 0.5) / 2 * Math.PI;
                double theta = (1.0 - (double)y / bake.height) * Math.PI;
                for (int x = 0; x < bake.width; x++)
                {
                    double phi = (double)x / bake.width * Math.PI * 2;
                    Vector3 pos = p_radius * new Vector3(Math.Cos(phi), Math.Sin(phi), 0);
                    double r = Math.Sqrt(pos.X * pos.X + pos.Y * pos.Y);
                    pos.Z = Math.Sin(theta - Math.PI / 2) * p_radius;
                    bake.SetPoint(x, y, sphere.SHADER.Diffuse(pos));
                }
            }
            Bitmap bakebmp = bake.ToSystemBitmap(); bakebmp.Save("shader_diffuse.png");
            DisplayRenderMain.Image = bakebmp;
        }

        //takes starfield data from REGS and turns it into a KCOLIMAG
        public void Test10()
        {
            RGalaxy kjianoaa = RGalaxy.FromFolder("D:\\Desktop Control\\VisualStudio\\V C# Net Projects\\REGS\\REGS\\bin\\Debug\\kjianoaa");

            Vector3 vpos = Vector3.Zero;
            KColorImage kci = new KColorImage(2048, 1024);
            for(int spc = 0; spc < kjianoaa.system_points.Length; spc++)
            {
                SystemPointStorage sp = kjianoaa.system_points[spc];
                double xr = sp.X - vpos.X;
                double yr = sp.Y - vpos.Y;
                double zr = sp.Z - vpos.Z;

                double dist = Math.Sqrt(xr * xr + yr * yr + zr * zr);

                double kxp;
                double kyp;

                double phi = 0;
                if (xr < 0)
                { phi = Math.PI + Math.Atan(yr / xr); }
                else
                { phi = Math.Atan(yr / xr); }

                //double r = Math.Sqrt(point.X * point.X + point.Y * point.Y);
                //double thet = Math.PI - (Math.Atan(point.Z / r) + Math.PI / 2);
            }
        }
        
        //full integration of power over the planet surface, not including atmospheric effects
        public void Test9()
        {
            //do spherical integration over entire planet surface.
            //first I need a quick table of wavelength intensity per square surface meter of blackbody? maybe
            //since this problem can be arranged to be spherically symmetric, we only need one integral for the surface component
            double T_sun = 5778; //kelvin
            double R_sun = 695700000; //meters
            double R_planet = 6371000; //meters
            double R_orbit = 1.496E+11; //meters

            double lpart = Physics.lumPartial(5778);

            //bake table of bbody intensity:
            //start at arb value 0.1 um
            //upper is harder bc the falloff is so slow, most activity is low enough around 4 um

            //double dwavelength = 0.001;
            //double wlmin = 0.1; double wlmax = 4.0;
            //double wlct = (wlmax - wlmin) / dwavelength;
            //for(double wl = wlmin; wl < wlmax; wl += dwavelength)
            //{
            //
            //}


            //                                                  \/ this is to account for planet "normal"
            //integrate: 4 pi R_planet^2 * from (0 to pi / 2) * cos(theta) * I(wavelength) dtheta
            
            //I(wl) = (sun wl intensity from table) (transmittance) (distance falloff ratio)
            //transmittance = e ^ -OPL

            double falloffHandle = (R_sun * R_sun) / (R_orbit * R_orbit);


            double dtheta = Math.PI / 2 / 10000.0;
            double accumulate = 0;

            //begin integrate.
            for(double theta = 0; theta < Math.PI / 2; theta += dtheta)
            {
                //get view angle: 
                double rayang = Math.PI - theta; //orientation of integration is Z = direction of SUN

                //distance adjustment:
                //double dchange = R_orbit - (Math.Cos(theta) * R_planet);

                //area of a trapezoid approximation to integrate: (a+b)dtheta / 2
                double area = 0.5 * (dtheta * (Math.Cos(theta) + Math.Cos(theta + dtheta)));
                accumulate += area;
            }

            double totalPow = accumulate * 4 * lpart * R_planet * R_planet * falloffHandle;

            Console.WriteLine(totalPow);
        }

        //plotting surface values for OPL
        public void Test8()
        {
            double theta = Math.PI;// / 2; //angle between center vector and raycast
            double RADIUS = 6371000.0; //radius of planet in question
            double wavelength = 0.490; //micrometers
            double d = RADIUS + 2;     //distance from raycast init to center of object
            double upper = 1000000;

            string inputFileCont = File.ReadAllText("oplInputChart.tsv");
            SpreadsheetHandler inputs = new SpreadsheetHandler(inputFileCont, 81, 2, '\t');
            SpreadsheetHandler outSheet = new SpreadsheetHandler(81, 3);
            outSheet.sheet[0, 0] = inputs.sheet[0, 0];
            outSheet.sheet[1, 0] = inputs.sheet[1, 0];
            outSheet.sheet[2, 0] = "KiraliOPLresult";

            for (int inRow = 1; inRow < inputs.Height; inRow++)
            {
                outSheet.sheet[0, inRow] = inputs.sheet[0, inRow];
                outSheet.sheet[1, inRow] = inputs.sheet[1, inRow];

                double currWav = 0.2;
                Double.TryParse(inputs.sheet[0, inRow], out currWav);

                double pathLength = Atmospherics.std_OpticalPathLength(d, theta, currWav, 1, upper);
                outSheet.sheet[2, inRow] = pathLength.ToString();

                Console.WriteLine("W: " + Math.Round(currWav, 2) + "  OPL: " + pathLength);
            }


            string saveConts = outSheet.SerializeSheet();
            File.WriteAllText("kirExportPaths.tsv", saveConts);
        }

        //determining optical depth as a function of Theta
        public void Test7()
        {
            double theta = Math.PI;// / 2; //angle between center vector and raycast
            double RADIUS = 6371000.0; //radius of planet in question
            double wavelength = 0.490; //micrometers
            double d = RADIUS + 1000000;     //distance from raycast init to center of object
            double upper = 10000000;
            Sphere sphere = new Sphere(new Vector3(0.0, 0.0, 0.0), RADIUS);

            Bitmap bmp = new Bitmap(1920 / 2, 1080 / 2);
            Pen fpen = new Pen(Color.White, 2.0f);


            //finds the point at which the ray would hit the sphere, if applicable.
            Vector3 hitPoint = sphere.NearHit(new Vector3(-1 * d, 0.0, 0.0), new Vector3(Math.Cos(theta), Math.Sin(theta), 0.0));
            bool didHit = false;
            Vector3 normalAt = Vector3.Zero;
            if (hitPoint.Form == Vector3.VectorForm.POSITION) { didHit = true; normalAt = sphere.Grad(hitPoint); normalAt.Form = Vector3.VectorForm.NORMAL; }
            else { normalAt.Form = Vector3.VectorForm.INFINITY; }
            Vector3 reflAt = Vector3.Bounce(new Vector3(Math.Cos(theta), Math.Sin(theta), 0.0), normalAt);


            //draw spectrum and OPL for each fun little wavelength!
            using (Graphics gh = Graphics.FromImage(bmp))
            {
                //SCATTER FUNCTION DRAW
                Point lastPos0 = new Point(0, 0);
                Point lastPos1 = new Point(0, 0);
                double f_min = 0;
                double f_max = 1;
                double f_range = f_max - f_min;
                Pen f_pen = new Pen(Color.Black);


                //Dat DRW
                SolidBrush datapointbr = new SolidBrush(Color.Black);
                for (int y = 0; y < bmp.Height; y++)
                {
                    double v = (((double)y / bmp.Height));
                    double f = 0.7 * Math.Pow(Interpolate.Smooth(0, 1.0, v), 6);
                    Pen ps = new Pen((f * KColor4.fullspecWavelengthRGB(wavelength)).ToSystemColor());
                    gh.DrawLine(ps, new Point(0, y), new Point(bmp.Width, y));
                }

                for (int x = 0; x < bmp.Width; x+=3)
                {
                    //double fv0 = Atmospherics.std_crossPerMolecule(wl, 'u', 0);
                    double angle = Math.PI * (1.0 - (double)x / bmp.Width);
                    double fv1 = Atmospherics.std_OpticalPathLength(d, angle, wavelength, 1, upper);
                    Point newPos1 = new Point(x, (int)(bmp.Height * (1.0 - (fv1 - f_min) / f_range)));
                    //Console.WriteLine("theta:" + Math.Round(angle * 360.0 / (Math.PI * 2.0), 2) + " tau:" + Math.Round(fv1, 2));
                    f_pen = fpen;
                    try
                    {
                        //gh.DrawLine(f_pen, lastPos0, newPos0);
                        gh.DrawLine(f_pen, lastPos1, newPos1);
                    }
                    catch { }
                    //lastPos0 = newPos0;
                    double deg = angle * 360 / (Math.PI * 2.0);
                    if (Math.Abs(((int)(deg) % 30)) < 0.5 * (1.0 / bmp.Width) * 360 / (Math.PI * 2.0))
                    {
                        gh.DrawLine(fpen, new Point(x, bmp.Height), new Point(x, bmp.Height - 25));
                        //Console.WriteLine("h: " + current.X + "p" + current.Y);
                    } else if (Math.Abs(((int)(deg) % 5)) < 0.5 * (1.0 / bmp.Width) * 360 / (Math.PI * 2.0))
                    {
                        gh.DrawLine(fpen, new Point(x, bmp.Height), new Point(x, bmp.Height - 15));
                        //Console.WriteLine("h: " + current.X + "p" + current.Y);
                    }
                    lastPos1 = newPos1;
                }

                double thetaMax = Math.Asin(RADIUS / d);
                int xFrom = (int)(bmp.Width * (1.0 - (thetaMax / Math.PI)));
                gh.DrawLine(fpen, new Point(xFrom, bmp.Height), new Point(xFrom, 0));
            }

            DisplayRenderMain.Image = bmp;
            bmp.Save("opticpath\\fdraw_opticDepth_l" + wavelength + "um_z" + Math.Round(d - RADIUS) + "m.png");
        }

        //determining optical depth
        public void Test6()
        {
            double theta = Math.PI;// / 2; //angle between center vector and raycast
            double RADIUS = 6371000.0; //radius of planet in question
            double wavelength = 0.20; //micrometers
            double d = RADIUS + 2;     //distance from raycast init to center of object
            double upper = 10000000;
            Sphere sphere = new Sphere(new Vector3(0.0, 0.0, 0.0), RADIUS);

            //finds the point at which the ray would hit the sphere, if applicable.
            Vector3 hitPoint = sphere.NearHit(new Vector3(-1 * d, 0.0, 0.0), new Vector3(Math.Cos(theta), Math.Sin(theta), 0.0));
            bool didHit = false;
            Vector3 normalAt = Vector3.Zero;
            if (hitPoint.Form == Vector3.VectorForm.POSITION) { didHit = true; normalAt = sphere.Grad(hitPoint); normalAt.Form = Vector3.VectorForm.NORMAL; }
            else { normalAt.Form = Vector3.VectorForm.INFINITY; }
            Vector3 reflAt = Vector3.Bounce(new Vector3(Math.Cos(theta), Math.Sin(theta), 0.0), normalAt);

            Bitmap bmp = new Bitmap(1920 / 2, 1080 / 2);
            Pen fpen = new Pen(Color.White, 2.0f);

            double wlmin = 0.23;
            double wlmax = 1.0;

            //draw spectrum and OPL for each fun little wavelength!
            using (Graphics gh = Graphics.FromImage(bmp))
            {
                //SPECT DRAW
                double wl = wavelength;
                double wlch = (wlmax - wlmin) / bmp.Width;
                Pen ps;

                //SCATTER FUNCTION DRAW
                Point lastPos0 = new Point(0, 0);
                Point lastPos1 = new Point(0, 0);
                double f_min = 0;
                double f_max = 5;
                double f_range = f_max - f_min;
                Pen f_pen = new Pen(Color.Black);


                //Dat DRW
                SolidBrush datapointbr = new SolidBrush(Color.Black);

                for (int x = 0; x < bmp.Width; x++)
                {
                    //spectrum draw
                    wl = wlmin + x * wlch;
                    ps = new Pen(KColor4.fullspecWavelengthRGB(wl).ToSystemColor());
                    gh.DrawLine(ps, new Point(x, 0), new Point(x, bmp.Height));

                    //double fv0 = Atmospherics.std_crossPerMolecule(wl, 'u', 0);
                    double fv1 = Atmospherics.std_OpticalPathLength(d, theta, wl);
                    //Console.WriteLine(wl + " : " + fv1);
                    //Point newPos0 = new Point(x, (int)(disp.Height * (1.0 - (fv0 - f_min) / f_range)));
                    Point newPos1 = new Point(x, (int)(bmp.Height * (1.0 - (fv1 - f_min) / f_range)));
                    f_pen = new Pen(KColor4.fullspecWavelengthRGB(wl).Flip().ToSystemColor(), 2.5f);
                    try
                    {
                        //gh.DrawLine(f_pen, lastPos0, newPos0);
                        gh.DrawLine(f_pen, lastPos1, newPos1);
                    }
                    catch { }
                    //lastPos0 = newPos0;
                    lastPos1 = newPos1;
                }
            }

            //draw posistion and orientation little cute diagram!
            using (Graphics g1 = Graphics.FromImage(bmp))
            {
                //g1.FillRectangle(new SolidBrush(Color.Black), new Rectangle(new Point(0, 0), bmp.Size));
                int rE = bmp.Height / 5;
                int posxE = 3 * bmp.Width / 4 - rE;
                int posyE = bmp.Height / 4 - rE;
                Point cCenter = new Point(posxE, posyE);
                g1.DrawEllipse(fpen, new Rectangle(cCenter, new Size(rE * 2, rE * 2)));
                cCenter = new Point(posxE + rE, posyE + rE);
                Point rStart = new Point(posxE - (int)(d / RADIUS * rE) + rE, posyE + rE);
                g1.DrawLine(fpen, cCenter, rStart);
                Point rEnd;
                if (!didHit)
                {
                    rEnd = new Point(rStart.X + (int)(Math.Cos(theta) * upper / RADIUS * rE), rStart.Y - (int)(Math.Sin(theta) * upper / RADIUS * rE));
                    g1.DrawLine(fpen, rEnd, rStart);
                }
                else
                {
                    double length = (hitPoint - new Vector3(-1 * d, 0.0, 0.0)).Length();
                    rEnd = new Point((int)(Math.Cos(theta) * length / RADIUS * rE), (int)(-1 * Math.Sin(theta) * length / RADIUS * rE));
                    rEnd.X += rStart.X;
                    rEnd.Y += rStart.Y;
                    g1.DrawLine(fpen, rEnd, rStart);
                    rStart.X = rEnd.X + (int)(reflAt.X * ((upper - length) / RADIUS) * rE);
                    rStart.Y = rEnd.Y + (int)(-1 * reflAt.Y * ((upper - length) / RADIUS) * rE);
                    g1.DrawLine(fpen, rEnd, rStart);
                }


            }

            DisplayRenderMain.Image = bmp;
           bmp.Save("fdraw_opticDepthWLChart.png");
        }

        //determining pressure depth
        public void Test5()
        {
            double theta = Math.PI / 4;// / 2; //angle between center vector and raycast
            double RADIUS = 6371000.0; //radius of planet in question
            Sphere sphere = new Sphere(new Vector3(0.0, 0.0, 0.0), RADIUS);
            double d = Math.Sqrt(2) * RADIUS + 2000;     //distance from raycast init to center of object
            double wavelength = 0.640; //micrometers

            //finds the point at which the ray would hit the sphere, if applicable.
            Vector3 hitPoint = sphere.NearHit(new Vector3(-1 * d, 0.0, 0.0), new Vector3(Math.Cos(theta), Math.Sin(theta), 0.0));
            bool didHit = false;
            double distHit = (hitPoint - new Vector3(-1 * d, 0.0, 0.0)).Length();
            Vector3 normalAt = Vector3.Zero;
            if (hitPoint.Form == Vector3.VectorForm.POSITION) { didHit = true; normalAt = sphere.Grad(hitPoint); normalAt.Form = Vector3.VectorForm.NORMAL; }
            else { normalAt.Form = Vector3.VectorForm.INFINITY; }
            Vector3 reflAt = Vector3.Bounce(new Vector3(Math.Cos(theta), Math.Sin(theta), 0.0), normalAt);

            //this is a f(x) & x use of Vector3 just to store data.
            Vector3 current = Vector3.Zero;
            Vector3 next = Vector3.Zero; 

            //our integration precision \/
            double drange = 100;
            double upper = 50000000;
            double Accumulate = 0;
            double range = 0.0;
            double h_next = Math.Sqrt(range * range + d * d + 2 * range * d * Math.Cos(Math.PI - theta)) - RADIUS;

            //run function for current
            current.X = range;
            current.Y = Atmospherics.std_airPressureApprox(h_next);
            //Console.WriteLine(h_next + " : " + current.Y);
            /*
            current.Y = Atmospherics.std_crossBulk(wavelength,
                Atmospherics.std_airPressureApprox(h_next),
                Atmospherics.std_roughAltTemp(h_next));
            */
            Bitmap bmp = new Bitmap(1920, 1080);
            Pen fpen = new Pen(Color.White, 2.0f);
            using (Graphics g1 = Graphics.FromImage(bmp))
            {
                g1.FillRectangle(new SolidBrush(Color.Black), new Rectangle(new Point(0, 0), bmp.Size));
                int rE = bmp.Height / 5;
                int posxE = 5 * bmp.Width / 6 - rE;
                int posyE = bmp.Height / 4 - rE;
                Point cCenter = new Point(posxE, posyE);
                g1.DrawEllipse(fpen, new Rectangle(cCenter, new Size(rE * 2, rE * 2)));
                cCenter = new Point(posxE + rE, posyE + rE);
                Point rStart = new Point(posxE - (int)(d / RADIUS * rE) + rE, posyE + rE);
                g1.DrawLine(fpen, cCenter, rStart);
                Point rEnd;
                if (!didHit) 
                {
                    rEnd = new Point((int)(Math.Cos(theta) * upper / RADIUS * rE), (int)(-1 * Math.Sin(theta) * upper / RADIUS * rE));
                    rEnd.X += rStart.X;
                    rEnd.Y += rStart.Y;
                    g1.DrawLine(fpen, rEnd, rStart);
                }
                else
                {
                    double length = (hitPoint - new Vector3(-1 * d, 0.0, 0.0)).Length();
                    rEnd = new Point((int)(Math.Cos(theta) * length / RADIUS * rE), (int)(-1 * Math.Sin(theta) * length / RADIUS * rE));
                    rEnd.X += rStart.X;
                    rEnd.Y += rStart.Y;
                    g1.DrawLine(fpen, rEnd, rStart);
                    rStart.X = rEnd.X + (int)(reflAt.X * ((upper - length) / RADIUS) * rE);
                    rStart.Y = rEnd.Y + (int)(-1 * reflAt.Y * ((upper - length) / RADIUS) * rE);
                    g1.DrawLine(fpen, rEnd, rStart);
                }
                

            }

            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, 0);
            double rmax = 1500;//Atmospherics.std_airPressureApprox(h_next) * 2;
            double rmin = 0;
            double dmax = RADIUS + 1000000;
            double dmin = RADIUS - 1000000;


            bool doBounce = false;
            bool drV = false;
            int xdv = 0;
            double travelled = 0.0;

            using (Graphics g1 = Graphics.FromImage(bmp))
            {
                for (range = 0.0; travelled < upper; travelled += drange)
                {
                    //run function for next
                    
                    if (!doBounce)
                    {
                        next.X = range + drange;
                        range += drange;
                    }
                    else { range -= drange; next.X = range - drange; }
                    h_next = Math.Sqrt(next.X * next.X + d * d + 2 * next.X * d * Math.Cos(Math.PI - theta)) - RADIUS;
                    if(h_next < 0 && didHit)
                    {
                        doBounce = true; drV = true;
                        range -= 2 * drange;
                        h_next = Math.Sqrt(next.X * next.X + d * d + 2 * next.X * d * Math.Cos(Math.PI - theta)) - RADIUS;
                    }
                    next.Y = Atmospherics.std_airPressureApprox(h_next);
                    /*
                    next.Y = Atmospherics.std_crossBulk(wavelength,
                             Atmospherics.std_airPressureApprox(h_next),
                             Atmospherics.std_roughAltTemp(h_next));
                    */
                    //double Area = drange * (current.Y + next.Y) / 2.0;
                    //Accumulate += Area;
                    current.X = next.X;
                    current.Y = next.Y;
                    //Console.WriteLine(range + " : " + h_next + " : " + current.Y);

                    //drawing:
                    p2.X = (int)(bmp.Width * (travelled - dmin) / (dmax - dmin));
                    p2.Y = (int)(bmp.Height * ((rmax - next.Y) / (rmax - rmin)));
                    g1.DrawLine(fpen, p1, p2);
                    p1.X = p2.X;
                    p1.Y = p2.Y;

                    if(Math.Abs(((int)(h_next) % 5000)) < drange)
                    {
                        g1.DrawLine(fpen, new Point(p1.X, bmp.Height), new Point(p1.X, bmp.Height - 25));
                        //Console.WriteLine("h: " + current.X + "p" + current.Y);
                    } else if(Math.Abs(((int)(h_next) % 500)) < drange)
                    {
                        g1.DrawLine(fpen, new Point(p1.X, bmp.Height), new Point(p1.X, bmp.Height - 15));
                    }

                    if (drV)
                    {
                        g1.DrawLine(fpen, new Point(p1.X, bmp.Height), new Point(p1.X, 0));
                        xdv = p1.X;
                        drV = false;
                    }
                }

                for(double y = 0; y < rmax; y+= 1013.15 / 10)
                {
                    Point pz = new Point(xdv - 10, (int)(bmp.Height * ((rmax - y) / (rmax - rmin))));
                    Point pe = new Point(xdv + 10, (int)(bmp.Height * ((rmax - y) / (rmax - rmin))));

                    g1.DrawLine(fpen, pz, pe);
                }
            }
            Console.WriteLine(Accumulate * range);
            DisplayRenderMain.Image = bmp;
            bmp.Save("fdraw_pressureAtr.png");
        }

        //Shader and sphere rendering section
        public void Test4()
        {
            double p_radius = 6371.0;
            DateTime dtStart = DateTime.Now;
            Sphere sphere = new Sphere(new Vector3(0.0, 0.0, 0.0), p_radius);

            Vector3 camerapos = new Vector3(0, p_radius * 4.5, 0);
            //Vector3 camerapos = new Vector3(0, 0, p_radius * 4.5);
            Vector3 camerarot = new Vector3(0, 0, 0);
            //Vector3 camerarot = new Vector3(Math.PI/2, 0, 0);
            //Vector3 camerarot = new Vector3(Math.PI / 3, -1 * Math.PI / 4, 0);

            //Vector3 LAMP = new Vector3(-2.1, -3.2, -1.3).Normalize();
            Vector3 LAMP_POS = new Vector3(-5 * p_radius, -5 * p_radius, 1); double LAMP_LUM = 0.5 * 6455548880.0;
            int dres = 2;
            Camera MainCamera = new Camera(camerapos, camerarot, 1920 / dres, 1080 / dres);


            MainCamera.RotateThet(Math.PI / 2);
            //WMainCamera.RotatePhi(Math.PI / 6);
            MainCamera.RotateR(Math.PI * (1 + 0.15));

            KColorImage renderImageRaw = new KColorImage(MainCamera.Width, MainCamera.Height);
            renderImageRaw.Fill(KColor4.BLACK);
            Bitmap vmp = new Bitmap(MainCamera.Width, MainCamera.Height);
            int samplesMax = 4;
            double dist = camerapos.Length();

            //Shader!
            //Bitmap earthPic = new Bitmap("earth.jpg");

            //sphere.SHADER = new RectSphereMap(p_radius, KColorImage.FromSystemBitmap(earthPic));
            sphere.SHADER = new PlanetShader(p_radius);

            //the angle limiter only renders pixels near to the object's position
            bool useAngleLimiter = true;
            double angleLimit = Math.PI / 48 + Math.Asin(p_radius / MainCamera.position.Length());
            double sunAngleLimit = Math.Asin(100 / MainCamera.position.Length());

            for (int frame = 0; frame < 1; frame++)
            {

                sphere.RotateR(frame * Math.PI / 20.0);

                for (int y = 0; y < vmp.Height; y++)
                {
                    for (int x = 0; x < vmp.Width; x++)
                    {
                        int sn = samplesMax;
                        KColor4[] samples = new KColor4[sn];
                        bool dosetPoint = true;

                        for (int s = 0; s < sn; s++)
                        {
                            double bright = 0;
                            Vector3 currentCameraVector = MainCamera.GetRayCast(x, y);
                            //do point lamp draw
                            double betlamp = Vector3.Between(LAMP_POS - camerapos, currentCameraVector);
                            if(betlamp < sunAngleLimit)
                            {
                                
                            }

                            //do angle limiter?
                            double bet = Vector3.Between(sphere.position - camerapos, currentCameraVector);
                            if (useAngleLimiter && bet < angleLimit)
                            {
                                Vector3 hitClose = sphere.NearHit(camerapos, currentCameraVector);
                                if (hitClose.Form != Vector3.VectorForm.INFINITY && hitClose.Form == Vector3.VectorForm.POSITION)
                                {
                                    double lampdist = (LAMP_POS - hitClose).Length();
                                    bright = -1 * LAMP_LUM/(lampdist * lampdist) * Vector3.Dot((LAMP_POS - hitClose).Normalize(), sphere.Grad(hitClose));
                                    //bright = -1 * Vector3.Dot(LAMP, sphere.Grad(hitClose));
                                    //planet
                                    //samples[s] = sphere.SHADER.Emit(hitClose) + sphere.SHADER.Diffuse(hitClose) * bright;// + path * new KColor4(0.7, 0.7, 0.9);

                                    samples[s] = sphere.SHADER.Diffuse(sphere.GetNewMap(hitClose)) * bright;


                                    //facing render
                                    //samples[s] = Vector3.Dot(-1 * currentCameraVector, sphere.Grad(hitClose)) * new KColor4(1.0, 1.0, 1.0);

                                    //fresnel
                                    //samples[s] = Vector3.Dot(currentCameraVector, Vector3.Bounce(currentCameraVector, sphere.Grad(hitClose))) * new KColor4(1.0, 1.0, 1.0);

                                    //samples[s] = Vector3.Dot(LAMP, -1 * sphere.SHADER.Normal(hitClose)) * new KColor4(1.0, 1.0, 1.0);


                                }
                                else
                                {
                                    samples[s] = new KColor4(0.0, 0.0, 0.0, 1.0);
                                }
                            }
                            else if(useAngleLimiter)
                            {
                                dosetPoint = false;
                            }
                            if (!useAngleLimiter)
                            {
                                Vector3 hitClose = sphere.NearHit(camerapos, currentCameraVector);
                                if (hitClose.Form != Vector3.VectorForm.INFINITY && hitClose.Form == Vector3.VectorForm.POSITION)
                                {
                                    //bright = -1 * Vector3.Dot(LAMP, sphere.Grad(hitClose));
                                    //planet
                                    //samples[s] = sphere.SHADER.Emit(hitClose) + sphere.SHADER.Diffuse(hitClose) * bright;// + path * new KColor4(0.7, 0.7, 0.9);

                                    samples[s] = sphere.SHADER.Diffuse(sphere.GetNewMap(hitClose)) * bright;


                                    //facing render
                                    //samples[s] = Vector3.Dot(-1 * currentCameraVector, sphere.Grad(hitClose)) * new KColor4(1.0, 1.0, 1.0);

                                    //fresnel
                                    //samples[s] = Vector3.Dot(currentCameraVector, Vector3.Bounce(currentCameraVector, sphere.Grad(hitClose))) * new KColor4(1.0, 1.0, 1.0);

                                    //samples[s] = Vector3.Dot(LAMP, -1 * sphere.SHADER.Normal(hitClose)) * new KColor4(1.0, 1.0, 1.0);


                                }
                                else
                                {
                                    samples[s] = new KColor4(0.0, 0.0, 0.0, 1.0);
                                }
                            }
                        }
                        if (dosetPoint)
                        {
                            renderImageRaw.SetPoint(x, y, KColor4.Average(samples));
                        }
                    }
                }
                //KColorImage bloomIm = renderImageRaw.GetBloomMapped(15, 0.0005);

                vmp = renderImageRaw.ToSystemBitmap();

                TimeSpan renderTimeElapse = DateTime.Now - dtStart;

                vmp.Save("fastrenders\\render" + frame + "_" + samplesMax + "iter_" + Math.Round(renderTimeElapse.TotalMilliseconds) / 1000.0 + "sec.png");
            }
            DisplayRenderMain.Image = vmp;
        }

        //Spectrum and scattering graphing lab
        public void Test3(bool drawScatterFunction = false, double drawVertical = -1, bool drawDat = false)
        {
            double wlmin = 0.2;//0.23;
            double wlmax = 1.00;

            Bitmap disp = new Bitmap(1920 / 1, 1080 / 1);


            //Get drawdat data if needed:
            string sprcontents;
            SpreadsheetHandler sph = SpreadsheetHandler.Empty;
            if (drawDat)
            {
                sprcontents = File.ReadAllText("atmdat.tsv");
                sph = new SpreadsheetHandler(sprcontents, 81, 4); //hopefully this works its old RiftEngine code ;)
            }

            //debug purposes map fits
            SpreadsheetHandler diff;
            string[] diffContainer = new string[81 * 4];
            diffContainer[0] = "Wavelength_um";
            diffContainer[1] = "sigma0";
            diffContainer[2] = "calculated_s2";
            diffContainer[3] = "s0/s2";


            using (Graphics gh = Graphics.FromImage(disp))
            {
                //SPECT DRAW
                double wl = wlmin;
                double wlch = (wlmax - wlmin) / disp.Width;
                Pen ps;

                //SCATTER FUNCTION DRAW
                Point lastPos0 = new Point(0, 0);
                Point lastPos1 = new Point(0, 0);
                double f_min = Atmospherics.std_crossPerMolecule(wlmax);
                double f_max = Atmospherics.std_crossPerMolecule(wlmin);
                double f_range = f_max - f_min;
                Pen f_pen = new Pen(Color.Black);


                //Dat DRW
                SolidBrush datapointbr = new SolidBrush(Color.Black);
                


                for (int x = 0; x < disp.Width; x++)
                {
                    //spectrum draw
                    wl = wlmin + x * wlch;
                    ps = new Pen(KColor4.fullspecWavelengthRGB(wl).ToSystemColor());
                    gh.DrawLine(ps, new Point(x, 0), new Point(x, disp.Height));

                    //draw scatterfunction
                    if (drawScatterFunction)
                    {
                        //double fv0 = Atmospherics.std_crossPerMolecule(wl, 'u', 0);
                        double fv1 = Atmospherics.std_crossPerMolecule(wl, 'u', 3);
                        //Point newPos0 = new Point(x, (int)(disp.Height * (1.0 - (fv0 - f_min) / f_range)));
                        Point newPos1 = new Point(x, (int)(disp.Height * (1.0 - (fv1 - f_min) / f_range)));
                        f_pen = new Pen(KColor4.fullspecWavelengthRGB(wl).Flip().ToSystemColor(), 2.5f);
                        try
                        {
                            //gh.DrawLine(f_pen, lastPos0, newPos0);
                            gh.DrawLine(f_pen, lastPos1, newPos1);
                        }
                        catch { }
                        //lastPos0 = newPos0;
                        lastPos1 = newPos1;
                    }
                }

                //draw data from spreadsheet
                if (drawDat && !sph.NULL)
                {
                    for (int row = 1; row < sph.Height; row++)
                    {
                        double fvx = 0;
                        double fvy = 0;

                        Double.TryParse(sph.GetCell(0, row), out fvx);
                        Double.TryParse(sph.GetCell(1, row), out fvy);

                        int xp = (int)(disp.Width * (fvx - wlmin) / (wlmax - wlmin));
                        int yp = (int)(disp.Height * (1.0 - (fvy - f_min) / f_range));

                        Point ctr = new Point(xp, yp);

                        datapointbr = new SolidBrush(KColor4.visibleWavelengthToRGB(fvx).Flip().ToSystemColor());
                        int pointSize = 10;
                        Rectangle r = new Rectangle(new Point(xp - pointSize / 2, yp - pointSize / 2), new Size(pointSize, pointSize));
                        gh.FillEllipse(datapointbr, r);

                        //report differences
                        double delt = (fvy / Atmospherics.std_crossPerMolecule(fvx, 'u', 3));
                        Console.WriteLine("Scatter Delta: " + Math.Round(1000 * fvx) + "nm, d: " + delt);
                        diffContainer[row * 4 + 0] = sph.sheet[0, row];
                        diffContainer[row * 4 + 1] = sph.sheet[1, row];
                        diffContainer[row * 4 + 2] = Atmospherics.std_crossPerMolecule(fvx, 'u', 1).ToString();
                        diffContainer[row * 4 + 3] = delt.ToString();
                    }
                    //export spreadsheet with error data:
                    diff = new SpreadsheetHandler(diffContainer, 81, 4);
                    string conts = diff.SerializeSheet();
                    File.WriteAllText("diffmaps.tsv", conts);
                }

                //Draw vertical line at input wavelength:
                if(drawScatterFunction && drawVertical != -1)
                {
                    Point tp = new Point((int)(disp.Width * (drawVertical - wlmin) / (wlmax - wlmin)), 0);
                    Point bp = new Point((int)(disp.Width * (drawVertical - wlmin) / (wlmax - wlmin)), disp.Height);

                    Console.WriteLine("ScatterConst: " + Atmospherics.std_crossPerMolecule(drawVertical, 'u', 3));

                    gh.DrawLine(new Pen(KColor4.fullspecWavelengthRGB(drawVertical).Flip().ToSystemColor(), 2f), tp, bp);
                }
            }


            DisplayRenderMain.Image = disp;
            if (!drawScatterFunction)
            {
                disp.Save("spectrum_render.png");
            }
            else
            {
                if (drawVertical != -1)
                {
                    disp.Save("spectrum_renderWfunc_At,"+ Math.Round(drawVertical * 1000.0) + "nm.png");
                }
                else
                {
                    disp.Save("spectrum_renderWfunc.png");
                }
            }
        }

        //SLOW attempt at rendering a solid sphere using Ray Tracing
        public void Test2()
        {
            SolidSphere exp = new SolidSphere(Vector3.Zero, 2);


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

        //Basic light refract and bounce lab test
        public void Test1()
        {

            //Vector3 posInit = new Vector3(7.8, -12.6, 0, Vector3.VectorForm.POSITION);
            //Vector3 dirInit = new Vector3(-5, 6, 0, Vector3.VectorForm.DIRECTION).Normalize();
            
            
            //Vector3 posInit = new Vector3(7.8, -12.6, 0, Vector3.VectorForm.POSITION);
            Vector3 posInit = new Vector3(6.0, 2.0, 0, Vector3.VectorForm.POSITION);
            Vector3 dirInit = new Vector3(-1.5, 6, 0, Vector3.VectorForm.DIRECTION).Normalize();


            LightRay lr = new LightRay(posInit, dirInit);
            Console.WriteLine("Light Ray init position: " + lr.Position.ToString(3));
            Console.WriteLine("Light Ray init direction: " + lr.Direction.ToString(3));
            //LinearFalloff lf_explicit = new LinearFalloff();

            SphereContainer lf_explicit = new SphereContainer();
            lr.SetMedium(lf_explicit, 0.0000005);
            lr.Intensity = 1.0;
            //lr.SetMedium(lf_explicit, 0.001);

            Bitmap vmp = new Bitmap(1920 / 1, 1080 / 1);
            double z = 40;

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
                int pathsct = 0; //for limiting bounces
                bool limitOperations = true; int oplim = 32;
                while (lr.Position.Length() < 35.0)
                {
                    int xc1 = (int)(((lr.Position.X / z) + 0.5) * vmp.Width);
                    int yc1 = (int)((lr.Position.Y / (z * ((double)vmp.Height / vmp.Width)) + 0.5) * vmp.Height);

                    lr.March(0.002, 35, false);

                    int xc2 = (int)(((lr.Position.X / z) + 0.5) * vmp.Width);
                    int yc2 = (int)((lr.Position.Y / (z * ((double)vmp.Height / vmp.Width)) + 0.5) * vmp.Height);

                    f.DrawLine(wpen, new Point(xc1, yc1), new Point(xc2, yc2));
                    int inc = (int)(255 * lr.Intensity);
                    wpen = new Pen(Color.FromArgb(inc, inc, inc));

                    if(Math.Round(lr.Position.X * 100) % 10 == 0)
                    {
                        //Console.WriteLine(lr.Intensity);
                    }

                    pathsct++;
                    if(limitOperations && oplim == pathsct)
                    {
                        break;
                    }
                }
            }

            vmp.Save("dualtest2 postfix" + lr.n_sensiget + ".png");
            DisplayRenderMain.Image = vmp;

        }
    }
}
