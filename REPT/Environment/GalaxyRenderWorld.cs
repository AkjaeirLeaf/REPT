using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.REGS;
using Kirali.Celestials;
using Kirali.MathR;

namespace REPT.Environment
{
    public class GalaxyRenderWorld : RenderWorld
    {
        public RGalaxy Galaxy;

        public GalaxyRenderWorld()
        {
            Galaxy = RGalaxy.FromFolder("D:\\Desktop Control\\VisualStudio\\V C# Net Projects\\REGS\\REGS\\bin\\Debug\\kjianoaa",
                true, true);
        }



    }
}
