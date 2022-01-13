using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.Light;
using Kirali.MathR;

namespace Kirali.Environment
{
    public partial class KShader
    {
        private static KColor4 Monochrome = new KColor4(1.0, 1.0, 1.0);

        public KShader()
        {

        }

        public KShader(KColor4 monochrome)
        {
            Monochrome = monochrome;
        }

        public virtual KColor4 Diffuse(Vector3 point)
        {
            return Monochrome;
        }

        public virtual KColor4 Emit(Vector3 point)
        {
            return Monochrome;
        }

        public virtual Vector3 Normal(Vector3 point)
        {
            return new Vector3(point).Normalize();
        }
    }
}
