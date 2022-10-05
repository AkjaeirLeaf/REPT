using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.MathR;
using Kirali.Light;
using Kirali.Environment.Render.Primatives;

using REPT.Copied_Storage;

namespace REPT.Objects.Celestials
{
    public class PlanetAviea : CelestialRenderObject
    {
        public PlanetAviea()
        {
            //Make Mesh
            MESH = new QuadSmoothCube(baseCubSub, 10);

            //Load Cube Texture shit
            REPT.Copied_Storage.CubeMapImageBounds cmib = new REPT.Copied_Storage.CubeMapImageBounds();

            cmib.stripLeftNW = new Kirali.MathR.Vector2(   59 / 4096.0, 4076 / 4096.0);
            cmib.stripLeftNE = new Kirali.MathR.Vector2( 1415 / 4096.0, 4076 / 4096.0);
            cmib.stripLeftSE = new Kirali.MathR.Vector2( 1415 / 4096.0,   18 / 4096.0);
            cmib.stripLeftSW = new Kirali.MathR.Vector2(   59 / 4096.0,   18 / 4096.0);
            cmib.stripRightNW = new Kirali.MathR.Vector2(2679 / 4096.0, 4076 / 4096.0);
            cmib.stripRightNE = new Kirali.MathR.Vector2(2679 / 4096.0,   18 / 4096.0);
            cmib.stripRightSE = new Kirali.MathR.Vector2(4039 / 4096.0,   18 / 4096.0);
            cmib.stripRightSW = new Kirali.MathR.Vector2(4039 / 4096.0, 4076 / 4096.0);

            string celFilePath = "REPT.Resources.Celestial.Specific.PlanetMaps.";
            CubeMap = REPT_CubeMap.FromResource(celFilePath + "Aviea.aviea_diff.png", cmib);
            //CubeMap = REPT_CubeMap.FromResource("REPT.Resources.Debug.uv1.png", cmib);
        }

    }
}
