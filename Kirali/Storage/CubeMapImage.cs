using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

using Kirali.MathR;

namespace Kirali.Storage
{
    public class CubeMapImage
    {
        private int width, height;
        private CubeMapImageBounds bounds;

        private Bitmap imagebase;

        public static CubeMapImage FromFile(string filepath, CubeMapImageBounds imageBounds)
        {
            CubeMapImage cmi = new CubeMapImage();
            cmi.imagebase  =  new Bitmap(filepath);
            cmi.bounds     =  imageBounds;
            cmi.width      =  cmi.imagebase.Width;
            cmi.height     =  cmi.imagebase.Height;

            return cmi;
        }

    }

    public struct CubeMapImageBounds
    {
        public Vector2 stripLeftNW, stripLeftNE, stripLeftSW, stripLeftSE;
        public Vector2 stripRightNW, stripRightNE, stripRightSW, stripRightSE;
    }
}
