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

    }
}
