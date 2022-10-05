using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using Newtonsoft.Json;

namespace REPT
{
    public class TextureHandler
    {

        public static Texture2D LoadTexture(string filepath)
        {
            Bitmap bitmap = new Bitmap(filepath);

            int id = GL.GenTexture();

            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.BindTexture(TextureTarget.Texture2D, id);

            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba,
                bitmap.Width, bitmap.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte,
                bmpData.Scan0);

            bitmap.UnlockBits(bmpData);

            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

            return new Texture2D(id, bitmap.Width, bitmap.Height);
        }

        public static Texture2D LoadTexture(Bitmap bitmap)
        {

            int id = GL.GenTexture();

            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.BindTexture(TextureTarget.Texture2D, id);

            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba,
                bitmap.Width, bitmap.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte,
                bmpData.Scan0);

            bitmap.UnlockBits(bmpData);

            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

            return new Texture2D(id, bitmap.Width, bitmap.Height);
        }

        public static Image LoadImage(string resourcePath)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream(resourcePath);
            Image image = new Bitmap(myStream);
            return image;
        }

        public static Texture2D LoadTexture(string resourcePath, bool fromResource)
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream myStream = myAssembly.GetManifestResourceStream(resourcePath);
            Bitmap bitmap = new Bitmap(myStream);

            int id = GL.GenTexture();

            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.BindTexture(TextureTarget.Texture2D, id);

            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba,
                bitmap.Width, bitmap.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte,
                bmpData.Scan0);

            bitmap.UnlockBits(bmpData);

            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

            return new Texture2D(id, bitmap.Width, bitmap.Height);
        }

        public static Texture2D[] LoadFrames(string filepath, int count)
        {
            Texture2D[] frameList = new Texture2D[count];

            for (int i = 0; i < count; i++)
            {
                string str = filepath + "\\" + i + ".png";
                if (i < 10) { str = filepath + "\\0" + i + ".png"; }
                frameList[i] = LoadTexture(str);
            }

            return frameList;
        }

        public static Texture2D[] LoadFrames(string filepath, int count, bool fromResource)
        {
            Texture2D[] frameList = new Texture2D[count];

            for (int i = 0; i < count; i++)
            {
                string str = filepath + "." + i + ".png";
                if (i < 10) { str = filepath + ".0" + i + ".png"; }
                frameList[i] = LoadTexture(str, true);
            }

            return frameList;
        }

        /*public static ObjectSettings LoadObjectSettings(string objectId, bool fromResource)
        {
            ObjectSettings newSettings = new ObjectSettings();
            if (fromResource)
            {
                Assembly myAssembly = Assembly.GetExecutingAssembly();
                Stream inStream = myAssembly.GetManifestResourceStream("TetreiaCore.Resources.objects." + objectId + ".json");
                string data = new StreamReader(inStream).ReadToEnd();
                newSettings = JsonConvert.DeserializeObject<ObjectSettings>(data);

            }
            else
            {

            }
            return newSettings;
        }

        public static void SaveObjectSettings(ObjectSettings settings)
        {
            string output = JsonConvert.SerializeObject(settings);
            File.WriteAllText("objects\\" + settings.internalId + ".json", output);
        }
        */
    }
}
