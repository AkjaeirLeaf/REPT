using System;

using Kirali.MathR;
using Kirali.Light;
using Kirali.Celestials;
using Kirali.Framework;


namespace KiraliConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("KIRALI CONSOLE TESTING");
            Console.WriteLine("Hello Universe!\n\n");
            
            for(int ct = 0; ct < 10; ct++)
            {
                Console.WriteLine(Kirali.Framework.Random.Double(-1, 1));
            }


            /*

            Vector3 posInit = new Vector3(20, 0, 0, Vector3.VectorForm.POSITION);
            Vector3 dirInit = new Vector3(-4, 3, 0, Vector3.VectorForm.DIRECTION).Normalize();



            LightRay lr = new LightRay(posInit, dirInit);
            Console.WriteLine("Light Ray init position: " + lr.Position.ToString(3));
            Console.WriteLine("Light Ray init direction: " + lr.Direction.ToString(3));
            LinearFalloff lf_explicit = new LinearFalloff();
            lr.SetMedium(lf_explicit, 0.01);
            
            while (lr.Position.X > -10)
            {
                lr.March(0.002, 35);
                Console.WriteLine("Light Ray position stepped: " + lr.Position.ToString(3));
            }
            
            Console.WriteLine("Light Ray has reached the bounds!");
            */
        }
    }
}
