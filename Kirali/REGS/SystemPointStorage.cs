using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.MathR;

namespace Kirali.REGS
{
    /// <summary>
    /// <tooltip>Storage file used in old REGS databases.</tooltip>
    /// </summary>
    public class SystemPointStorage
    {
        public int spectralClass = 00;
        public double temp = 0;
        public int masterBranch = 0;

        public string starName = "";

        public double X = 0;
        public double Y = 0;
        public double Z = 0;

        public SystemPointStorage()
        {

        }

        public SystemPointStorage(double x, double y, double z, int MasterBranch)
        {
            X = x;
            Y = y;
            Z = z;
            masterBranch = MasterBranch;
        }
    }

    /// <summary>
    /// <tooltip>Storage file used in old REGS databases.</tooltip>
    /// </summary>
    public class StarDataFocused
    {
        public int spectralClass = 00;
        public double temp = 0;
        public int masterBranch = 0;

        public string starName = "";

        //different
        public int masterPlacementId = 0; //placement of data in master array.

        public double X = 0;
        public double Y = 0;
        public double Z = 0;

        public StarDataFocused()
        {

        }

        public StarDataFocused(SystemPointStorage sd, int masterplacement)
        {
            X = sd.X;
            Y = sd.Y;
            Z = sd.Z;
            spectralClass = sd.spectralClass;
            masterBranch = sd.masterBranch;
            starName = sd.starName;
            temp = sd.temp;
            masterPlacementId = masterplacement;
        }

        public StarDataFocused(double x, double y, double z, int MasterBranch)
        {
            X = x;
            Y = y;
            Z = z;
            masterBranch = MasterBranch;
        }
    }
}
