using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirali.MathR
{
    /// <summary>
    /// <tooltip>Class containing some basic operations and useful physical constants.</tooltip>
    /// </summary>
    public static class Physics
    {
        /// <summary><tooltip>Speed of Light</tooltip></summary>
        public const double c = 299792458;
        /// <summary><tooltip>Planck's Constant</tooltip></summary>
        public const double h = 6.62607015E-34;
        /// <summary><tooltip>Reduced Planck's Constant</tooltip></summary>
        public const double hbar = h / (2 * Math.PI);
        /// <summary><tooltip>Gravitational Constant</tooltip></summary>
        public const double G = 6.6743015E-11;
        /// <summary><tooltip>Vacuum Electric Permittivity</tooltip></summary>
        public const double eps0 = 1 / (mu0 * c * c);
        /// <summary><tooltip>Vacuum Magnetic Permeability</tooltip></summary>
        public const double mu0 = 1.25663706212E-6;
        /// <summary><tooltip>Avogadro's Number</tooltip></summary>
        public const double Na = 6.02214076E+23;
        /// <summary><tooltip>Boltzmann Constant</tooltip></summary>
        public const double kb = 1.380649E-23;
        /// <summary><tooltip>Coulomb Constant</tooltip></summary>
        public const double k = 8.9875517923E+9;
        /// <summary><tooltip>Molar Gas Constant</tooltip></summary>
        public const double R = Na * kb;
        /// <summary><tooltip>Stefan-Boltzmann Constant</tooltip></summary>
        public const double sigsb = 5.670374419E-8;
        /// <summary><tooltip>Wien Displacement Constant</tooltip></summary>
        public const double wien = 2.898E-3;

        //series erf approximation
        static double erf(double x)
        {
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            int sign = 1;
            if (x < 0)
                sign = -1;
            x = Math.Abs(x);

            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return sign * y;
        }

        public static double planckLaw(double temperature, double wavelength, char measure = 'm')
        {
            double L = wavelength;
            switch (measure)
            {
                case 'u': //input is in micrometers
                    L = wavelength / 1000000.0;
                    break;
                case 'n': //input is in nanometers
                    L = wavelength / 1000000000.0;
                    break;
                case 'c': //input is in centimeters
                    L = wavelength * 100.0;
                    break;
                case 'M': //input is in METERS
                    L = wavelength;
                    break;
                default: //input type not recognised, presume meters
                    L = wavelength;
                    break;
            }

            double num1 = 2 * Math.PI * h * c * c;
            double den1 = Math.Pow(L, 5);

            double den2 = Math.Pow(Math.E, (h * c) / (L * kb * temperature)) - 1;

            return (num1 / (den1 * den2));
        }

        public static double lumTotal(double temperature, double radius)
        {
            return (4 * Math.PI * sigsb * (radius * radius) * (temperature * temperature * temperature * temperature));
        }

        /// <summary>
        /// <tooltip>Returns the total Power emitted from all wavelengths per square surface meter of a blackbody with constant temperature. Radius not included.</tooltip>
        /// </summary>
        /// <param name="temperature"></param>
        /// <returns></returns>
        public static double lumPartial(double temperature)
        {
            return (sigsb * (temperature * temperature * temperature * temperature));
        }

        public static double maxWavelengthEmission(double temperature)
        {
            return 0;
        }
    }
}
