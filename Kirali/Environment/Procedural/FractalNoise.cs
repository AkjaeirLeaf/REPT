using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.MathR;

namespace Kirali.Environment.Procedural
{
    public class FractalNoise
    {
        private const int MAX_OCTAVES = 16;
        private bool first = true;
        private double[] exponentArray = new double[MAX_OCTAVES];
        private double H;
        private double Lacunarity;

        public FractalNoise(double h, double lacunarity)
        {
            H = h;
            Lacunarity = lacunarity;
            double frequency;
            int i;
            //precompute
            if (first)
            {
                frequency = 1.0;
                for (i = 0; i < MAX_OCTAVES; i++)
                {
                    exponentArray[i] = Math.Pow(frequency, -1 * H);
                    frequency *= lacunarity;
                }
                first = false;
            }
        }

        public double fBm(Vector3 point, double octaves)
        {
            double value, remainder;
            int i;

            value = 0.0;
            for (i = 0; i < octaves; i++)
            {
                value += PerlinNoise.Perlin(point.X, point.Y, point.Z) * exponentArray[i];
                point *= Lacunarity;
            }

            remainder = octaves - (int)octaves;
            if (remainder != 0)
            {
                value += remainder * PerlinNoise.Perlin(point.X, point.Y, point.Z) * exponentArray[i];
            }
            return value;
        }

        public double HybridMultifractal(Vector3 point, double octaves, double offset)
        {
            double result, signal, weight, remainder;
            int i;

            result = (PerlinNoise.Perlin(point) + offset) * exponentArray[0];
            weight = result;
            /* increase frequency */
            point *= Lacunarity;

            for (i = 1; i < octaves; i++)
            {
                if (weight > 1.0) weight = 1.0;
                signal = (PerlinNoise.Perlin(point) + offset) * exponentArray[i];
                result += weight * signal;
                weight *= signal;
                point *= Lacunarity;
            }

            remainder = octaves - (int)octaves;
            if (remainder != 0)
            {
                result += remainder * PerlinNoise.Perlin(point) * exponentArray[i];
            }
            return result;
        }


    }
}
