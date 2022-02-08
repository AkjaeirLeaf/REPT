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
        
        //creates a polynomial approximation using all points of a set (use FourPointSeries for faster approx)
        public static double PolynomialSeries(double x, double[] xseries, double[] yseries)
        {
            if (xseries.Length == yseries.Length)
            {
                int l = xseries.Length;
                double result = 0;

                for (int i = 0; i < l; i++)
                {
                    double numer = 1;
                    double denom = 1;
                    for (int j = 0; j < l; j++)
                    {
                        if(i != j)
                        {
                            numer *= (x - xseries[j]);
                            denom *= (xseries[i] - xseries[j]);
                        }
                    }
                    result += yseries[i] * (numer / denom);
                }

                return result;
            }
            else
            {
                throw new Exception("Attempted invalid polynomial interpolation. X and Y series must be of equal length!");
            }
        }

        //polynomial interpolation using four datapoints, two low, two high to get an approximation more accurate than LERP but faster than PolynomialSeries
        public static double FourPointSeries(double x, double[] xseries, double[] yseries)
        {
            if (xseries.Length == yseries.Length)
            {
                int xlow = GetLow(x, xseries);
                double[] xnear = new double[] { xseries[xlow - 1], xseries[xlow], xseries[xlow + 1], xseries[xlow + 2] };
                double[] ynear = new double[] { yseries[xlow - 1], yseries[xlow], yseries[xlow + 1], yseries[xlow + 2] };

                return PolynomialSeries(x, xnear, ynear);
            }
            else
            {
                throw new Exception("Attempted invalid polynomial interpolation. X and Y series must be of equal length!");
            }
        }

        //returns the position in a series of the closest value less than the given target.
        public static int GetLow(double x, double[] xseries)
        {
            for(int i = 0; i < xseries.Length; i++)
            {
                if (x < xseries[i]) { return i; }
            }
            return -1; //DNF
        }

        //returns the position in a series of the closest value greater than the given target.
        public static int GetHigh(double x, double[] xseries)
        {
            for (int i = xseries.Length - 1; i > 0; i--)
            {
                if (x > xseries[i]) { return i; }
            }
            return -1; //DNF
        }
    }
}
