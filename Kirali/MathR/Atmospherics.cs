using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Kirali.MathR
{
    public class Atmospherics
    {

        //implement later interpolation
        public static readonly double[] std_atm1976_heights = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 38, 40, 42, 44, 46, 48, 50, 52, 54, 56, 58, 60, 62, 64, 66, 68, 70, 72, 74, 76, 78, 80, 82, 84, 86, 88, 90, 92, 94, 96, 98, 100, 101, 102, 103, 104, 105, 106, 107, 108, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 135, 140, 145, 150, 155, 160, 165, 170, 175, 180, 185, 190, 195, 200, 210, 220, 230, 240, 250, 260, 270, 280, 290, 300 };
        public static readonly double[] std_atm1976_tempers = new double[] { 288.15, 281.65, 275.15, 268.65, 262.15, 255.65, 249.15, 242.65, 236.15, 229.65, 223.15, 216.65, 216.65, 216.65, 216.65, 216.65, 216.65, 216.65, 216.65, 216.65, 216.65, 217.65, 218.65, 219.65, 220.65, 221.65, 222.65, 223.65, 224.65, 225.65, 226.65, 227.65, 228.65, 231.45, 234.25, 236.51, 239.282, 245.45, 250.35, 255.878, 261.403, 266.925, 270.65, 270.65, 268, 264, 258, 253, 245, 240, 234, 229, 223, 217, 213, 209, 205, 201, 197, 193, 187, 187, 187, 187, 187, 188, 189, 192, 195, 197, 200, 202, 205, 208, 213, 218, 223, 240, 252, 264, 276, 288, 300, 312, 324, 336, 348, 360, 372, 384, 395, 406, 417, 428, 438, 449, 459, 469, 517, 560, 600, 634, 667, 696, 723, 748, 770, 790, 809, 825, 841, 855, 879, 899, 916, 930, 941, 951, 959, 966, 971, 976 };


        //WAVELENGTH NOTE KEEP UNITS CLEAR7

        /// <summary>
        /// <tooltip>Scattering Cross Section per molecule of standard air. Multiply by Number Density for volume.</tooltip>
        /// </summary>
        /// <param name="wavelength">
        /// <tooltip>Input Wavelength in MICROMETERS.</tooltip>
        /// </param>
        /// <returns></returns>
        public static double std_crossPerMolecule(double wavelength, char measure = 'u', int approxType = 3)
        {
            double wlcm = wavelength / 10000.0;
            double wlum = wavelength;

            switch (measure)
            {
                case 'u':
                    wlcm = wavelength / 10000.0;
                    wlum = wavelength;
                break;
                case 'n':
                    wlcm = wavelength / 10000000.0;
                    wlum = wavelength / 1000.0;
                    break;
                case 'c':
                    wlcm = wavelength;
                    wlum = wavelength / 10000.0;
                    break;
                default:
                    wlcm = wavelength / 10000.0;
                    wlum = wavelength;
                    break;
            }

            

            //for sigma approximation:
            double a, b, c, d;
            double sigma = 0;

            switch (approxType)
            {
                case 0: //second approx attempt using the consts described in the paper for wavelengths less than whatever
                    a = 3.01577E-28;
                    b = 3.55212;
                    c = 1.35579;
                    d = 0.11563;
                    //APPROXIMATION OF SIGMA (SCATTERING CROSS SECTION OF ONE "STD MOLECULE")
                    sigma = a * Math.Pow(wlum, -1 * (b + (c * wlum) * (d / wlum)));
                    return sigma;
                case 1: //second approx attempt using the consts described in the paper for wavelengths greater than than whatever
                    a = 4.01061E-28;
                    b = 3.99668;
                    c = 1.10298E-3;
                    d = 2.71393E-2;
                    //APPROXIMATION OF SIGMA (SCATTERING CROSS SECTION OF ONE "STD MOLECULE")
                    sigma = a * Math.Pow(wlum, -1 * (b + (c * wlum) * (d / wlum)));
                    return sigma;
                case 2: //first approximation using the full equation setup stuff
                    double N_s = 2.54743E+19; //number density of STD air
                    double n_s = std_refractiveatm(wavelength, measure); //Refractive index in std air at wavelength.
                    double p_n = std_depolanisotropy(wavelength, measure); //anisotropy / depolarization const

                    sigma = (24.0 * Math.Pow(Math.PI, 3) * Math.Pow(Math.Pow(n_s, 2) - 1, 2)) / (Math.Pow(wlcm, 4) * Math.Pow(N_s, 2) * Math.Pow(Math.Pow(n_s, 2) + 2, 2));

                    double F_k = (6 + 3 * p_n) / (6 - 7 * p_n);
                    return -1 * sigma * F_k;
                case 3: //third approximation from just fitting the data in the table for maximum simplicity! yay!
                    a = 4.01061E-28;
                    b = 3.99668;
                    c = 1.10298E-3;
                    d = 2.71393E-2;

                    //B_s = N_s * sig


                    double scalefix = 0.0144305785008 * Math.Pow(wlum - 0.104778856273, -1.47551997338) + 0.983135681962;
                    //APPROXIMATION OF SIGMA (same as long wave) (SCATTERING CROSS SECTION OF ONE "STD MOLECULE") with additional scaling fix!
                    sigma = (a * Math.Pow(wlum, -1 * (b + (c * wlum) * (d / wlum)))) * scalefix;
                    return sigma;
                default:
                    
                    break;
            }

            return sigma; //so visual studio doesnt complain :/
        }

        /// <summary>
        /// <tooltip>Returns the depolarization constant relating to the anisotropy of standard atmosphere.</tooltip>
        /// </summary>
        /// <param name="wavelength"></param>
        /// <param name="measure"></param>
        /// <returns></returns>
        public static double std_depolanisotropy(double wavelength, char measure = 'u')
        {

            //UNITS CORRECTION SECTION (Equation takes MICROMETERS)
            double wlum = wavelength;

            switch (measure)
            {
                case 'u': //input is in micrometers
                    wlum = wavelength;
                    break;
                case 'n': //input is in nanometers
                    wlum = wavelength / 1000.0;
                    break;
                case 'c': //input is in centimeters
                    wlum = wavelength * 10000.0;
                    break;
                default: //input type not recognised, presume micrometers
                    wlum = wavelength;
                    break;
            }

            //Equation adapted from data : D. R. Bates, ‘‘Rayleigh scattering by air,’’ Planet. Space Sci. 32,
            //785–790 119842.

            return (0.031874237231 * Math.Pow(wlum - 0.118186431396, -1.62366996883) + 2.69230400325) / 100.0;
        }

        /// <summary>
        /// <tooltip>Returns the IOR of standard air dependent of wavelength</tooltip>
        /// </summary>
        /// <param name="wavelength"></param>
        /// <param name="measure"></param>
        /// <returns></returns>
        public static double std_refractiveatm(double wavelength, char measure = 'u')
        {
            //UNITS CORRECTION SECTION (Equation takes MICROMETERS)
            double wlum = wavelength;

            switch (measure)
            {
                case 'u': //input is in micrometers
                    wlum = wavelength;
                    break;
                case 'n': //input is in nanometers
                    wlum = wavelength / 1000.0;
                    break;
                case 'c': //input is in centimeters
                    wlum = wavelength * 10000.0;
                    break;
                default: //input type not recognised, presume micrometers
                    wlum = wavelength;
                    break;
            }

            //Equation sourced from:
            //E. R. Peck and K. Reeder, ‘‘Dispersion of air,’’ J. Opt. Soc. Am.
            //62, 958–962 119722.

            double n_s = 1.0;

            double oneovlsq = 1.0 / Math.Pow(wlum, 2);

            if(wlum < 0.23)
            {
                n_s += (1 / 100000000.0) * ((5791817.0 / (238.01852 - oneovlsq)) + (167909.0 / (57.362 - oneovlsq)));
            }
            else
            {
                n_s += (1 / 100000000.0) * (8060.51 + (2480990.0 / (132.2742 - oneovlsq)) + (17455.7 / (39.32957 - oneovlsq)));
            }

            return n_s;
        }

        //First an altitude correction of Sigma or beta
        //The value from std_crosspermolecule
        //valid for:
        //N_s = 2.55E+24
        //T=288.15 K
        //P_s=1013.25 mbars

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wavelength"></param>
        /// <param name="Pressure"></param>
        /// <param name="Temperature"></param>
        /// <param name="measure"></param>
        /// <returns></returns>
        public static double std_crossBulk(double wavelength, double Pressure, double Temperature, char measure = 'u')
        {
            double baseB = std_crossPerMolecule(wavelength, measure) * (288.15 / 1013.25) * 2.55E+24;

            return baseB * Pressure / Temperature;
        }

        public static double std_roughAltTemp(double h)
        {
            //h is in km
            double a = 38.4401;
            double b = 127.113;
            double c = 0.138023;
            double d = 270.067;

            return a * Math.Pow(Math.E, -1 * Math.Pow(h / b, 2.0)) * Math.Cos(c * h) + d;
        }

        public static double std_airPressureApprox(double h)
        {
            //h is in kilometers
            h /= 1000;
            double P_b = 1013.25;
            double M = 0.0289644;
            double g0 = 9.80665;
            double R = Physics.R;
            double T_b = std_roughAltTemp(h);

            return P_b * Math.Pow(Math.E, -1 * g0 * M * h * 1000 / (R * T_b));
        }

        //Rayleigh Optical Depth
        //tau(wavelength, z0) = integral z0 -> inf (Beta(wavelength, z) dz)

        public static double std_OpticalPathLength(double distance, double theta, double wavelength, double precision = 1, double upper = 50000, double RADIUS = 6371000)
        {
            double d = distance;
            Environment.Render.Primatives.Sphere sphere = new Environment.Render.Primatives.Sphere(new Vector3(0.0, 0.0, 0.0), RADIUS);

            //finds the point at which the ray would hit the sphere, if applicable.
            Vector3 hitPoint = sphere.NearHit(new Vector3(-1 * d, 0.0, 0.0), new Vector3(Math.Cos(theta), Math.Sin(theta), 0.0));
            bool didHit = false;
            Vector3 normalAt = Vector3.Zero;
            if (hitPoint.Form == Vector3.VectorForm.POSITION) { didHit = true; normalAt = sphere.Grad(hitPoint); normalAt.Form = Vector3.VectorForm.NORMAL; }
            else { normalAt.Form = Vector3.VectorForm.INFINITY; }
            Vector3 reflAt = Vector3.Bounce(new Vector3(Math.Cos(theta), Math.Sin(theta), 0.0), normalAt);

            //this is a f(x) & x use of Vector3 just to store data.
            Vector3 current = Vector3.Zero;
            Vector3 next = Vector3.Zero;

            //our integration precision \/
            double drange = precision;
            double Accumulate = 0;
            double range = 0.0;
            double h_next = Math.Sqrt(range * range + d * d + 2 * range * d * Math.Cos(Math.PI - theta)) - RADIUS;

            //run function for current
            current.X = range;
            current.Y = Atmospherics.std_crossBulk(wavelength,
                Atmospherics.std_airPressureApprox(h_next),
                Atmospherics.std_roughAltTemp(h_next));

            bool doBounce = false;


            double travelled = 0.0;

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
                if (h_next < 0 && didHit)
                {
                    doBounce = true;
                    range -= 2 * drange;
                    h_next = Math.Sqrt(next.X * next.X + d * d + 2 * next.X * d * Math.Cos(Math.PI - theta)) - RADIUS;
                }
                //next.Y = Atmospherics.std_airPressureApprox(h_next);

                next.Y = Atmospherics.std_crossBulk(wavelength,
                         Atmospherics.std_airPressureApprox(h_next),
                         Atmospherics.std_roughAltTemp(h_next));

                double Area = drange * (current.Y + next.Y) / 2.0;
                Accumulate += Area / 1000; // the 1k is from pressure measurements I think?
                current.X = next.X;
                current.Y = next.Y;
            }

            return Accumulate;
        }



    }
}
