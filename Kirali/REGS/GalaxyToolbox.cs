using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.Celestials;
using Kirali.MathR;

namespace Kirali.REGS
{
    /// <summary>
    /// <tooltip>Toolbox of handy data controls and features ported from REGS.</tooltip>
    /// </summary>
    public class GalaxyToolbox
    {
        public static readonly string currentVersion = "v22x09m09_v1";


        public static void CacheOctalReference(RGalaxy Galaxy, out string info)
        {
            //Find Galaxy Scale Limits:
            double xMin = 0;
            double xMax = 0;
            double yMin = 0;
            double yMax = 0;
            double zMin = 0;
            double zMax = 0;

            for (int stc = 0; stc < Galaxy.system_points.Length; stc++)
            {
                if (Galaxy.system_points[stc].X < xMin) { xMin = Galaxy.system_points[stc].X; }
                if (Galaxy.system_points[stc].Y < yMin) { yMin = Galaxy.system_points[stc].Y; }
                if (Galaxy.system_points[stc].Z < zMin) { zMin = Galaxy.system_points[stc].Z; }

                if (Galaxy.system_points[stc].X > xMax) { xMax = Galaxy.system_points[stc].X; }
                if (Galaxy.system_points[stc].Y > yMax) { yMax = Galaxy.system_points[stc].Y; }
                if (Galaxy.system_points[stc].Z > zMax) { zMax = Galaxy.system_points[stc].Z; }
            }

            //Lims at 32 sector wide cubes
            int starting_sec = 32;
            int x_N_Lim = (int)(Math.Floor(xMin / starting_sec));
            int x_P_Lim = (int)(Math.Ceiling(xMax / starting_sec));
            int y_N_Lim = (int)(Math.Floor(yMin / starting_sec));
            int y_P_Lim = (int)(Math.Ceiling(yMax / starting_sec));
            int z_N_Lim = (int)(Math.Floor(yMin / starting_sec));
            int z_P_Lim = (int)(Math.Ceiling(yMax / starting_sec));

            int level_0_count = x_N_Lim + x_P_Lim + y_N_Lim + y_P_Lim + z_N_Lim + z_P_Lim;
            StarfieldReferenceSegment[] l_0 = new StarfieldReferenceSegment[level_0_count];

            int activecounter = 0;
            int subdivs = 5; // hmmm

            //Cycle all starting boxes.
            int address = 0;
            int[] prefix = new int[3];
            for (int z_s = z_N_Lim; z_s <= z_P_Lim; z_s++)
            {
                for (int y_s = y_N_Lim; y_s <= y_P_Lim; y_s++)
                {
                    for (int x_s = x_N_Lim; x_s <= x_P_Lim; x_s++)
                    {
                        prefix = new int[] { x_s, y_s, z_s };
                        StarfieldReferenceSegment SRS = new StarfieldReferenceSegment(Galaxy, prefix, out _);
                        l_0[activecounter] = SRS;
                        SRS.Save(Galaxy, SRS.position);
                        activecounter++;
                    }
                }
            }

            //Subdivide all further boxes
            activecounter = 0;
            for (int cyc = 0; cyc < level_0_count; cyc++)
            {
                StarfieldReferenceSegment current_contain = l_0[cyc].Clone();

                //rough rip
                StarfieldReferenceSegment[] l_1 = StarfieldReferenceSegment.Octalize(Galaxy, current_contain);
                for(int cyc_1 = 0; cyc_1 < l_1.Length; cyc_1++)
                {
                    StarfieldReferenceSegment[] l_2 = StarfieldReferenceSegment.Octalize(Galaxy, l_1[cyc_1]);



                    l_1[cyc_1].Save(Galaxy, l_1[cyc_1].position);
                }
            }

            info = "";
        }

    }


}
