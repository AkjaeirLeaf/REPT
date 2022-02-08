using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;


namespace Kirali.Light
{
    public enum KColorFormat
    {
        RGBA = 0,      //Standard default KColor4
        RGB = 1,       //Standard default KColor3 (jpglike no transparency support)
        WAVE = 2,      //Color defined by single wavelength light
        FREQ = 3,      //Color defined by single frequency light
        EXPLICIT = 4,  //Color defined by Explicit-type function
        SERIES = 5     //Color defined by Series-type approximation
    }

    public class KColor4
    {
        private double r = 0.0;
        private double g = 0.0;
        private double b = 0.0;
        private double a = 1.0;

        public double R { get { return r; } set { r = value; } }
        public double G { get { return g; } set { g = value; } }
        public double B { get { return b; } set { b = value; } }
        public double A { get { return a; } set { a = value; } }

        public KColor4() { }

        public KColor4(double red, double green, double blue)
        {
            if (red   >= 0.0) { r = red;   } else { r = 0.0; }
            if (green >= 0.0) { g = green; } else { g = 0.0; }
            if (blue  >= 0.0) { b = blue;  } else { b = 0.0; }
        }

        public KColor4(double red, double green, double blue, double alpha)
        {
            if (red   >= 0.0) { r = red;   } else { r = 0.0; }
            if (green >= 0.0) { g = green; } else { g = 0.0; }
            if (blue  >= 0.0) { b = blue;  } else { b = 0.0; }
            if (alpha >= 0.0) { a = alpha; } else if (alpha >= 1 ) { a = 1.0; } else { a = 0.0; }
        }

        public KColor4(KColor4 color)
        {
            r = color.R;
            g = color.G;
            b = color.B;
            a = color.A;
        }

        public KColor4(Color color) 
        {
            r = color.R / 255.0;
            g = color.G / 255.0;
            b = color.B / 255.0;
            a = color.A / 255.0;
        }

        public Color ToSystemColor(bool incAlpha = false)
        {
            int newR = 0;
            int newG = 0;
            int newB = 0;
            int newA = 255;

            if (r < 1.0) { newR = (int)(255 * r); }
            else { newR = 255; }
            if (g < 1.0) { newG = (int)(255 * g); }
            else { newG = 255; }
            if (b < 1.0) { newB = (int)(255 * b); }
            else { newB = 255; }
            if (a < 1.0) { newA = (int)(255 * a); }
            else { newA = 255; }


            if (r < 0) { newR = 0; }
            if (g < 0) { newG = 0; }
            if (b < 0) { newB = 0; }
            if (a < 0) { newA = 0; }

            if (incAlpha)
            {
                return Color.FromArgb(newA, newR, newG, newB);
            }
            else
            {
                return Color.FromArgb(255, newR, newG, newB);
            }
        }


        //MORE NON-STATIC

        /// <summary>
        /// Returns the inverted KColor4 (only rgb/rgba format)
        /// </summary>
        /// <param name="flipAlpha"></param>
        /// <returns></returns>
        public KColor4 Flip(bool flipAlpha = false)
        {
            if (!flipAlpha)
            {
                return new KColor4(1.0 - r, 1.0 - g, 1.0 - b);
            }
            else
            {
                return new KColor4(1.0 - r, 1.0 - g, 1.0 - b, 1.0 - a);
            }
        }

        public double IntensityRGB()
        {
            return (r + g + b) / 3.0;
        }

        public double IntensityRGBA()
        {
            return (r + g + b + a) / 4.0;
        }



        //STATIC OP


        public static KColor4 Average(KColor4[] colors)
        {
            KColor4 kc4 = new KColor4();

            for(int c = 0; c < colors.Length; c++)
            {
                kc4 += colors[c];
            }

            kc4 /= colors.Length;

            return kc4;
        }

        public static double IntensityRGB(KColor4 c1)
        {
            return (c1.r + c1.g + c1.b) / 3.0;
        }

        public static double IntensityRGBA(KColor4 c1)
        {
            return (c1.r + c1.g + c1.b + c1.a) / 4.0;
        }

        public static KColor4 Mix(KColor4 c1, KColor4 c2)
        {
            return new KColor4(c1.r + c2.r, c1.g + c2.g, c1.b + c2.b, c1.a + c2.a) / 2;
        }

