using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Security.Cryptography;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

using Kirali.MathR;
using Kirali.Light;
using Kirali.Environment.Render.Primatives;
using Kirali.Storage;
using Kirali.REGS;

using REPT.Copied_Storage;

namespace REPT.Objects
{
    public partial class CelestialRenderObject
    {

        protected QuadSmoothCube MESH;
        protected static REPT_CubeMap CubeMap;
        protected static int baseCubSub = 5;

        public static Kirali.MathR.Vector3 LAMP_DIR = new Kirali.MathR.Vector3(3, 1, -4).Normalize();

        public Kirali.MathR.Vector3 Rotation { get { return MESH.Rotation; } set { MESH.Rotation = value; } }

        public CelestialRenderObject()
        {
            //Make Mesh
            MESH = new QuadSmoothCube(baseCubSub, 10);

            double bottom = 0.03;
            double top    = 0.97;
            //double middle_1 = (0.97 - 0.03) * (1 / 3) + 0.03;
            //double middle_2 = (0.97 - 0.03) * (2 / 3) + 0.03;


            double col1 = 0.0 + (1 - 0.975);
            double col2 = 0.5 - (1 - 0.975);
            double col3 = 0.5 + (1 - 0.975);
            double col4 = 0.975;

            //Load Cube Texture shit
            REPT.Copied_Storage.CubeMapImageBounds cmib = new REPT.Copied_Storage.CubeMapImageBounds();

            cmib.stripLeftNW  = new Kirali.MathR.Vector2( col1,    top);
            cmib.stripLeftNE  = new Kirali.MathR.Vector2( col2,    top);
            cmib.stripLeftSE  = new Kirali.MathR.Vector2( col2, bottom);
            cmib.stripLeftSW  = new Kirali.MathR.Vector2( col1, bottom);
            cmib.stripRightNE = new Kirali.MathR.Vector2( col4,    top);
            cmib.stripRightNW = new Kirali.MathR.Vector2( col4, bottom);
            cmib.stripRightSE = new Kirali.MathR.Vector2( col3,    top);
            cmib.stripRightSW = new Kirali.MathR.Vector2( col3, bottom);

            string celFilePath1 = "REPT.Resources.Celestial.Specific.PlanetMaps.";
            string celFilePath2 = "REPT.Resources.Celestial.";
            CubeMap = REPT_CubeMap.FromResource(celFilePath2 + "default_planet.png", cmib);
        }

        public void ReloadTextures()
        {
            GL.BindTexture(TextureTarget.Texture2D, CubeMap.GL_Texture.ID);
        }

        public void UnloadTextures()
        {
            //GL.DeleteTexture(CubeMap.GL_Texture.ID);
            GL.BindTexture(TextureTarget.Texture2D, 0);

        }

