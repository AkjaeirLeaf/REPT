using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.Environment;
using Kirali.Light;
using Kirali.MathR;

using REPT.Objects;
using REPT.Copied_Storage;

namespace REPT.Environment
{
    public partial class RenderWorld
    {
        //Static and error handling
        public static Texture2D ErrorImage;


        //Object Storage
        public Camera MainCamera;
        public CelestialRenderMethod CurrentCelestialRenderer = CelestialRenderMethod.SHADING;

        private int        TextureStorageFill = 0;
        private string[]   TextureStorageCallstrings = new string[0];
        private int[]      TextureStorageCallints = new int[0];
        public Texture2D[] TextureStorage = new Texture2D[0];

        public CelestialRenderObject[] Celestials = new CelestialRenderObject[0];

        public RenderWorld()
        {
            MainCamera = new Camera();
            Init();
        }

        public void Init()
        {

        }

        public void AddObject(CelestialRenderObject CRO)
        {
            if(Celestials.Length == 0)
            {
                Celestials = new CelestialRenderObject[] { CRO };
            }
            else
            {
                CelestialRenderObject[] temp_cro = new CelestialRenderObject[Celestials.Length + 1];
                for(int k = 0; k < Celestials.Length; k++)
                {
                    temp_cro[k] = Celestials[k];
                }
                temp_cro[Celestials.Length] = CRO;
                Celestials = temp_cro;
            }
        }
        public int RegisterTexture(Texture2D texture2D)
        {
            if (TextureStorage.Length == 0)
            {
                TextureStorage = new Texture2D[] { texture2D };
                TextureStorageCallints = new int[] { texture2D.ID };
                TextureStorageCallstrings = new string[] { "" };
            }
            else
            {
                Texture2D[] temp_texs = new Texture2D[TextureStorage.Length + 1];
                int[]       temp_texs_intCall = new int[TextureStorage.Length + 1];
                string[]    temp_texs_strCall = new string[TextureStorage.Length + 1];
                for (int k = 0; k < TextureStorage.Length; k++)
                {
                    temp_texs[k] = TextureStorage[k];
                    temp_texs_intCall[k] = TextureStorageCallints[k];
                    temp_texs_strCall[k] = TextureStorageCallstrings[k];

                }
                temp_texs[TextureStorage.Length] = texture2D;
                temp_texs_intCall[TextureStorage.Length] = texture2D.ID;
                temp_texs_strCall[TextureStorage.Length] = "";
                TextureStorage = temp_texs;
                TextureStorageCallints = temp_texs_intCall;
                TextureStorageCallstrings = temp_texs_strCall;
                TextureStorageFill++;
            }
            return 0;
        }

        public int RegisterTexture(Texture2D texture2D, string textureName)
        {
            if (TextureStorage.Length == 0)
            {
                TextureStorage = new Texture2D[] { texture2D };
                TextureStorageCallints = new int[] { texture2D.ID };
                TextureStorageCallstrings = new string[] { textureName };
            }
            else
            {
                Texture2D[] temp_texs = new Texture2D[TextureStorage.Length + 1];
                int[] temp_texs_intCall = new int[TextureStorage.Length + 1];
                string[] temp_texs_strCall = new string[TextureStorage.Length + 1];
                for (int k = 0; k < TextureStorage.Length; k++)
                {
                    temp_texs[k] = TextureStorage[k];
                    temp_texs_intCall[k] = TextureStorageCallints[k];
                    temp_texs_strCall[k] = TextureStorageCallstrings[k];

                }
                temp_texs[TextureStorage.Length] = texture2D;
                temp_texs_intCall[TextureStorage.Length] = texture2D.ID;
                temp_texs_strCall[TextureStorage.Length] = textureName;
                TextureStorage = temp_texs;
                TextureStorageCallints = temp_texs_intCall;
                TextureStorageCallstrings = temp_texs_strCall;
                TextureStorageFill++;
            }
            return 0;
        }


        public virtual void RenderAll()
        {
            var drawOrder = Celestials.OrderByDescending(CelestialRenderObject => Vector3.Distance(CelestialRenderObject.Position, MainCamera.position)).ToArray();
            CelestialRenderObject[] listOrder = (CelestialRenderObject[])drawOrder;

            for (int Cel = 0; Cel < listOrder.Length; Cel++)
            {
                listOrder[Cel].Render(MainCamera, CurrentCelestialRenderer, out _);
            }
        }

    }
}
