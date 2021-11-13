using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;


namespace Kirali.Light
{
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
            r = red;
            g = green;
            b = blue;
        }

        public KColor4(double red, double green, double blue, double alpha)
        {
            r = red;
            g = green;
            b = blue;
            a = alpha;
        }

        public Color ToSystemColor()
        {
            int newR = 0;
            int newG = 0;
            int newB = 0;
            int newA = 255;


            if (r < 1.0 && g < 1.0 && g < 1.0)
            {
                newR = (int)(255 * r);
                newG = (int)(255 * g);
                newB = (int)(255 * b);
            }
            else
            {
                newR = (int)(255);
                newG = (int)(255);
                newB = (int)(255);
            }

            if (r < 0) { newR = 0; }
            if (g < 0) { newG = 0; }
            if (b < 0) { newB = 0; }

            return Color.FromArgb(newA, newR, newG, newB);
        }

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
    }
}
