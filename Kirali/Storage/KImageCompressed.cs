using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.Light;
using Kirali.Framework;

namespace Kirali.Storage
{
    public enum KImageFormat
    {
        KIMAGE = 0,
        KFRAG = 1
    }

    public enum KCompressionType
    {
        NONE = 0,
        ColorTablesFull = 1,
        ColorTablesPart = 2,
        LCD = 3
    }

    public enum KColorSize
    {
        SYSTEM = 0, //255   COLORS
        BIG = 1,    //4096  COLORS
        SUPER = 2,  //65536 COLORS
    }

    public class KImageCompressed
    {
        private const string ident = "KIRALIIMAGEFORMAT";
        private KImageFormat kFormat;
        private KCompressionType kCtype;
        private KColorSize kColorSize;
        private KColor4 kBackground;
        private bool isAlpha = true;

        private int colorSize = 0;

        public KImageFormat ImageFormat { get { return kFormat; } }
        public KCompressionType CompressionType { get { return kCtype; } }
        public KColorSize ColorSize { get { return kColorSize; } }
        public KColor4 Background { get { return kBackground; } set { kBackground = value; } }

        //for all compression types:
        private int[] pixRefs;

        //for no compression type
        private KColor4[] allPixels;

        //for full table type
        private KColor4[] fullTableRef;

        //for part table type
        private int[] partTableRef;

        private bool beenSectored = false;

        public static KImageCompressed CompressImage(KColorImage image, KImageFormat format, KCompressionType compressionType)
        {
            KImageCompressed compressed = new KImageCompressed();
            compressed.Background = image.defaultColor;


            return null;
        }

        private void doListFull(KColor4[] pixels, bool[] doDraw)
        {
            if (!beenSectored)
            {
                int unique = 1; //unique = 0 is technically reserved for the background
                //length of full img data
                int[] tempPoint = new int[pixels.Length];
                bool[] isUnique = ArrayHandler.SetAll(pixels.Length, true);
                //length of how many unique colors
                int[] howMany = new int[pixels.Length];
                KColor4[] stack = new KColor4[pixels.Length];

                //arrange pointers for repeated pixels and define which are unique or not.
                for (int thru = 0; thru < pixels.Length; thru++)
                {
                    if (doDraw[thru])
                    {
                        if (isUnique[thru])
                        {
                            tempPoint[thru] = unique;
                            int repeat = 0;
                            KColor4 currCol = pixels[thru];
                            for (int check = thru + 1; check < pixels.Length; check++)
                            {
                                if (isUnique[check])
                                {
                                    if (isFullSame(currCol, pixels[check]))
                                    {
                                        repeat++;
                                        tempPoint[check] = unique;
                                        isUnique[check] = false;
                                    }
                                }
                            }
                            howMany[unique] = repeat;
                            stack[unique] = currCol;
                            unique++;
                        }
                    }
                }


                //sort colors in order of popularity.
                KColor4[] uniqueColors = new KColor4[unique];
                int[] oldPointer = new int[unique];
                bool[] isCached = ArrayHandler.SetAll(unique, false);
                for (int u = 0; u < unique; u++)
                {
                    int max = -1;
                    int place = 0;
                    for (int ux = 0; ux < unique; ux++)
                    {
                        if (!isCached[ux])
                        {
                            if (howMany[ux] > max || max == -1)
                            {
                                max = howMany[ux];
                                place = ux;
                            }
                        }
                    }
                    uniqueColors[u] = stack[place];
                    oldPointer[place] = u;
                    isCached[place] = true;
                }


                //correct old pixel reference integers
                int[] newPointer = new int[pixels.Length];
                for (int pix = 0; pix < pixels.Length; pix++)
                {
                    newPointer[pix] = oldPointer[tempPoint[pix]] + 1;
                }


                //set compressed image data
                pixRefs = newPointer;
                fullTableRef = uniqueColors;
                beenSectored = true;
            }
        }

        private bool isFullSame(KColor4 c1, KColor4 c2)
        {
            if(Math.Round(colorSize * c1.R) != Math.Round(colorSize * c2.R)) { return false; }
            if(Math.Round(colorSize * c1.G) != Math.Round(colorSize * c2.G)) { return false; }
            if(Math.Round(colorSize * c1.B) != Math.Round(colorSize * c2.B)) { return false; }
            if(Math.Round(colorSize * c1.A) != Math.Round(colorSize * c2.A) && isAlpha) { return false; }

            return true;
        }
    }
}
