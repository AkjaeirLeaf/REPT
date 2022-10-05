using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Kirali.Storage
{
    public enum OpticalStorageType
    {
        R_OPPATHDEPTH = 0xF6F0, //optical path depth storage
        R_TRANSMITTAN = 0xF6F1  //optical transmittance
    }

    public enum OpticalStorageMethod
    {
        ANGLE_PAH = 0x5E20, //path angle data method; use this only for in-atmosphere tables.
        ANGLE_HIT = 0x5E21, //hit angle data method; most efficient method only usable outside of atmosphere, complete dataset must also use D_CLOSEST for outside regions.
        D_CLOSEST = 0x5E22 //closest distance ray - center method, most effective outside atmosphere.
    }

    public enum OpticalAngleRangeMethod
    {
        ANGLES_FULL2PI = 0xBBE1,
        ANGLES_HALF1PI = 0xBBE2,
        ANGLES_QUAR05P = 0xBBE3,
        ANGLES_CTRSURF = 0xBBE9
    }


    public class OpticalStorage
    {
        //storage can be simplified through use of in space/not in space/ bounce removal
        //for in-space: 
        //store 2 sections: ANGLE_HIT, D_CLOSEST
        //
        private readonly byte[] FORMAT_IDENT_STR = Encoding.Default.GetBytes("OPS+kir22");
        private readonly byte[] FORMAT_WAVDECLAR = ByteFromInt(0xAEA0B3);
        private readonly byte[] FORMAT_DISTCOUNT = ByteFromInt(0xAF77B9);
        private readonly byte[] FORMAT_DISTUPPER = ByteFromInt(0xA7F7B9);
        private readonly byte[] FORMAT_ANGLEDIVS = ByteFromInt(0xFBDBBA);
        private readonly byte[] FORMAT_ANGLRANGM = ByteFromInt(0xFBEEBB);
        private readonly byte[] FORMAT_SECTIONHD = ByteFromInt(0xFADAEA);
        private readonly byte[] FORMAT_CONTBEGIN = ByteFromInt(0x10FDFE);
        private readonly byte[] FORMAT_CONTENEND = ByteFromInt(0x12DEDF);
        private readonly byte[] FORMAT_FULFILEND = ByteFromInt(0xEEBEF6);

        private const int DOUBLE_s = sizeof(double);
        private const int INT32_s  = sizeof(int);

        //please use universal wavelength storage per file 
        private Int32[] wavelengths;
        private int wavelengthsCount = 0;
        private int distanceblockcount = 100;
        private int distanceupper = 500;
        private int angleblockcount = 100;
        private OpticalAngleRangeMethod OARM;

        private bool use__osmanglepah_r_oppathdepth = false;
        private bool use__osmanglepah_r_transmittan = false;
        private double[] block_osmanglepah_r_oppathdepth;
        private double[] block_osmanglepah_r_transmittan;

        private bool use__osmanglehit_r_oppathdepth = false;
        private bool use__osmanglehit_r_transmittan = false;
        private double[] block_osmanglehit_r_oppathdepth;
        private double[] block_osmanglehit_r_transmittan;

        private bool use__osmdclosest_r_oppathdepth = false;
        private bool use__osmdclosest_r_transmittan = false;
        private double[] block_osmdclosest_r_oppathdepth;
        private double[] block_osmdclosest_r_transmittan;


        public OpticalStorage(Int32[] declareWavelengths, OpticalAngleRangeMethod oarm, int angle_bc = 100, int dist_bc = 100, int dist_up = 500)
        {
            wavelengths = declareWavelengths;
            wavelengthsCount = wavelengths.Length;
            OARM = oarm;
            angleblockcount = angle_bc;
            distanceblockcount = dist_bc;
            distanceupper = dist_up;
        }

        public void Add_R_OPPATHDEPTH(int wlindex, double[] oppathdepths)
        {
            if (!use__osmanglehit_r_oppathdepth)
            {
                block_osmanglehit_r_oppathdepth = new double[wavelengthsCount * angleblockcount];
                use__osmanglehit_r_oppathdepth = true;
            }
            for(int ctr = 0; ctr < angleblockcount; ctr++)
            {
                block_osmanglehit_r_oppathdepth[(wlindex * angleblockcount) + ctr] = oppathdepths[ctr];
            }
        }

        public void Add_R_DCLOSEST(int wlindex, double[] oppathdepths)
        {
            if (!use__osmdclosest_r_oppathdepth)
            {
                block_osmdclosest_r_oppathdepth = new double[wavelengthsCount * angleblockcount];
                use__osmdclosest_r_oppathdepth = true;
            }
            for (int ctr = 0; ctr < angleblockcount; ctr++)
            {
                block_osmdclosest_r_oppathdepth[(wlindex * angleblockcount) + ctr] = oppathdepths[ctr];
            }
        }

        public int SerializeWriteObject(string filepath)
        {
            //determine total length:
            //first comes identifier of file type, then the wavelength declaration tag.
            int totalLength = FORMAT_IDENT_STR.Length + FORMAT_WAVDECLAR.Length;
            //next is the actual array of wavelengths
            totalLength += sizeof(int) + (sizeof(int) * wavelengths.Length);
            //next is the distance count tag, distance count decl,
            totalLength += FORMAT_DISTCOUNT.Length + sizeof(int);
            //next is distance upper limit tag and decl, presume lower is h=0 km, 
            totalLength += FORMAT_DISTUPPER.Length + sizeof(int);
            //next is the angle divisions tag and the declaration of angle detail
            totalLength += FORMAT_ANGLEDIVS.Length + sizeof(int);
            //next is angle division method tag and declare
            totalLength += FORMAT_ANGLRANGM.Length + ByteFromInt((Int32)OARM).Length;
            //start section definitions and lengths.
            totalLength += FORMAT_SECTIONHD.Length;

            //next is count of total sections, but we don't know what that is yet.
            //sections count guide:
            // 1001 do osm angle pah
            // 1001 1000 optical depth
            // 1001 1001 transmission
            // 
            // 1010 do osm angle 
            // 1010 1000 optical depth
            // 1010 1010 transmission
            // 
            // 1100 do dist closest
            // 1100 1000 optical depth
            // 1100 1100 transmission
            //say there is one section, and it is

            int totalsections = 0;
            byte sectionsID = 128 + 8;
            if (use__osmanglepah_r_oppathdepth || use__osmanglepah_r_transmittan)
            {
                sectionsID += 16; totalsections += 1;
                totalLength += FORMAT_CONTBEGIN.Length + FORMAT_CONTENEND.Length;
                if (use__osmanglepah_r_oppathdepth) { sectionsID += 0; totalLength += block_osmanglepah_r_oppathdepth.Length * sizeof(double); }
                if (use__osmanglepah_r_transmittan) { sectionsID += 1; totalLength += block_osmanglepah_r_transmittan.Length * sizeof(double); }
            }
            if (use__osmanglehit_r_oppathdepth || use__osmanglehit_r_transmittan)
            {
                sectionsID += 32; totalsections += 1;
                totalLength += FORMAT_CONTBEGIN.Length + FORMAT_CONTENEND.Length;
                if (use__osmanglehit_r_oppathdepth) { sectionsID += 0; totalLength += block_osmanglehit_r_oppathdepth.Length * sizeof(double); }
                if (use__osmanglehit_r_transmittan) { sectionsID += 2; totalLength += block_osmanglehit_r_transmittan.Length * sizeof(double); }
            }
            if (use__osmanglepah_r_oppathdepth || use__osmanglepah_r_transmittan)
            {
                sectionsID += 64; totalsections += 1;
                totalLength += FORMAT_CONTBEGIN.Length + FORMAT_CONTENEND.Length;
                if (use__osmdclosest_r_oppathdepth) { sectionsID += 0; totalLength += block_osmdclosest_r_oppathdepth.Length * sizeof(double); }
                if (use__osmdclosest_r_transmittan) { sectionsID += 4; totalLength += block_osmdclosest_r_transmittan.Length * sizeof(double); }
            }

            byte[] sectionsByteID = ByteFromInt(sectionsID);
            totalLength += sectionsByteID.Length + FORMAT_FULFILEND.Length;


            using (var stream = File.Open(filepath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8))
                {
                    writer.Write(FORMAT_IDENT_STR);
                    writer.Write(FORMAT_WAVDECLAR);
                    writer.Write(ByteFromInt(wavelengthsCount));
                    for(int wlc = 0; wlc < wavelengthsCount; wlc++)
                    { writer.Write(ByteFromInt(wavelengths[wlc])); }
                    writer.Write(FORMAT_DISTCOUNT);
                    writer.Write(ByteFromInt(distanceblockcount));
                    writer.Write(FORMAT_DISTUPPER);
                    writer.Write(ByteFromInt(distanceupper));
                    writer.Write(FORMAT_ANGLEDIVS);
                    writer.Write(ByteFromInt(angleblockcount));
                    writer.Write(FORMAT_ANGLRANGM);
                    writer.Write(ByteFromInt(((int)OARM)));
                    writer.Write(FORMAT_SECTIONHD);
                    writer.Write(ByteFromInt(sectionsID));


                    //write angle path data first if applicable
                    if (use__osmanglepah_r_oppathdepth)
                    {
                        int sectl = block_osmanglepah_r_oppathdepth.Length;
                        writer.Write(ByteFromInt(sectl));
                        writer.Write(FORMAT_CONTBEGIN);
                        for(int ix = 0; ix < sectl; ix++)
                        {
                            writer.Write(ByteFromDouble(block_osmanglepah_r_oppathdepth[ix]));
                        }
                        writer.Write(FORMAT_CONTENEND);
                    }
                    else if (use__osmanglepah_r_transmittan)
                    {
                        int sectl = block_osmanglepah_r_transmittan.Length;
                        writer.Write(ByteFromInt(sectl));
                        writer.Write(FORMAT_CONTBEGIN);
                        for (int ix = 0; ix < sectl; ix++)
                        {
                            writer.Write(ByteFromDouble(block_osmanglepah_r_transmittan[ix]));
                        }
                        writer.Write(FORMAT_CONTENEND);
                    }


                    //write angle hit data next if applicable
                    if (use__osmanglehit_r_oppathdepth)
                    {
                        int sectl = block_osmanglehit_r_oppathdepth.Length;
                        writer.Write(ByteFromInt(sectl));
                        writer.Write(FORMAT_CONTBEGIN);
                        for (int ix = 0; ix < sectl; ix++)
                        {
                            writer.Write(ByteFromDouble(block_osmanglehit_r_oppathdepth[ix]));
                        }
                        writer.Write(FORMAT_CONTENEND);
                    }
                    else if (use__osmanglehit_r_transmittan)
                    {
                        int sectl = block_osmanglehit_r_transmittan.Length;
                        writer.Write(ByteFromInt(sectl));
                        writer.Write(FORMAT_CONTBEGIN);
                        for (int ix = 0; ix < sectl; ix++)
                        {
                            writer.Write(ByteFromDouble(block_osmanglehit_r_transmittan[ix]));
                        }
                        writer.Write(FORMAT_CONTENEND);
                    }

                    //write d_closest data last if applicable
                    if (use__osmdclosest_r_oppathdepth)
                    {
                        int sectl = block_osmdclosest_r_oppathdepth.Length;
                        writer.Write(ByteFromInt(sectl));
                        writer.Write(FORMAT_CONTBEGIN);
                        for (int ix = 0; ix < sectl; ix++)
                        {
                            writer.Write(ByteFromDouble(block_osmdclosest_r_oppathdepth[ix]));
                        }
                        writer.Write(FORMAT_CONTENEND);
                    }
                    else if (use__osmdclosest_r_transmittan)
                    {
                        int sectl = block_osmdclosest_r_transmittan.Length;
                        writer.Write(ByteFromInt(sectl));
                        writer.Write(FORMAT_CONTBEGIN);
                        for (int ix = 0; ix < sectl; ix++)
                        {
                            writer.Write(ByteFromDouble(block_osmdclosest_r_transmittan[ix]));
                        }
                        writer.Write(FORMAT_CONTENEND);
                    }

                    //write file close
                    writer.Write(FORMAT_FULFILEND);
                }
            }

            return -1;
        }

        private static byte[] ByteFromInt(Int32 I32)
        {
            return BitConverter.GetBytes(I32);
        }

        private static byte[] ByteFromDouble(double doublePres)
        {
            return BitConverter.GetBytes(doublePres);
        }
    }
}
