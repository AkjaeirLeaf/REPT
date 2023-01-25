using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;

using OpenTK.Graphics.OpenGL;

using REPT.Environment;
using REPT.Copied_Storage;

namespace REPT
{
    public class Texture2D
    {
        private int handle;
        private int width, height;

        private float x;
        private float y;
        private float angle;

        private string ResourcePath;
        private bool isCompiledResource = true;

        public int ID { get { return handle; } }
        public int Width { get { return width; } }
        public int Height { get { return height; } }

        public Texture2D(string resourcePath, RenderWorld worldSender)
        {
            ResourcePath = resourcePath;
            isCompiledResource = true;
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream(resourcePath);
            Bitmap bmp = new Bitmap(myStream);

            handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, handle);

            width  = bmp.Width;
            height = bmp.Height;

            BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0,
                          OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);

            bmp.UnlockBits(bmpData);

            worldSender.RegisterTexture(this);
        }

        public Texture2D(string resourcePath)
        {
            ResourcePath = resourcePath;
            isCompiledResource = true;
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream(resourcePath);
            Bitmap bmp = new Bitmap(myStream);

            handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, handle);

            width = bmp.Width;
            height = bmp.Height;

            BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0,
                          OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);

            bmp.UnlockBits(bmpData);

        }

        public Texture2D(int Id, int Width, int Height)
        {
            handle = Id;
            width  = Width;
            height = Height;
        }

        private void BindIfNot(TextureTarget target = TextureTarget.Texture2D)
        {
            //Bind Texture
            if (REPTsysWindow.CurrentBoundTexture != handle)
            {
                GL.BindTexture(TextureTarget.Texture2D, handle);
                REPTsysWindow.CurrentBoundTexture = handle;
            }
            GL.BindTexture(target, handle);
        }

        public void Draw(Kirali.MathR.Vector2[] pointArray)
        {
            //Translations if needed
            GL.PushMatrix();
            GL.Translate(x, y, 0);
            GL.Rotate(angle, 0d, 0d, 1d);

            //Bind Texture
            BindIfNot();

            //DRAW
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(pointArray[0].X, pointArray[0].Y);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(pointArray[1].X, pointArray[1].Y);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(pointArray[2].X, pointArray[2].Y);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(pointArray[3].X, pointArray[3].Y);
            GL.End();

            GL.PopMatrix();
        }
        public void Draw(Kirali.MathR.Vector2[] pointArray, Color tint)
        {
            //Translations if needed
            GL.PushMatrix();
            GL.Translate(x, y, 0);
            GL.Rotate(angle, 0d, 0d, 1d);

            //Bind Texture
            BindIfNot();

            //DRAW
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(tint);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(pointArray[0].X, pointArray[0].Y);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(pointArray[1].X, pointArray[1].Y);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(pointArray[2].X, pointArray[2].Y);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(pointArray[3].X, pointArray[3].Y);
            GL.End();

            GL.PopMatrix();
        }
        public void Draw(Kirali.MathR.Vector2[] pointArray, Color tint, TextureTile tile)
        {
            //Translations if needed
            GL.PushMatrix();
            GL.Translate(x, y, 0);
            GL.Rotate(angle, 0d, 0d, 1d);

            //Bind Texture
            BindIfNot();

            //DRAW
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(tint);
            GL.TexCoord2(tile.Top_Left.X     , tile.Top_Left.Y     );  GL.Vertex2(pointArray[0].X, pointArray[0].Y);
            GL.TexCoord2(tile.Top_Right.X    , tile.Top_Right.Y    );  GL.Vertex2(pointArray[1].X, pointArray[1].Y);
            GL.TexCoord2(tile.Bottom_Right.X , tile.Bottom_Right.Y );  GL.Vertex2(pointArray[2].X, pointArray[2].Y);
            GL.TexCoord2(tile.Bottom_Left.X  , tile.Bottom_Left.Y  );  GL.Vertex2(pointArray[3].X, pointArray[3].Y);
            GL.End();

            GL.PopMatrix();
        }
        public void Draw(Kirali.MathR.Vector2[] pointArray, TextureTile tile)
        {
            //Translations if needed
            GL.PushMatrix();
            GL.Translate(x, y, 0);
            GL.Rotate(angle, 0d, 0d, 1d);

            //Bind Texture
            BindIfNot();

            //DRAW
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(pointArray[0].X, pointArray[0].Y);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(pointArray[3].X, pointArray[3].Y);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(pointArray[2].X, pointArray[2].Y);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(pointArray[1].X, pointArray[1].Y);
            GL.End();

            GL.PopMatrix();
        }
        public void Draw(Kirali.MathR.Vector2[] pointArray, Color[] colorArray, TextureTile tile)
        {
            //Translations if needed
            GL.PushMatrix();
            GL.Translate(x, y, 0);
            GL.Rotate(angle, 0d, 0d, 1d);

            //Bind Texture
            BindIfNot();

            //DRAW
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(colorArray[0]); GL.TexCoord2(tile.Top_Left.X     , tile.Top_Left.Y     ); GL.Vertex2(pointArray[0].X, pointArray[0].Y); // TOP    -- LEFT
            GL.Color4(colorArray[1]); GL.TexCoord2(tile.Top_Right.X    , tile.Top_Right.Y    ); GL.Vertex2(pointArray[1].X, pointArray[1].Y); // TOP    -- RIGHT
            GL.Color4(colorArray[2]); GL.TexCoord2(tile.Bottom_Right.X , tile.Bottom_Right.Y ); GL.Vertex2(pointArray[2].X, pointArray[2].Y); // BOTTOM -- RIGHT
            GL.Color4(colorArray[3]); GL.TexCoord2(tile.Bottom_Left.X  , tile.Bottom_Left.Y  ); GL.Vertex2(pointArray[3].X, pointArray[3].Y); // BOTTOM -- LEFT
            GL.End();

            //GL.PopMatrix();
        }
        public void Draw(Kirali.MathR.Vector2[] pointArray, Color[] colorArray, Kirali.MathR.Vector2[] uvMap)
        {
            //Translations if needed
            GL.PushMatrix();
            GL.Translate(x, y, 0);
            GL.Rotate(angle, 0d, 0d, 1d);

            //Bind Texture
            BindIfNot();

            //DRAW
            GL.Begin(PrimitiveType.Triangles);
            GL.Color4(colorArray[0]); GL.TexCoord2(uvMap[0].X , uvMap[0].Y ); GL.Vertex2(pointArray[0].X, pointArray[0].Y);
            GL.Color4(colorArray[1]); GL.TexCoord2(uvMap[1].X , uvMap[1].Y ); GL.Vertex2(pointArray[1].X, pointArray[1].Y);
            GL.Color4(colorArray[2]); GL.TexCoord2(uvMap[2].X , uvMap[2].Y ); GL.Vertex2(pointArray[2].X, pointArray[2].Y);
            GL.End();

            //GL.PopMatrix();
        }

        public static void ClearTexture(TextureTarget target = TextureTarget.Texture2D)
        {
            GL.BindTexture(target, 0);
        }
    }

    public enum TextureAlignMode
    {
        CENTER,
        TOP_LEFT,
        TOP_RIGHT,
        BOTTOM_RIGHT,
        BOTTOM_LEFT
    }
}