        public void Render(Camera MainCamera, CelestialRenderMethod mode, out int triangles)
        {
            bool naturallampshading;
            

            int quadcount;
            //Quad3D[] trix = MESH.AllMeshLayerQuad(out quadcount);

            Quad3D[] trix = MESH.AllMeshLayers_Dynamic(MainCamera.position, out quadcount);
            int limiter_rendercount = 0;
            int limit_true;

            int trict = 0;

            if (limiter_rendercount < quadcount && limiter_rendercount > 0)
                limit_true = limiter_rendercount;
            else
                limit_true = quadcount;

            //Begin Draw
            if ((int)mode < 3)
            {
                var drawOrder = trix.OrderByDescending(Quad3D => Kirali.MathR.Vector3.Distance(Quad3D.Middle, MainCamera.position)).ToArray();
                trix = (Quad3D[])drawOrder;

                GL.BindTexture(TextureTarget.Texture2D, CubeMap.GL_Texture.ID);
                Kirali.MathR.Vector2 Tex_Size = new Kirali.MathR.Vector2(CubeMap.GL_Texture.Width, CubeMap.GL_Texture.Height);

                GL.Begin(PrimitiveType.Triangles);

                for (int i = 0; i < limit_true; i++)
                {
                    Kirali.MathR.Vector2 p0, p1, p2, p3;

                    p0 = MainCamera.PointToScreen(trix[i].Points[0]);
                    p1 = MainCamera.PointToScreen(trix[i].Points[1]);
                    p2 = MainCamera.PointToScreen(trix[i].Points[2]);
                    p3 = MainCamera.PointToScreen(trix[i].Points[3]);


                    if (p0.Form != Kirali.MathR.Vector2.VectorForm.INFINITY
                        && p1.Form != Kirali.MathR.Vector2.VectorForm.INFINITY
                        && p2.Form != Kirali.MathR.Vector2.VectorForm.INFINITY
                        && p3.Form != Kirali.MathR.Vector2.VectorForm.INFINITY
                        )
                    {
                        double shading = 1;
                        int col255;

                        if (mode == CelestialRenderMethod.SHADING || mode == CelestialRenderMethod.FLAT)
                        {
                            if (mode == CelestialRenderMethod.SHADING)
                                naturallampshading = true;
                            else naturallampshading = false;

                            //TEXTURING
                            TextureTile TTileFace = CubeMap.GetAbsoluteTileBounds(trix[i].TextureLink);
                            //GL.Color3(Color.FromArgb(255, 255, 255));

                            //RIGHT

                            //shading = Kirali.MathR.Vector3.Dot(-1 * trix[i].Normal, LAMP_DIR);

                            if (naturallampshading)
                                shading = Kirali.MathR.Vector3.Dot(-1 * trix[i].Points[0].SafeNormalize(), LAMP_DIR);
                            //shading = Kirali.MathR.Vector3.Dot(trix[i].PointNormals[0].SafeNormalize(), LAMP_DIR);
                            if (shading < 0) { shading = 0; }
                            if (shading > 1) { shading = 1; }
                            col255 = (int)(shading * 255);
                            GL.Color3(Color.FromArgb(col255, col255, col255));
                            GL.TexCoord2(TTileFace.Top_Left.X, TTileFace.Top_Left.Y); GL.Vertex2(p0.X, p0.Y);

                            //shading = Kirali.MathR.Vector3.Dot(-1 * trix[i].Normal, LAMP_DIR);
                            if (naturallampshading)
                                shading = Kirali.MathR.Vector3.Dot(-1 * trix[i].Points[1].SafeNormalize(), LAMP_DIR);
                            //shading = Kirali.MathR.Vector3.Dot(trix[i].PointNormals[1].SafeNormalize(), LAMP_DIR);
                            if (shading < 0) { shading = 0; }
                            if (shading > 1) { shading = 1; }
                            col255 = (int)(shading * 255);
                            GL.Color3(Color.FromArgb(col255, col255, col255));
                            GL.TexCoord2(TTileFace.Top_Right.X, TTileFace.Top_Right.Y); GL.Vertex2(p1.X, p1.Y);

                            //shading = Kirali.MathR.Vector3.Dot(-1 * trix[i].Normal, LAMP_DIR);
                            if (naturallampshading)
                                shading = Kirali.MathR.Vector3.Dot(-1 * trix[i].Points[2].SafeNormalize(), LAMP_DIR);
                            //shading = Kirali.MathR.Vector3.Dot(trix[i].PointNormals[2].SafeNormalize(), LAMP_DIR);
                            if (shading < 0) { shading = 0; }
                            if (shading > 1) { shading = 1; }
                            col255 = (int)(shading * 255);
                            GL.Color3(Color.FromArgb(col255, col255, col255));
                            GL.TexCoord2(TTileFace.Bottom_Right.X, TTileFace.Bottom_Right.Y); GL.Vertex2(p2.X, p2.Y);


                            //LEFT


                            if (naturallampshading)
                                shading = Kirali.MathR.Vector3.Dot(-1 * trix[i].Points[2].SafeNormalize(), LAMP_DIR);
                            //shading = Kirali.MathR.Vector3.Dot(trix[i].PointNormals[2].SafeNormalize(), LAMP_DIR);
                            if (shading < 0) { shading = 0; }
                            if (shading > 1) { shading = 1; }
                            col255 = (int)(shading * 255);
                            GL.Color3(Color.FromArgb(col255, col255, col255));
                            GL.TexCoord2(TTileFace.Bottom_Right.X, TTileFace.Bottom_Right.Y); GL.Vertex2(p2.X, p2.Y);

                            if (naturallampshading)
                                shading = Kirali.MathR.Vector3.Dot(-1 * trix[i].Points[3].SafeNormalize(), LAMP_DIR);
                            //shading = Kirali.MathR.Vector3.Dot(trix[i].PointNormals[3].SafeNormalize(), LAMP_DIR);
                            if (shading < 0) { shading = 0; }
                            if (shading > 1) { shading = 1; }
                            col255 = (int)(shading * 255);
                            GL.Color3(Color.FromArgb(col255, col255, col255));
                            //GL.TexCoord2(TTileFace.Bottom_Left.X, TTileFace.Bottom_Left.Y); GL.Vertex2(p3.X, p3.Y);
                            GL.TexCoord2(TTileFace.Bottom_Left.X, TTileFace.Bottom_Left.Y); GL.Vertex2(p3.X, p3.Y);

                            if (naturallampshading)
                                shading = Kirali.MathR.Vector3.Dot(-1 * trix[i].Points[0].SafeNormalize(), LAMP_DIR);
                            //shading = Kirali.MathR.Vector3.Dot(trix[i].PointNormals[0].SafeNormalize(), LAMP_DIR);
                            if (shading < 0) { shading = 0; }
                            if (shading > 1) { shading = 1; }
                            col255 = (int)(shading * 255);
                            GL.Color3(Color.FromArgb(col255, col255, col255));
                            GL.TexCoord2(TTileFace.Top_Left.X, TTileFace.Top_Left.Y); GL.Vertex2(p0.X, p0.Y);
                        }

                        trict++;
                    }



                }
                GL.End();
            }
            else if ((int)mode == 3)
            {
                //GL.DeleteTexture(CubeMap.GL_Texture.ID);
                GL.BindTexture(TextureTarget.Texture2D, 0);

                GL.Begin(PrimitiveType.Lines);

                for (int i = 0; i < limit_true; i++)
                {
                    Kirali.MathR.Vector2 p0, p1, p2, p3;

                    p0 = MainCamera.PointToScreen(trix[i].Points[0]);
                    p1 = MainCamera.PointToScreen(trix[i].Points[1]);
                    p2 = MainCamera.PointToScreen(trix[i].Points[2]);
                    p3 = MainCamera.PointToScreen(trix[i].Points[3]);


                    if (p0.Form != Kirali.MathR.Vector2.VectorForm.INFINITY
                        && p1.Form != Kirali.MathR.Vector2.VectorForm.INFINITY
                        && p2.Form != Kirali.MathR.Vector2.VectorForm.INFINITY
                        && p3.Form != Kirali.MathR.Vector2.VectorForm.INFINITY
                        )
                    {
                        GL.Color3(Color.FromArgb(0, 255, 0));

                        GL.Vertex2(p0.X, p0.Y);
                        GL.Vertex2(p1.X, p1.Y);
                        
                        GL.Vertex2(p1.X, p1.Y);
                        GL.Vertex2(p2.X, p2.Y);
                        
                        GL.Vertex2(p2.X, p2.Y);
                        GL.Vertex2(p0.X, p0.Y);

                        
                        GL.Vertex2(p2.X, p2.Y);
                        GL.Vertex2(p3.X, p3.Y);
                        
                        GL.Vertex2(p3.X, p3.Y);
                        GL.Vertex2(p0.X, p0.Y);

                        //MIDDLE
                        //GL.Vertex2(p0.X, p0.Y);
                        //GL.Vertex2(p2.X, p2.Y);

                        trict++;
                    }



                }
                GL.End();
            }

            
            
            triangles = quadcount;
        }

    }
}