        public static KColor4 Mix(KColor4 c1, KColor4 c2, double factor)
        {
            KColor4 c1sk = (factor) * c1;
            KColor4 c2sk = (1 - factor) * c2;
            KColor4 result = c1sk + c2sk; result.a = 1.0;

            return result;
        }

        public static KColor4 IrgbMix(KColor4 c1, KColor4 c2)
        {
            double i1 = c1.IntensityRGB();
            double i2 = c2.IntensityRGB();
            double sum = i1 + i2;

            KColor4 c1sk = (i1 / sum) * c1;
            KColor4 c2sk = (i2 / sum) * c2;


            return new KColor4(c1sk.r + c2sk.r, c1sk.g + c2sk.g, c1sk.b + c2sk.b, 1.0);
        }

        /// <summary>
        /// <tooltip>Mixes the colors together based on a factor of how intense the color is (stronger color remains strongest)</tooltip>
        /// </summary>
        /// <param name="colors"></param>
        /// <returns></returns>
        public static KColor4 IrgbMix(KColor4[] colors)
        {
            double[] intensities = new double[colors.Length];
            double sum = 0;

            for(int xs = 0; xs < colors.Length; xs++)
            {
                intensities[xs] = colors[xs].IntensityRGB();
                sum += intensities[xs];
            }

            double newR = 0;
            double newG = 0;
            double newB = 0;

            for (int xs = 0; xs < colors.Length; xs++)
            {
                newR += (intensities[xs] / sum) * colors[xs].R;
                newG += (intensities[xs] / sum) * colors[xs].G;
                newB += (intensities[xs] / sum) * colors[xs].B;
            }

            return new KColor4(newR, newG, newB, 1.0);
        }

        public double GetSaturation()
        {
            return (Math.Abs(r - g) + Math.Abs(b - g) + Math.Abs(r - b)) / 2.0;
        }

        public KColor4 AdjustSaturation(double factor)
        {
            double newR, newG, newB;
            double avg = (r + g + b) / 3;
            double sat = GetSaturation();
            newR = r + (1 - factor) * (avg - r);
            newG = g + (1 - factor) * (avg - g);
            newB = b + (1 - factor) * (avg - b);
            return new KColor4(newR, newG, newB);
        }

        public static KColor4 Flip(KColor4 c1, bool flipAlpha = false)
        {
            if (!flipAlpha)
            {
                return new KColor4(1.0 - c1.r, 1.0 - c1.g, 1.0 - c1.b);
            }
            else
            {
                return new KColor4(1.0 - c1.r, 1.0 - c1.g, 1.0 - c1.b, 1.0 - c1.a);
            }
        }

        public static KColor4 operator +(KColor4 c1, KColor4 c2)
        {
            return new KColor4(c1.r + c2.r, c1.g + c2.g, c1.b + c2.b, c1.a + c2.a);
        }

        public static KColor4 operator /(KColor4 c1, double a)
        {
            return new KColor4(c1.r / a, c1.g / a, c1.b / a, c1.a / a);
        }

        public static KColor4 operator *(KColor4 c1, double a)
        {
            return new KColor4(c1.r * a, c1.g * a, c1.b * a, c1.a * a);
        }

        public static KColor4 operator *(double a, KColor4 c1)
        {
            return new KColor4(c1.r * a, c1.g * a, c1.b * a, c1.a * a);
        }

        public static KColor4 operator *(KColor4 c1, KColor4 c2)
        {
            return new KColor4(c1.r * c2.r, c1.g * c2.g, c1.b * c2.b, c1.a * c2.a);
        }

        public void A_fill() { a = 1.0; }


        //SYSCOLORS
        public static explicit operator Color(KColor4 kc4) => kc4.ToSystemColor();
        public static explicit operator KColor4(Color col) => new KColor4(col);

        public static KColor4 visibleBlackBodyApprox(double T)
        {
            return null;
        }

