using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPT.Environment
{
    public class CelestialRenderWorld : RenderWorld
    {
        public CelestialRenderWorld()
        {
            //Load CRO Textures
            string celFilePath2 = "REPT.Resources.Celestial.";
            //SLOT 00
            RegisterTexture(new Texture2D(celFilePath2 + "default_planet.png"), "SURF_Planet_Default");
            //SLOT 01
            RegisterTexture(new Texture2D(celFilePath2 + "star_small_emit.png"), "SURF_Small_Star");
        }
    }
}
