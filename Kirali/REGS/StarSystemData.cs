using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.Celestials;

namespace Kirali.REGS
{
    public class StarSystemData
    {
        //Release of stardata

        //dont fuck around with the file order or naming as it will break system lock.
        public int fileOrderId = 0;

        public string sectorID = "";
        public string superfaction = "";


        public string systemName = "";

        public double majorEcliptic = 0.0;
        public double phaseEcliptic = 0.0;

        public bool objectsGenerated = false;
        public bool discovered = false;

        public int Generation = 0;

        public CelestialObject[] objects;
    }
}
