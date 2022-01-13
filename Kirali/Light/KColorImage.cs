using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

using Kirali.MathR;

namespace Kirali.Light
{
    public class KColorImage
    {
        private int m_width = 1024;
        private int m_height = 1024;
        private KColor4[,] pixels;

        public int width { get { return m_width; } }
        public int height { get { return m_height; } }


        public KColorImage(int Width, int Height)
        {
            m_width = Width;
            m_height = Height;
            pixels = new KColor4[width, height];
        }

        public KColor4 GetPoint(int x, int y)
        {
            return pixels[x, y];
        }

        public void SetPoint(int x, int y, KColor4 color)
        {
            pixels[x, y] = color;
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

        public Bitmap ToSystemBitmap()
        {
            Bitmap bmp = new Bitmap(m_width, m_height);

            for(int y = 0; y < m_height; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    bmp.SetPixel(x, y, GetPoint(x, y).ToSystemColor());
                }
            }

            return bmp;
        }
    }
}
