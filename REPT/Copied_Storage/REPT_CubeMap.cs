using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

using Kirali.MathR;

namespace REPT.Copied_Storage
{
    public class REPT_CubeMap
    {
        private int width, height;
        private REPT.Copied_Storage.CubeMapImageBounds bounds;

        private int method = -1;
        private Texture2D texture_;
        public Texture2D GL_Texture { get { return texture_; } }
        private Bitmap imagebase;

        public static REPT_CubeMap FromFile(string filepath, REPT.Copied_Storage.CubeMapImageBounds imageBounds)
        {
            REPT_CubeMap cmi = new REPT_CubeMap();
            cmi.method = 1;
            cmi.imagebase  =  new Bitmap(filepath);
            cmi.bounds     =  imageBounds;
            cmi.width      =  cmi.imagebase.Width;
            cmi.height     =  cmi.imagebase.Height;

            return cmi;
        }

        public static REPT_CubeMap FromResource(string resourcePath, REPT.Copied_Storage.CubeMapImageBounds imageBounds)
        {
            REPT_CubeMap cmi = new REPT_CubeMap();
            cmi.texture_ = TextureHandler.LoadTexture(resourcePath, true);
            cmi.method = 0;
            //cmi.imagebase = cmi.texture_;
            cmi.bounds = imageBounds;
            cmi.width  = cmi.texture_.Width;
            cmi.height = cmi.texture_.Height;

            return cmi;
        }

        public TextureTile GetTileBounds(int face, bool normalized)
        {
            switch (face)
            {
                case 1:
                    return new TextureTile(
                        bounds.stripLeftNW,
                        Vector2.Mix(bounds.stripLeftNW, bounds.stripLeftSW, (1 / 3.0)),
                        Vector2.Mix(bounds.stripLeftNE, bounds.stripLeftSE, (1 / 3.0)),
                        bounds.stripLeftNE
                        );
                case 2:
                    return new TextureTile(
                        Vector2.Mix(bounds.stripLeftNE, bounds.stripLeftSE, (1 / 3.0)),
                        Vector2.Mix(bounds.stripLeftNW, bounds.stripLeftSW, (1 / 3.0)),
                        Vector2.Mix(bounds.stripLeftNW, bounds.stripLeftSW, (2 / 3.0)),
                        Vector2.Mix(bounds.stripLeftNE, bounds.stripLeftSE, (2 / 3.0))
                        );
                case 3:
                    return new TextureTile(
                        Vector2.Mix(bounds.stripLeftNE, bounds.stripLeftSE, (2 / 3.0)),
                        Vector2.Mix(bounds.stripLeftNW, bounds.stripLeftSW, (2 / 3.0)),
                        bounds.stripLeftSW,
                        bounds.stripLeftSE
                        );
                case 4:
                    return new TextureTile(
                        bounds.stripRightNW,
                        Vector2.Mix(bounds.stripRightNW, bounds.stripRightNE, (1 / 3.0)),
                        Vector2.Mix(bounds.stripRightSW, bounds.stripRightSE, (1 / 3.0)),
                        bounds.stripRightSW
                        );
                case 5:
                    return new TextureTile(
                        Vector2.Mix(bounds.stripRightNW, bounds.stripRightNE, (1 / 3.0)),
                        Vector2.Mix(bounds.stripRightNW, bounds.stripRightNE, (2 / 3.0)),
                        Vector2.Mix(bounds.stripRightSW, bounds.stripRightSE, (2 / 3.0)),
                        Vector2.Mix(bounds.stripRightSW, bounds.stripRightSE, (1 / 3.0))
                        );
                case 6:
                    return new TextureTile(
                        Vector2.Mix(bounds.stripRightNW, bounds.stripRightNE, (2 / 3.0)),
                        bounds.stripRightNE,
                        bounds.stripRightSE,
                        Vector2.Mix(bounds.stripRightSW, bounds.stripRightSE, (2 / 3.0))
                        );
                default:
                    return GetTileBoundFirst(face, normalized);
            }
        }

