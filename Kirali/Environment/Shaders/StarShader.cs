using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.MathR;
using Kirali.Light;
using Kirali.Environment;
using Kirali.Environment.Procedural;

namespace Kirali.Environment.Shaders
{
    public class StarShader : KShader
    {
        public double RADIUS;
        public StarShader(double r)
        {
            RADIUS = r;
        }
        public override KColor4 Emit(Vector3 point)
        {
            FractalNoise fm = new FractalNoise(0.04, 2.56);
            //can't handle negative values yet for some reason?
            double scale = 5.0;
            double persist = 0.5;
            Vector3 point_ = new Vector3(point).Add(new Vector3(1.0, 2.0, 3.0) * RADIUS * scale);
            //double p_sm = PerlinNoise.OctavePerlin(scale * (normal.X + RADIUS), scale * (normal.Y + RADIUS), scale * (normal.Z + 9.3 + RADIUS), 4, persist);
            scale = 1.2;
            //double p_big = Interpolate.Smooth(0.0, 1, PerlinNoise.OctavePerlin(scale * (normal.X + 3.44 + RADIUS), scale * (normal.Y + 8.5 + RADIUS), scale * (normal.Z + RADIUS), 4, persist));

            double p_sm = fm.HybridMultifractal(point_ * scale, 4, 0.9);

            double f = Interpolate.Smooth(0.0, 1.0, p_sm / 10);// Interpolate.Mix(p_sm, p_big, p_big);

            return new KColor4(f, f, f);
        }

        public override KColor4 Diffuse(Vector3 point)
        {
            return new KColor4(0.0, 0.0, 0.0);
        }
    }
}
