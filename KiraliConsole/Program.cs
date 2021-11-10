using System;

using Kirali.MathR;
using Kirali.Light;
using Kirali.Celestials;

namespace KiraliConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("KIRALI CONSOLE TESTING");
            Console.WriteLine("Hello Universe!\n\n");



            Vector3 posInit = new Vector3(20, 0, 0, Vector3.VectorForm.POSITION);
            Vector3 dirInit = new Vector3(-4, 3, 0, Vector3.VectorForm.DIRECTION).Normalize();

            LightRay lr = new LightRay(posInit, dirInit);
            LinearFalloff lf_explicit = new LinearFalloff();
            lr.SetMedium(lf_explicit, 0.01);

            lr.Step(1.0);

            while (lr.Position.X > -10)
            {
                lr.March(0.002, 35);
                Console.WriteLine("Light Ray position stepped: " + lr.Position.ToString(3));
            }
            Console.WriteLine("Light Ray has reached the bounds!");
        }
    }
}
