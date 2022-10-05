using System;
using Kirali.MathR;

namespace KiraliConsoleLab
{
    class Program
    {
        static void Main(string[] args)
        {

            //do interpolate test later.... ;-;
            //double[] xseries = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 15, 20, 24, 30 };
            //double[] yseries = { 3.6337, 3.2389, 3.0069, 2.8524, 2.7413, 2.6572, 2.5911, 2.5377, 2.4935, 2.4247, 2.3522, 2.2756, 2.2354, 2.1938};
            //
            //Console.WriteLine(Interpolate.PolynomialSeries(19, xseries, yseries));


            //Current Test: Planks law and RGB adapter
            double T = 70000;
            //double doPlankFor = Kirali.Light.KColor4.Bv_L(300, T);
            //Console.WriteLine(doPlankFor);
            //Console.WriteLine(Kirali.Light.KColor4.Bv_L(300, T));
            Console.WriteLine(Kirali.Light.KColor4.visibleBlackBodyApprox(T).ToString());
        }
    }
}
