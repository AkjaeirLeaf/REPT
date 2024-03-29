﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.MathR;
using Kirali.REGS;

namespace Kirali.Celestials
{
    public enum CobjectTypes
    {
        nebula = 0,
        star = 1,
        planet = 2,
        moon = 3,
        asteroid = 4,
        comet = 5,
        other = 6,
        neutronStar = 7,
        blackHole = 8,
        galaxy = 9
    }
    public class CelestialObject
    {
        public string version = "v22x09m09_v1";
        
        public bool DataDeveloped = false; // F00F
        public string objectID = ""; //F01F

        public string CelestialID = ""; //F02F
        public string FilePath = ""; //F03F
        public string[] CelestialName = { "" }; //F04F
        public CobjectTypes objectType; //F05F
        public string[] objectSoiID = { "" }; //F06F
        public string[] powerAlignmentID = { "" }; //F07F
        public string[] capitalCityID = { "" }; //F08F
        public string[] districtID = { "" }; //F09F
        public string[] speciesID = { "" }; //F0AF
        public string[] languageID = { "" }; //F0BF
        public Biome[] biomes; //F0CF
        public Time[] discoveryTime; //F0DF
        public string[] discoveryID = { "" }; //F0EF
        public double AtmosphereSize; //F0FF
        public bool volcanicActive; //F10F
        public double Radius; //F11F
        public double Mass; //F12F
        public double solpm; //F13F
        public string[] compositionObj; //F14F
        public double[] occurenceTotal; //F15F
        public double[] occurenceExtractable; //F16F
        public double avDensity; //F17F
        public double Temperature; //F18F
        public double Luminance; //F25F
        public double accFactor; //F19F
        public double sGravity; //F1AF
        public double obliquitySOI = 0; //F1BF
        public Time rotationalPeriod; //F1CF
        public Time orbitalPeriod; //F1DF
        //public Scalar1 apoapsis; //F1EF
        //public Scalar1 periapsis; //F1FF
        public double apoapsis; //F1EF
        public double periapsis; //F1FF
        public Time timePlacementOrbit; //F20F

        public double zRotOrbitSOI; //F21F
        public double trueAnomaly;

        public Vector3 SystemCenterPosition; //F22F

        public int StellarGeneration = 0; //F23F


        //Constructors

        public CelestialObject(string celestial_ID, string primaryDisplayName, CobjectTypes celestialType)
        {
            CelestialID = celestial_ID;
            CelestialName = new string[] { primaryDisplayName };
            objectType = celestialType;
        }

        public void RunAll_Update(RGalaxy galaxy, StarSystemData systemData)
        {
            int fileOrderID = systemData.fileOrderId;
            SystemPointStorage SPS = galaxy.system_points[fileOrderID];
            FilePath = galaxy.StoragePath + "\\systems\\[" + RGalaxy.SelectivePlacementInteger(fileOrderID, 9) + "] " + systemData.systemName + "\\" + CelestialID;
            //NOTE FILEPATH DOES NOT INCLUDE EXTENSION OF CELESTIAL!!!
            StellarGeneration = systemData.Generation;
            SystemCenterPosition = new Vector3(galaxy.system_points[fileOrderID].X,
                galaxy.system_points[fileOrderID].Y,
                galaxy.system_points[fileOrderID].Z);
        }



        //Data Update Methods

        public int UpdateLuminance()
        {
            if (Temperature > 0 && Radius > 0)
            {
                double L = Kirali.MathR.Physics.lumTotal(Temperature, Radius);
                Luminance = L;
            }
            return 1;
        }


    }
}
