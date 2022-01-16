using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

using Kirali.Light;
using Kirali.MathR;

namespace Kirali.Storage
{
    public class KColorImage
    {
        private int m_width = 1024;
        private int m_height = 1024;
        private KColor4[,] pixels;
        private bool[,] doDraw;
        private KColor4 bgColor = KColor4.BLACK;

        public int width { get { return m_width; } }
        public int height { get { return m_height; } }

        public KColor4 defaultColor { get { return bgColor; } }

        public KColorImage(int Width, int Height)
        {
            m_width = Width;
            m_height = Height;
            pixels = new KColor4[width, height];
            doDraw = new bool[width, height];

            for (int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    doDraw[x, y] = false;
                }
            }
        }

        public void Fill(KColor4 fillColor)
        {
            for (int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    doDraw[x, y] = false;
                }
            }

            bgColor = fillColor;
        }

        public KColor4 GetPoint(int x, int y)
        {
            return pixels[x, y];
        }

        public void SetPoint(int x, int y, KColor4 color)
        {
            pixels[x, y] = color;
            doDraw[x, y] = true;
        }

        public KColorImage GetBloomMapped(double scale, double factor)
        {
            KColorImage bloomMapImage = new KColorImage(m_width, m_height);
            KColor4[,] original = pixels;
            KColor4[,] bloomMap = new KColor4[m_width, m_height];

            for (int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    KColor4 pixc = new KColor4(original[x, y]);
                    int fallrange = (int)(3.0 * scale);

                    for (int ypass = y - fallrange; ypass < y + fallrange; ypass++)
                    {
                        for (int xpass = x - fallrange; xpass < x + fallrange; xpass++)
                        {
                            if(xpass >= 0 && xpass < width && ypass >= 0 && ypass < height)
                            {
                                double intensity = original[xpass, ypass].IntensityRGB();
                                if(intensity > 1.0)
                                {
                                    pixc += factor * original[xpass, ypass] * Interpolate.GaussianFalloff(intensity,
                                        scale, Math.Sqrt((xpass - x) * (xpass - x) + (ypass - y) * (ypass - y)));

                                }
                            }
                        }
                    }


                    bloomMap[x, y] = pixc;
                }
            }
            bloomMapImage.pixels = bloomMap;
            return bloomMapImage;
        }

        public Bitmap ToSystemBitmap(bool includeAlpha = false)
        {
            Bitmap bmp = new Bitmap(m_width, m_height);

            using(Graphics g = Graphics.FromImage(bmp))
            {
                Rectangle r = new Rectangle(new Point(0, 0), new Size(m_width, m_height));
                SolidBrush br = new SolidBrush(bgColor.ToSystemColor());
                g.FillRectangle(br, r);
            }

            for(int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    if(doDraw[x, y])
                    {
                        bmp.SetPixel(x, y, GetPoint(x, y).ToSystemColor(includeAlpha));
                    }
                }
            }

            return bmp;
        }

        public static KColorImage FromSystemBitmap(Bitmap image)
        {
            KColorImage kimage = new KColorImage(image.Width, image.Height);
            for(int y = 0; y < kimage.height; y++)
            {
                for (int x = 0; x < kimage.width; x++)
                {
                    kimage.SetPoint(x, y, new KColor4(image.GetPixel(x, y)));
                    kimage.doDraw[x, y] = true;
                }
            }

            return kimage;
        }


    }
}
