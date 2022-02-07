using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.MathR;
using Kirali.Light;
using Kirali.Storage;
using Kirali.Environment;
using Kirali.Environment.Procedural;

namespace Kirali.Environment.Shaders
{
    public class RectSphereMap : KShader
    {
        public KColorImage kimageMaps;


        public double RADIUS;
        public RectSphereMap(double r, KColorImage kimage)
        {
            RADIUS = r;
            kimageMaps = kimage;
        }

        public override KColor4 Diffuse(Vector3 point)
        {
            //MAP THE 2D Image to a sphere!!!
            
            double phi = 0;
            if(point.X < 0)
            { phi = Math.PI + Math.Atan(point.Y / point.X); }
            else
            { phi = Math.Atan(point.Y / point.X); }

            double r = Math.Sqrt(point.X * point.X + point.Y * point.Y);
            double thet = Math.PI - (Math.Atan(point.Z / r) + Math.PI / 2);
            //- thet + pi/2
            //phi += Math.PI / 2;


            //think range and domain:
            //the image should domain from  zero to 2pi!
            //the image should range  from  zero to 1pi!
            double yposEX = (double)kimageMaps.height * (thet / Math.PI);
            int yposMIN = (int)Math.Floor(yposEX);
            int yposMAX = (int)Math.Ceiling(yposEX);
            if (yposEX > kimageMaps.height)
            { yposEX  -= kimageMaps.height; }
            else if (yposEX < 0)
            { yposEX  += kimageMaps.height; }
            if (yposMIN > kimageMaps.height)
            { yposMIN -= kimageMaps.height; }
            else if (yposMIN < 0)
            { yposMIN += kimageMaps.height; }
            if (yposMAX > kimageMaps.height)
            { yposMAX -= kimageMaps.height; }
            else if (yposMAX < 0)
            { yposMAX += kimageMaps.height; }
            double ry = yposEX - (double)yposMIN;

            double xposEX = (double)kimageMaps.width * 0.5 * (phi / Math.PI);
            int xposMIN = (int)Math.Floor(xposEX);
            int xposMAX = (int)Math.Ceiling(xposEX);
            if (xposEX > kimageMaps.width)
            { xposEX -= kimageMaps.width; }
            else if (xposEX < 0)
            { xposEX += kimageMaps.width; }
            if (xposMIN > kimageMaps.width)
            { xposMIN -= kimageMaps.width; }
            else if (xposMIN < 0)
            { xposMIN += kimageMaps.width; }
            if (xposMAX > kimageMaps.width)
            { xposMAX -= kimageMaps.width; }
            else if (xposMAX < 0)
            { xposMAX += kimageMaps.width; }
            double rx = xposEX - (double)xposMIN;


            KColor4 c1 = kimageMaps.GetPoint(xposMIN, yposMIN);
            KColor4 c2 = kimageMaps.GetPoint(xposMAX, yposMIN);
            KColor4 c3 = kimageMaps.GetPoint(xposMIN, yposMAX);
            KColor4 c4 = kimageMaps.GetPoint(xposMAX, yposMAX);

            double R = Interpolate.Lerp(c1.R, c2.R, rx) + Interpolate.Lerp(c3.R, c4.R, rx) + Interpolate.Lerp(c1.R, c3.R, ry) + Interpolate.Lerp(c2.R, c4.R, ry); R /= 4;
            double G = Interpolate.Lerp(c1.G, c2.G, rx) + Interpolate.Lerp(c3.G, c4.G, rx) + Interpolate.Lerp(c1.G, c3.G, ry) + Interpolate.Lerp(c2.G, c4.G, ry); G /= 4;
            double B = Interpolate.Lerp(c1.B, c2.B, rx) + Interpolate.Lerp(c3.B, c4.B, rx) + Interpolate.Lerp(c1.B, c3.B, ry) + Interpolate.Lerp(c2.B, c4.B, ry); B /= 4;

            return new KColor4(R, G, B);
        }

        public override Vector3 Normal(Vector3 point)
        {
            return new Vector3(point).Normalize();
        }
    }
}