        /// <summary>
        /// <tooltip>Returns KColor4 Format conversion of wavelength as a human eye would likely see it.</tooltip>
        /// </summary>
        /// <param name="wavelength"></param>
        /// <param name="measure"></param>
        /// <returns></returns>
        public static KColor4 visibleWavelengthToRGB(double wavelength, char measure = 'u')
        {
            double Wavelength = wavelength * 1000;
            switch (measure)
            {
                case 'u': //input is in micrometers
                    Wavelength = wavelength * 1000;
                    break;
                case 'n': //input is in nanometers
                    Wavelength = wavelength;
                    break;
                case 'c': //input is in centimeters
                    Wavelength = wavelength * 10000000.0;
                    break;
                default: //input type not recognised, presume micrometers
                    Wavelength = wavelength * 1000;
                    break;
            }

            double Red = 0, Green = 0, Blue = 0;

            Red   = (130 * Math.Pow(Math.E, -1 * Math.Pow(Wavelength / 37.5 - 10.7, 2)) + 255 * Math.Pow(Math.E, -1 * Math.Pow(Wavelength / 122 - 5.3, 6))) / 255.0;
            Green = (255 * Math.Pow(Math.E, -1 * Math.Pow(Wavelength / 81.348 - 6.6, 4))) / 255.0;
            if (Wavelength > 387)
            {
                Blue = (255 * Math.Pow(Math.E, -1 * Math.Pow(Wavelength / 60.587 - 7.327, 6))) / 255.0;
            }
            else
            {
                Blue = (255 * Math.Pow(Math.E, -1 * Math.Pow(Wavelength / 40 - 10.5, 2))) / 255.0;
            }

            return new KColor4(Red, Green, Blue);
        }

        public static KColor4 ultravioletWavelengthToRGB(double wavelength, char measure = 'u')
        {
            KColor4 outColor = new KColor4(0.0, 0.0, 0.0);
            KColor4[] Uvs = new KColor4[3];

            double Wavelength = wavelength * 1000;
            switch (measure)
            {
                case 'u': //input is in micrometers
                    Wavelength = wavelength * 1000;
                    break;
                case 'n': //input is in nanometers
                    Wavelength = wavelength;
                    break;
                case 'c': //input is in centimeters
                    Wavelength = wavelength * 10000000.0;
                    break;
                default: //input type not recognised, presume micrometers
                    Wavelength = wavelength * 1000;
                    break;
            }

            if (Wavelength < 450 && Wavelength > 0)
            {
                //THREE RANGES, UVA, UVB, UVC

                //created some gaussians to fake the three sections
                double intensityA = Math.Pow(Math.E, -1 * Math.Pow((Wavelength - 342.0) / 20, 4.0));
                double intensityB = Math.Pow(Math.E, -1 * Math.Pow((Wavelength - 298.0) / 40, 4.0));
                double intensityC = Math.Pow(Math.E, -1 * Math.Pow((Wavelength - 190.0) / 100, 4.0));

                //some basic theme colors chosen in violetish, maybe I'll add some custom or more adv themes later idk
                Uvs[0] = (new KColor4(Color.FromArgb(129, 093, 156)) * intensityA);
                Uvs[1] = (new KColor4(Color.FromArgb(172, 149, 189)) * intensityB);
                Uvs[2] = (new KColor4(Color.FromArgb(201, 190, 209)) * intensityC);

                Uvs[0].A_fill();
                Uvs[1].A_fill();
                Uvs[2].A_fill();

                //use the fancy KColor Intensity Mix function!
                outColor = IrgbMix(Uvs);

                return outColor;
            }
            else
            {
                return new KColor4(0.0, 0.0, 0.0, 1.0);
            }
        }

        public static KColor4 infraredWavelengthToRGB(double wavelength, char measure = 'u')
        {
            KColor4 outColor = new KColor4(0.0, 0.0, 0.0);
            KColor4[] IRS = new KColor4[3];

            double Wavelength = wavelength * 1000;
            switch (measure)
            {
                case 'u': //input is in micrometers
                    Wavelength = wavelength * 1000;
                    break;
                case 'n': //input is in nanometers
                    Wavelength = wavelength;
                    break;
                case 'c': //input is in centimeters
                    Wavelength = wavelength * 10000000.0;
                    break;
                default: //input type not recognised, presume micrometers
                    Wavelength = wavelength * 1000;
                    break;
            }

            if (Wavelength < 1001000 && Wavelength > 780)
            {
                //THREE RANGES, UVA, UVB, UVC

                //created some gaussians to fake the three sections
                double intensityA = Math.Pow(Math.E, -1 * Math.Pow((Wavelength - 1090.0) / 260, 8.0));
                double intensityB = Math.Pow(Math.E, -1 * Math.Pow((Wavelength - 2200.0) / 820, 8.0));
                double intensityC = Math.Pow(Math.E, -1 * Math.Pow((Wavelength - 501500.0) / 498500, 1000.0));

                IRS[0] = new KColor4(2.0 / 3, 0, 2.4 / 3) * intensityA;
                IRS[1] = new KColor4(1.4 / 3, 0, 1.4 / 3) * intensityB;
                IRS[2] = new KColor4(1.0 / 3, 0, 0.5 / 3) * intensityC;

                IRS[0].A_fill();
                IRS[1].A_fill();
                IRS[2].A_fill();

                //use the fancy KColor Intensity Mix function!
                outColor = IrgbMix(IRS);

                return outColor;
            }
            else
            {
                return KColor4.BLACK;
            }
        }

