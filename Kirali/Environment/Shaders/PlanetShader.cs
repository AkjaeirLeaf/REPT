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
    public class PlanetShader : KShader
    {
        private FractalNoise fm = new FractalNoise(0.04, 2.56);
        private double bumpy = 1000;

        public double RADIUS;
        public PlanetShader(double r)
        {
            RADIUS = r;
        }
        public override KColor4 Diffuse(Vector3 point)
        {
            double f = m_getH(point);

            //return outColor;
            double scale = 0.0012;
            Vector3 point_ = new Vector3(point).Add(new Vector3(1.0, 1.0, 1.0) * RADIUS);
            double persist = 0.5;
            double xsk = PerlinNoise.OctavePerlin(new Vector3(point_).Add(1392.67, 4505.62, 1934) * scale, 8, persist);
            double ysk = PerlinNoise.OctavePerlin(new Vector3(point_).Add(2396.21, 1505.22, 2934) * scale, 8, persist);
            double zsk = PerlinNoise.OctavePerlin(new Vector3(point_).Add(7391.41, 2505.12, 3934) * scale, 8, persist);

            Vector3 skew = new Vector3(xsk, ysk, zsk);// * (1 / 2.36);
            scale = 0.000002;
            double p_sm = fm.HybridMultifractal((point_ + skew * 2000) * scale, 8, 0.92);
            p_sm /= 5.5;
            double ps = p_sm * Math.Pow(1.04, p_sm * p_sm * p_sm) - p_sm;

            f *= ps;

            KColor4 ground1 = (new KColor4(Interpolate.Lerp(0.29, 0.77, f),
                                           Interpolate.Lerp(0.077, 0.32, f),
                                           Interpolate.Lerp(0.026, 0.077, f))).AdjustSaturation(0.4) * 1.05;


            if(Math.Pow(Interpolate.Smooth(0.0, 1.0, 1 - f), 4) < 0.0013)
            {
                ground1 = new KColor4(0.34, 0.34, 0.53) * (f / 2 + 0.5) * 0.82;
            }

            double polar = Math.Pow(Math.Sin(0.5 * Math.PI * point.Z / RADIUS), 6.0) * (Math.Pow(f * 0.9 * m_getH(point), 2) / 2 + 0.6) * 0.8;

            KColor4 outColor = KColor4.Mix(new KColor4(0.95, 0.95, 1.0) * 0.9, ground1, polar);
            return outColor;
        }

        private double m_getH(Vector3 point)
        {
            //can't handle negative values yet for some reason?
            double scale = 5.0;
            double persist = 0.5;
            Vector3 point_ = new Vector3(point).Add(new Vector3(1.0, 2.0, 3.0) * RADIUS * scale);
            //double p_sm = PerlinNoise.OctavePerlin(scale * (normal.X + RADIUS), scale * (normal.Y + RADIUS), scale * (normal.Z + 9.3 + RADIUS), 4, persist);
            scale = 1.2;
            double p_big = Interpolate.Smooth(0.0, 1, PerlinNoise.OctavePerlin(point_ * 2.2 * (1.0 / RADIUS), 8, persist));

            double p_sm = fm.HybridMultifractal(point_ * scale * (1.0 / RADIUS), 8, 0.9);

            double f = p_sm * p_big;//Interpolate.Mix(Interpolate.Smooth(0.0, 1.0, p_sm / 10), p_big, p_sm);// Interpolate.Mix(p_sm, p_big, p_big);
            f /= 6;
            f += 0.6;
            //f = Math.Pow(Interpolate.Smooth(0.0, 1.0, f), 4.0);

            return f;
        }

        public override Vector3 Normal(Vector3 point)
        {
            double range = 1;

            double f_center = m_getH(point);

            Vector3 initial = point.Normalize();

            double dx = (m_getH((new Vector3(point)).Add(range  / 2 * Vector3.Xaxis)) 
                       - m_getH((new Vector3(point)).Add(-range / 2 * Vector3.Xaxis))) / range;
            double dy = (m_getH((new Vector3(point)).Add(range  / 2 * Vector3.Yaxis))
                       - m_getH((new Vector3(point)).Add(-range / 2 * Vector3.Yaxis))) / range;
            double dz = (m_getH((new Vector3(point)).Add(range  / 2 * Vector3.Zaxis))
                       - m_getH((new Vector3(point)).Add(-range / 2 * Vector3.Zaxis))) / range;

            double xfac = (1 - Math.Abs(Vector3.Dot(Vector3.Xaxis, initial)));
            double yfac = (1 - Math.Abs(Vector3.Dot(Vector3.Yaxis, initial)));
            double zfac = (1 - Math.Abs(Vector3.Dot(Vector3.Zaxis, initial)));

            Vector3 newNormal = new Vector3(initial);
            Vector3 adj = new Vector3(xfac * dx, yfac * dy, zfac * dz);
            newNormal.Add(adj * -1 * bumpy);

            return newNormal.Normalize();
        }

        public override KColor4 Emit(Vector3 point)
        {
            return new KColor4(0.0, 0.0, 0.0);
        }
    }
}
