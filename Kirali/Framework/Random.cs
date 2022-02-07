using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirali.Framework
{
    public class Random
    {
        private static readonly System.Random PickDouble = new System.Random();
        private static readonly System.Random PickInt    = new System.Random();
        private static readonly object syncLock = new object();

        public static int Int(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return PickInt.Next(min, max);
            }
        }

        public static double Double(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return PickDouble.Next(min, max) + PickDouble.NextDouble();
            }
        }
    }
}