        private TextureTile GetTileBoundFirst(int face_abs, bool normalized)
        {
            char arr = face_abs.ToString().ToCharArray()[0];
            int face;
            Int32.TryParse(arr.ToString(), out face);
            
            switch (face)
            {
                case 1:
                    return new TextureTile(
                        bounds.stripLeftNW,
                        Vector2.Mix(bounds.stripLeftNW, bounds.stripLeftSW, (1 / 3.0)),
                        Vector2.Mix(bounds.stripLeftNE, bounds.stripLeftSE, (1 / 3.0)),
                        bounds.stripLeftNE
                        );
                case 2:
                    return new TextureTile(
                        Vector2.Mix(bounds.stripLeftNE, bounds.stripLeftSE, (1 / 3.0)),
                        Vector2.Mix(bounds.stripLeftNW, bounds.stripLeftSW, (1 / 3.0)),
                        Vector2.Mix(bounds.stripLeftNW, bounds.stripLeftSW, (2 / 3.0)),
                        Vector2.Mix(bounds.stripLeftNE, bounds.stripLeftSE, (2 / 3.0))
                        );
                case 3:
                    return new TextureTile(
                        Vector2.Mix(bounds.stripLeftNE, bounds.stripLeftSE, (2 / 3.0)),
                        Vector2.Mix(bounds.stripLeftNW, bounds.stripLeftSW, (2 / 3.0)),
                        bounds.stripLeftSW,
                        bounds.stripLeftSE
                        );
                case 4:
                    return new TextureTile(
                        bounds.stripRightNW,
                        Vector2.Mix(bounds.stripRightNW, bounds.stripRightNE, (1 / 3.0)),
                        Vector2.Mix(bounds.stripRightSW, bounds.stripRightSE, (1 / 3.0)),
                        bounds.stripRightSW
                        );
                case 5:
                    return new TextureTile(
                        Vector2.Mix(bounds.stripRightNW, bounds.stripRightNE, (1 / 3.0)),
                        Vector2.Mix(bounds.stripRightNW, bounds.stripRightNE, (2 / 3.0)),
                        Vector2.Mix(bounds.stripRightSW, bounds.stripRightSE, (2 / 3.0)),
                        Vector2.Mix(bounds.stripRightSW, bounds.stripRightSE, (1 / 3.0))
                        );
                case 6:
                    return new TextureTile(
                        Vector2.Mix(bounds.stripRightNW, bounds.stripRightNE, (2 / 3.0)),
                        bounds.stripRightNE,
                        bounds.stripRightSE,
                        Vector2.Mix(bounds.stripRightSW, bounds.stripRightSE, (2 / 3.0))
                        );
                default:
                    return new TextureTile();
            }
        }

        public TextureTile GetAbsoluteTileBounds(int address)
        {
            char[] arr = address.ToString().ToCharArray();
            int face;
            Int32.TryParse(arr[0].ToString(), out face);

            TextureTile start = GetTileBounds(face, false);
            TextureTile next;
            for(int v = 0; v < arr.Length - 1; v++)
            {
                int p;
                Int32.TryParse(arr[v + 1].ToString(), out p);
                next = TextureTile.SubtileOct(start, p);
                start = next;
            }
            return start;
        }
    }

    public struct CubeMapImageBounds
    {
        public Vector2 stripLeftNW, stripLeftNE, stripLeftSW, stripLeftSE;
        public Vector2 stripRightNW, stripRightNE, stripRightSW, stripRightSE;
    }

    public class TextureTile
    {
        public Vector2 Top_Left      = new Vector2( 0, 1 );
        public Vector2 Top_Right     = new Vector2( 1, 1 );
        public Vector2 Bottom_Right  = new Vector2( 0, 0 );
        public Vector2 Bottom_Left = new Vector2( 1, 0 );

        public TextureTile() { RecalcMiddle(); }

        public TextureTile(Vector2[] vecs)
        {
            Top_Left      = vecs[0];
            Top_Right     = vecs[1];
            Bottom_Right  = vecs[2];
            Bottom_Left   = vecs[3];
            RecalcMiddle();
        }

        public void Normalize(double width, double height)
        {
            Top_Left    .X *= (1.0 / width); Top_Left    .Y *= (1.0 / height);
            Top_Right   .X *= (1.0 / width); Top_Right   .Y *= (1.0 / height);
            Bottom_Right.X *= (1.0 / width); Bottom_Right.Y *= (1.0 / height);
            Bottom_Left .X *= (1.0 / width); Bottom_Left .Y *= (1.0 / height);
            RecalcMiddle();
        }

        public void Normalize(Vector2 size)
        {
            Normalize(size.X, size.Y);
        }

        public TextureTile(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
        {
            Top_Left      = v1;
            Top_Right     = v2;
            Bottom_Right  = v3;
            Bottom_Left   = v4;
            RecalcMiddle();
        }

        private Vector2 middle;
        public Vector2 Middle { get { return middle; } }
        private void RecalcMiddle()
        {
            middle = Vector2.Average(new Vector2[4] { Top_Left, Top_Right, Bottom_Right, Bottom_Left });
        }


        public static TextureTile SubtileOct(TextureTile parent, int quadrant)
        {
            switch (quadrant)
            {
                case 1:
                    return new TextureTile ( 
                          parent.Top_Left,
                          Vector2.Mix(parent.Top_Left, parent.Top_Right, 0.5),
                          parent.Middle,
                          Vector2.Mix(parent.Top_Left, parent.Bottom_Left, 0.5)
                        );
                case 2:
                    return new TextureTile ( 
                          Vector2.Mix(parent.Top_Left, parent.Top_Right, 0.5),
                          parent.Top_Right,
                          Vector2.Mix(parent.Top_Right, parent.Bottom_Right, 0.5),
                          parent.Middle
                        );
                case 3:
                    return new TextureTile ( 
                          parent.Middle,
                          Vector2.Mix(parent.Top_Right, parent.Bottom_Right, 0.5),
                          parent.Bottom_Right,
                          Vector2.Mix(parent.Bottom_Right, parent.Bottom_Left, 0.5)
                        );
                case 4:
                    return new TextureTile ( 
                          Vector2.Mix(parent.Top_Left, parent.Bottom_Left, 0.5),
                          parent.Middle,
                          Vector2.Mix(parent.Bottom_Right, parent.Bottom_Left, 0.5),
                          parent.Bottom_Left
                        );
                default:
                    return parent;
            }
        }
    }
}
