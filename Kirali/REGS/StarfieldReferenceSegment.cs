using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Kirali.Celestials;
using Kirali.MathR;

using Newtonsoft.Json;

namespace Kirali.REGS
{
    public class StarfieldReferenceSegment
    {
        public int address = 0;
        public double[] position = new double[3];
        public double[] bounds = new double[6];
        //order is - + xyz
        public int[] fileOrderPointers = new int[0];

        public StarfieldReferenceSegment()
        {

        }

        public StarfieldReferenceSegment(RGalaxy Galaxy, int[] prefix, out int errcode)
        {
            //Largest sector division side length:
            int size_base = 32;

            //Setup search params
            SystemSearchParameter SSP = new SystemSearchParameter();
            SSP.ChunkSize = size_base;
            SSP.X_chunk = prefix[0] * size_base;
            SSP.Y_chunk = prefix[1] * size_base;
            SSP.Z_chunk = prefix[2] * size_base;

            //Search!
            Galaxy.LoadSystemPoints(SSP);

            //Collect info
            int result_count = Galaxy.loaded_systemPoints.Length;


            //Create SRS
            address = 0;
            fileOrderPointers = new int[result_count];
            position = new double[] { prefix[0] * size_base, prefix[1] * size_base, prefix[2] * size_base };
            bounds = new double[] { position[0] - size_base / 2, position[0] + size_base / 2,
                                    position[1] - size_base / 2, position[1] + size_base / 2,
                                    position[2] - size_base / 2, position[2] + size_base / 2 };

            //Transfer ptrs from search results!
            for (int ixt = 0; ixt < result_count; ixt++)
            {
                //Transfer all pointers to new
                fileOrderPointers[ixt] = Galaxy.loaded_fileOrderIDs[ixt];
            }

            //Done!

            errcode = 1;
        }

        public StarfieldReferenceSegment Clone()
        {
            return (StarfieldReferenceSegment)MemberwiseClone();
        }

        public static StarfieldReferenceSegment[] Octalize(RGalaxy Galaxy, StarfieldReferenceSegment srs)
        {
            StarfieldReferenceSegment[] result = new StarfieldReferenceSegment[8];
            
            double size = (srs.bounds[1] - srs.bounds[0]) / 2; //EXAMETERS
            Vector3[] pos = new Vector3[8];
            double f0 = 0.25;
            double f1 = 0.75;
            SystemSearchParameter SSP = new SystemSearchParameter();
            int match = 0;
            int oxt = 1;

            // sec 1
            pos[0] = new Vector3
                (
                    srs.bounds[1] * f0 + f1 * srs.bounds[0],
                    srs.bounds[3] * f0 + f1 * srs.bounds[2],
                    srs.bounds[5] * f0 + f1 * srs.bounds[4]
                );
            // sec 2
            pos[1] = new Vector3
                (
                    srs.bounds[1] * f1 + f0 * srs.bounds[0],
                    srs.bounds[3] * f0 + f1 * srs.bounds[2],
                    srs.bounds[5] * f0 + f1 * srs.bounds[4]
                );
            // sec 3
            pos[2] = new Vector3
                (
                    srs.bounds[1] * f1 + f0 * srs.bounds[0],
                    srs.bounds[3] * f1 + f0 * srs.bounds[2],
                    srs.bounds[5] * f0 + f1 * srs.bounds[4]
                );
            // sec 4
            pos[3] = new Vector3
                (
                    srs.bounds[1] * f0 + f1 * srs.bounds[0],
                    srs.bounds[3] * f1 + f0 * srs.bounds[2],
                    srs.bounds[5] * f0 + f0 * srs.bounds[4]
                );
            // sec 5    EDIT FROM HERE
            pos[4] = new Vector3
                (
                    srs.bounds[1] * f0 + f1 * srs.bounds[0],
                    srs.bounds[3] * f0 + f1 * srs.bounds[2],
                    srs.bounds[5] * f1 + f0 * srs.bounds[4]
                );
            // sec 6
            pos[5] = new Vector3
                (
                    srs.bounds[1] * f1 + f0 * srs.bounds[0],
                    srs.bounds[3] * f0 + f1 * srs.bounds[2],
                    srs.bounds[5] * f1 + f0 * srs.bounds[4]
                );
            // sec 7
            pos[6] = new Vector3
                (
                    srs.bounds[1] * f1 + f0 * srs.bounds[0],
                    srs.bounds[3] * f1 + f0 * srs.bounds[2],
                    srs.bounds[5] * f1 + f0 * srs.bounds[4]
                );
            // sec 8
            pos[7] = new Vector3
                (
                    srs.bounds[1] * f0 + f1 * srs.bounds[0],
                    srs.bounds[3] * f1 + f0 * srs.bounds[2],
                    srs.bounds[5] * f1 + f0 * srs.bounds[4]
                );
            for (oxt = 1; oxt <= 8; oxt++)
            {
                //Define search param cube size
                SSP.ChunkSize = size;

                //Define search position center
                SSP.X_chunk = pos[oxt - 1].X;
                SSP.Y_chunk = pos[oxt - 1].Y;
                SSP.Z_chunk = pos[oxt - 1].Z;

                //Load seach parameter thru galaxy
                Galaxy.LoadSystemPoints(SSP);

                //Define address
                result[oxt - 1].address = srs.address * 10 + oxt;

                //Create new array for matches
                match = Galaxy.loaded_systemPoints.Length;
                result[oxt - 1] = new StarfieldReferenceSegment();  //Create cache
                result[oxt - 1].fileOrderPointers = new int[match]; //create pointerarray
                //Transfer position
                result[oxt - 1].position = new double[] { pos[oxt - 1].X, pos[oxt - 1].Y, pos[oxt - 1].Z };
                //Transfer bounds
                result[oxt - 1].bounds = new double[] { pos[oxt - 1].X - size / 2, pos[oxt - 1].X + size / 2,
                                                    pos[oxt - 1].Y - size / 2, pos[oxt - 1].Y + size / 2,
                                                    pos[oxt - 1].Z - size / 2, pos[oxt - 1].Z + size / 2 };

                for (int ixt = 0; ixt < match; ixt++)
                {
                    //Transfer all pointers to new
                    result[oxt - 1].fileOrderPointers[ixt] = Galaxy.loaded_fileOrderIDs[ixt];
                }
            }

            return result;
        }

        public void Save(RGalaxy Galaxy, double[] prefix)
        {
            string path;
            if (address != 0)
                path = Galaxy.StoragePath + "\\sector\\" + (int)prefix[0] + " " + (int)prefix[1] + " " + (int)prefix[2] + "\\" + address + ".gsref";
            else
                path = Galaxy.StoragePath + "\\sector\\" + (int)prefix[0] + " " + (int)prefix[1] + " " + (int)prefix[2] + "\\base.gsref";
            string content = JsonConvert.SerializeObject(this);
            File.WriteAllText(path, content);
        }
    }
}
