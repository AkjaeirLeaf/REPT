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
        public string version = "v22x09m09_v1";

        //dont fuck around with the file order or naming as it will break system lock.
        public int fileOrderId = 0;

        public string sectorID = "";
        public string superfaction = "";


        public string systemName = "";

        public double majorEcliptic = 0.0;
        public double phaseEcliptic = 0.0;

        public double totalLum = 0.0;
        public double brightestTemp = 0.0;

        public bool objectsGenerated = false;
        public bool discovered = false;

        public int Generation = 0;

        public CelestialObject[] objects;

        //Methods and Generative toolbox
        public int AddCelestialObject(CelestialObject celestial)
        {
            if (!DoesContainCelestialData(celestial.CelestialID, out _))
            {
                if (objects == null || objects.Length == 0) { objects = new CelestialObject[] { celestial }; }
                else
                {
                    CelestialObject[] temp = new CelestialObject[objects.Length + 1];
                    for (int cel = 0; cel < objects.Length; cel++)
                    {
                        temp[cel] = objects[cel];
                    }
                    temp[objects.Length] = celestial;
                    objects = temp;
                }
                return (int)REGS_STD_ERROR.CELESTIAL_OBJ_ADDED;
            }
            else
            {
                return (int)REGS_STD_ERROR.CELESTIAL_ALREADY_EXIST_ERROR;
            }
        }
        public bool DoesContainCelestialData(string ID, out int place)
        {
            if (objects == null || objects.Length == 0) { place = -1; return false; }
            else
            {
                for(int cel = 0; cel < objects.Length; cel++)
                {
                    if(objects[cel].CelestialID == ID) { place = cel; return true; }
                }
                place = -1;
                return false;
            }
        }
    }
}