        public static KColor4 fullspecWavelengthRGB(double wavelength, char measure = 'u')
        {
            KColor4[] Colors = new KColor4[3];

            Colors[0] = visibleWavelengthToRGB(wavelength, measure);
            Colors[1] = ultravioletWavelengthToRGB(wavelength, measure);
            Colors[2] = infraredWavelengthToRGB(wavelength, measure);

            return IrgbMix(Colors);
        }

        /// <summary>
        /// <tooltip>Returns KColor4 Format conversion of wavelength. Does not have bound falloff.</tooltip>
        /// </summary>
        /// <param name="wavelength"></param>
        /// <param name="measure"></param>
        /// <returns></returns>
        public static KColor4 depr_visibleWavelengthToRGB(double wavelength, char measure = 'u')
        {
            double Wavelength = wavelength * 1000;
            switch (measure)
            {
                case 'u': //input is in micrometers
                    Wavelength = wavelength * 1000;
                    break;
                case 'n': //input is in nanometers
                    Wavelength = wavelength;
                    break;
                case 'c': //input is in centimeters
                    Wavelength = wavelength * 10000000.0;
                    break;
                default: //input type not recognised, presume micrometers
                    Wavelength = wavelength * 1000;
                    break;
            }

            double Gamma = 0.80;
            double IntensityMax = 255;

            double factor;
            double Red, Green, Blue;

            if (Wavelength < 380)
            {
                Red = 1.0;
                Green = 0.0;
                Blue = 1.0;
            }
            else if ((Wavelength >= 380) && (Wavelength < 440))
            {
                Red = -(Wavelength - 440) / (440 - 380);
                Green = 0.0;
                Blue = 1.0;
            }
            else if ((Wavelength >= 440) && (Wavelength < 490))
            {
                Red = 0.0;
                Green = (Wavelength - 440) / (490 - 440);
                Blue = 1.0;
            }
            else if ((Wavelength >= 490) && (Wavelength < 510))
            {
                Red = 0.0;
                Green = 1.0;
                Blue = -(Wavelength - 510) / (510 - 490);
            }
            else if ((Wavelength >= 510) && (Wavelength < 580))
            {
                Red = (Wavelength - 510) / (580 - 510);
                Green = 1.0;
                Blue = 0.0;
            }
            else if ((Wavelength >= 580) && (Wavelength < 645))
            {
                Red = 1.0;
                Green = -(Wavelength - 645) / (645 - 580);
                Blue = 0.0;
            }
            else if ((Wavelength >= 645) && (Wavelength < 1000))
            {
                Red = 1.0 * (100 / Wavelength);
                Green = 0.0;
                Blue = 0.0;
            }
            else
            {
                Red = 0.0;
                Green = 0.0;
                Blue = 0.0;
            };

            int[] rgb = new int[3];
            factor = 1;
            // Don't want 0^x = 1 for x <> 0
            rgb[0] = Red == 0.0 ? 0 : (int)Math.Round(IntensityMax * Math.Pow(Red * factor, Gamma));
            rgb[1] = Green == 0.0 ? 0 : (int)Math.Round(IntensityMax * Math.Pow(Green * factor, Gamma));
            rgb[2] = Blue == 0.0 ? 0 : (int)Math.Round(IntensityMax * Math.Pow(Blue * factor, Gamma));



            return new KColor4(Red, Green, Blue);
        }


        //KCOLOR4 DEFAULT COLORS
        public static KColor4 WHITE { get { return new KColor4(1.0, 1.0, 1.0, 1.0); } }
        public static KColor4 BLACK { get { return new KColor4(0.0, 0.0, 0.0, 0.0); } }
    }
}
