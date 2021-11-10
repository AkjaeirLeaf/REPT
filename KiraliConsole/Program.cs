using System;

using Kirali.MathR;
using Kirali.Celestials;

namespace KiraliConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("KIRALI CONSOLE TESTING");
            Console.WriteLine("Hello World!\n\n");

            Vector3 vecInit = new Vector3(4, -3, 0);
            Vector3 vecNormal = new Vector3(-1, 0, 0).Normalize();

            Vector3 refl = Vector3.Bounce(vecInit, vecNormal);
            Vector3 refr = Vector3.Refract(vecInit, vecNormal, 1.0, 4/3.0);
            Console.WriteLine("Incoming Ray: " + vecInit.ToString(3) + "  Angle:  " + Vector3.Between(vecInit, new Vector3(vecNormal).Negate()));
            Console.WriteLine("Reflected Ray: " + refl.ToString(3) + "  Angle:  " + Vector3.Between(refl, new Vector3(vecNormal).Negate()));
            Console.WriteLine("Refracted Ray: " + refr.ToString(3) + "  Angle:  " + Vector3.Between(refr, new Vector3(vecNormal).Negate()));
        }
    }
}
