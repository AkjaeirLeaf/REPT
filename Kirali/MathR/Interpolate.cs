using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirali.MathR
{
    public class Interpolate
    {
        public static double Lerp(double a, double b, double x)
        {
            return a + x * (b - a);
        }

        public static double Smooth(double a, double b, double x)
        {
            return a + (6 * Math.Pow(x, 5) - 15 * Math.Pow(x, 4) + 10 * Math.Pow(x, 3)) * (b - a);
        }

        public static double GaussianFalloff(double center, double scale, double r, double h = 0.0)
        {
            return center * Math.Pow(Math.E, -1 * Math.Pow(r / scale, 2)) + h;
        }

        public static double Mix(double a, double b, double factor)
        {
            return a * (1.0 - factor) + b * factor;
        }
    }
}
