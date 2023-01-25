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

        private TextureDataProvider provider = TextureDataProvider.NONE_UNKNOWN;
        private Texture2D texture_;
        private int texture_renderworld;
        private int texture_position;
        public Texture2D GL_Texture
        {
            get 
            {
                switch (provider)
                {
                    case TextureDataProvider.NONE_UNKNOWN:
                        return REPTsysWindow.ErrorTexture;
                    case TextureDataProvider.INTERNAL_CUBEMAP:
                        return texture_;
                    case TextureDataProvider.RENDERWORLD:
                        Texture2D fromWorld;
                        if(REPTsysWindow.TryGetTexture(texture_renderworld, texture_position, out fromWorld))
                        {
                            return fromWorld;
                        }
                        return REPTsysWindow.ErrorTexture;
                    default:
                        return REPTsysWindow.ErrorTexture;

                }
            } 
        }
        private Bitmap imagebase;

        public static REPT_CubeMap FromFile(string filepath, REPT.Copied_Storage.CubeMapImageBounds imageBounds)
        {
            REPT_CubeMap cmi = new REPT_CubeMap();
            cmi.provider = TextureDataProvider.NONE_UNKNOWN;
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
            cmi.provider = TextureDataProvider.INTERNAL_CUBEMAP;
            //cmi.imagebase = cmi.texture_;
            cmi.bounds = imageBounds;
            cmi.width  = cmi.texture_.Width;
            cmi.height = cmi.texture_.Height;

            return cmi;
        }

        public static REPT_CubeMap FromTexture2D(Texture2D texture, CubeMapImageBounds imageBounds)
        {
            REPT_CubeMap cmi = new REPT_CubeMap();
            cmi.texture_ = texture;
            cmi.provider = TextureDataProvider.INTERNAL_CUBEMAP;
            cmi.bounds = imageBounds;
            cmi.width = texture.Width;
            cmi.height = texture.Height;
            cmi.bounds = imageBounds;
            return cmi;
        }

        public static REPT_CubeMap FromRenderWorld(int renderworldID, int position, CubeMapImageBounds imageBounds)
        {
            REPT_CubeMap cmi = new REPT_CubeMap();
            Texture2D tex;
            if(REPTsysWindow.TryGetTexture(renderworldID, position, out tex))
            {
                cmi.texture_renderworld = renderworldID;
                cmi.texture_position = position;
            }
            else
            {   //if not found set world and position to the default error image 0/0
                cmi.texture_renderworld = 0;
                cmi.texture_position = 0;
            }

            cmi.provider = TextureDataProvider.RENDERWORLD;
            cmi.bounds = imageBounds;
            cmi.width = tex.Width;
            cmi.height = tex.Height;
            cmi.bounds = imageBounds;
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

        public TextureTile GetAbsoluteTileBounds_OLD(int address)
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

        public TextureTile GetAbsoluteTileBounds(int address)
        {
            int[] sliced = new int[(int)Math.Ceiling(Math.Log10(address))];
            double remaining = address;

            for (int ix = 0; ix < sliced.Length; ix++)
            {
                sliced[sliced.Length - 1 - ix] = (int)(remaining % 10);
                remaining = Math.Floor(remaining / 10);
            }

            TextureTile start = GetTileBounds(sliced[0], false);
            TextureTile next;
            for (int v = 0; v < sliced.Length - 1; v++)
            {
                next = TextureTile.SubtileOct(start, sliced[v + 1]);
                start = next;
            }
            return start;
        }

        public static CubeMapImageBounds DefaultCubeBounds
        {
            get
            {
                REPT.Copied_Storage.CubeMapImageBounds cmib = new REPT.Copied_Storage.CubeMapImageBounds();

                double bottom = 0.03;
                double top = 0.97;
                double col1 = 0.0 + (1 - 0.975);
                double col2 = 0.5 - (1 - 0.975);
                double col3 = 0.5 + (1 - 0.975);
                double col4 = 0.975;

                //Load Cube Texture shit

                cmib.stripLeftNW = new Kirali.MathR.Vector2(col1, top);
                cmib.stripLeftNE = new Kirali.MathR.Vector2(col2, top);
                cmib.stripLeftSE = new Kirali.MathR.Vector2(col2, bottom);
                cmib.stripLeftSW = new Kirali.MathR.Vector2(col1, bottom);
                cmib.stripRightNE = new Kirali.MathR.Vector2(col4, top);
                cmib.stripRightNW = new Kirali.MathR.Vector2(col4, bottom);
                cmib.stripRightSE = new Kirali.MathR.Vector2(col3, top);
                cmib.stripRightSW = new Kirali.MathR.Vector2(col3, bottom);

                return cmib;
            }
        }
    }

    public struct CubeMapImageBounds
    {
        public Vector2 stripLeftNW, stripLeftNE, stripLeftSW, stripLeftSE;
        public Vector2 stripRightNW, stripRightNE, stripRightSW, stripRightSE;
    }
    public enum TextureDataProvider
    {
        NONE_UNKNOWN = -1,
        INTERNAL_CUBEMAP = 0,
        RENDERWORLD = 1

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
