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
using Kirali.Environment.Shaders;
using Kirali.Environment.Render.Primatives;
using Kirali.Storage;

using REPT.Objects;


namespace REPT
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            //drawdat takes comparison values from exported data

            Test4();
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

        //determining optical depth as a function of Theta!!!!!!!!
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

        //determining optical depth!!!!!!!
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

        //determining pressure depth!!!!!!!
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

            Vector3 LAMP = new Vector3(-2.1, -3.2, -1.3).Normalize();
            int dres = 4;
            Camera MainCamera = new Camera(camerapos, camerarot, 1920 / dres, 1080 / dres);


            MainCamera.RotateThet(Math.PI / 2);
            //WMainCamera.RotatePhi(Math.PI / 6);
            MainCamera.RotateR(Math.PI * (1 + 0.15));

            KColorImage renderImageRaw = new KColorImage(MainCamera.Width, MainCamera.Height);
            renderImageRaw.Fill(KColor4.BLACK);
            Bitmap vmp = new Bitmap(MainCamera.Width, MainCamera.Height);
            int samplesMax = 1;
            double dist = camerapos.Length();

            //Shader!
            //Bitmap earthPic = new Bitmap("earth.jpg");

            //sphere.SHADER = new RectSphereMap(p_radius, KColorImage.FromSystemBitmap(earthPic));
            sphere.SHADER = new PlanetShader(p_radius);

            //the angle limiter only renders pixels near to the object's position
            bool useAngleLimiter = true;
            double angleLimit = Math.PI / 48 + Math.Asin(p_radius  / MainCamera.position.Length());


            for(int frame = 0; frame < 1; frame++)
            {

                //sphere.RotateR(frame * Math.PI / 20.0);

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
                            
                            //do angle limiter?
                            double bet = Vector3.Between(sphere.position - camerapos, currentCameraVector);
                            if (useAngleLimiter && bet < angleLimit)
                            {
                                Vector3 hitClose = sphere.NearHit(camerapos, currentCameraVector);
                                if (hitClose.Form != Vector3.VectorForm.INFINITY && hitClose.Form == Vector3.VectorForm.POSITION)
                                {
                                    bright = -1 * Vector3.Dot(LAMP, sphere.Grad(hitClose));
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
                                    bright = -1 * Vector3.Dot(LAMP, sphere.Grad(hitClose));
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
