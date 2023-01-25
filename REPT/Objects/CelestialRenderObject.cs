using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Security.Cryptography;

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
        public static Vector3 LAMP_DIR = new Vector3(3, 1, -4).Normalize();



        //Object datas
        protected CRO_Type RenderType = CRO_Type.DEFAULT;
        protected QuadSmoothCube MESH;
        protected REPT_CubeMap CubeMap;
        protected static int baseCubSub = 4;
        protected double RenderRadius = 10;

        

        protected Texture2D HaloTexture;

        private KColor4 Hue_Alteration;
        private bool lockShade = false;

        public Vector3 Position { get { return MESH.RenderCenter; } set { MESH.RenderCenter = value; } }
        public Vector3 Rotation { get { return MESH.Rotation; } set { MESH.Rotation = value; } }

        public CelestialRenderObject()
        {
            RenderType = CRO_Type.DEFAULT;

            //Make Mesh
            MESH = new QuadSmoothCube(baseCubSub, RenderRadius);

            //Load Texture and UV Map Bounds
            REPT.Copied_Storage.CubeMapImageBounds cmib = REPT_CubeMap.DefaultCubeBounds;
            string celFilePath2 = "REPT.Resources.Celestial.";
            CubeMap = REPT_CubeMap.FromResource(celFilePath2 + "default_planet.png", cmib);
            
        }

        public CelestialRenderObject(CRO_Type celestialType, double scale)
        {
            RenderType = celestialType;

            //Make Mesh
            MESH = new QuadSmoothCube(baseCubSub, scale);
            //MESH.RenderCenter = new Vector3(0, 10, 0);

            //Load Texture and UV Map Bounds
            REPT.Copied_Storage.CubeMapImageBounds cmib = REPT_CubeMap.DefaultCubeBounds;

            if (celestialType == CRO_Type.STAR_SMALL)
            {
                CubeMap = REPT_CubeMap.FromRenderWorld(1, 1, cmib);
                //HaloTexture = TextureHandler.LoadTexture("REPT.Resources.Debug.uv1.png", true);
            }
            if (celestialType == CRO_Type.PLANET_TERRAN)
            {
                CubeMap = REPT_CubeMap.FromRenderWorld(1, 0, cmib);
            }
            lockShade = true;
            Hue_Alteration = new KColor4(1, 1, 1);
        }
        

        public void Render(Camera MainCamera, CelestialRenderMethod mode, out int triangles)
        {

            CelestialRenderMethod mode_adj;

            if(mode == CelestialRenderMethod.SHADING 
                && (lockShade) && ((int)RenderType >= 5))
            {
                mode_adj = CelestialRenderMethod.FLAT;
            }
            else { mode_adj = mode; }

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
            if ((int)mode_adj < 3)
            {
                var drawOrder = trix.OrderByDescending(Quad3D => Vector3.Distance(Quad3D.Middle, MainCamera.position)).ToArray();
                trix = (Quad3D[])drawOrder;

                Vector2 Tex_Size = new Vector2(CubeMap.GL_Texture.Width, CubeMap.GL_Texture.Height);

                for (int i = 0; i < limit_true; i++)
                {
                    //Place translated points into Vector2 Array.
                    Vector2[] projected = new Vector2[]
                    {
                        MainCamera.PointToScreen(trix[i].Points[0]),
                        MainCamera.PointToScreen(trix[i].Points[1]),
                        MainCamera.PointToScreen(trix[i].Points[2]),
                        MainCamera.PointToScreen(trix[i].Points[3])
                    };

                    //Check if points are within screen bounds.
                    //TO-DO update and change to avoid border flickering triangles...
                    if (   projected[0].Form != Vector2.VectorForm.INFINITY
                        && projected[1].Form != Vector2.VectorForm.INFINITY
                        && projected[2].Form != Vector2.VectorForm.INFINITY
                        && projected[3].Form != Vector2.VectorForm.INFINITY
                        )
                    {
                        double shading = 1;

                        if (mode_adj == CelestialRenderMethod.SHADING || mode_adj == CelestialRenderMethod.FLAT)
                        {
                            Color hue = ((Color)Hue_Alteration);
                            //Create Color Array (for vertex shading purposes
                            Color[] ColorTint = new Color[4];
                            if (mode_adj == CelestialRenderMethod.SHADING)
                            {
                                shading = Vector3.Dot(-1 * trix[i].Points[0].SafeNormalize(), LAMP_DIR);
                                if (shading < 0) { shading = 0; }
                                if (shading > 1) { shading = 1; }
                                ColorTint[0] = QuickGreyscale((int)(shading * 255));

                                shading = Vector3.Dot(-1 * trix[i].Points[1].SafeNormalize(), LAMP_DIR);
                                if (shading < 0) { shading = 0; }
                                if (shading > 1) { shading = 1; }
                                ColorTint[1] = QuickGreyscale((int)(shading * 255));

                                shading = Vector3.Dot(-1 * trix[i].Points[2].SafeNormalize(), LAMP_DIR);
                                if (shading < 0) { shading = 0; }
                                if (shading > 1) { shading = 1; }
                                ColorTint[2] = QuickGreyscale((int)(shading * 255));

                                shading = Vector3.Dot(-1 * trix[i].Points[3].SafeNormalize(), LAMP_DIR);
                                if (shading < 0) { shading = 0; }
                                if (shading > 1) { shading = 1; }
                                ColorTint[3] = QuickGreyscale((int)(shading * 255));
                            }
                            else
                            {
                                ColorTint[0] = hue;
                                ColorTint[1] = hue;
                                ColorTint[2] = hue;
                                ColorTint[3] = hue;
                            }

                            //TEXTURING, Grab Tile placement data based on quad address
                            TextureTile TTileFace = CubeMap.GetAbsoluteTileBounds(trix[i].TextureLink);

                            //Draw!
                            if(RenderType == CRO_Type.STAR_SMALL)
                            {
                                CubeMap.GL_Texture.Draw(projected, ColorTint, TTileFace);
                            }
                            else if (RenderType == CRO_Type.PLANET_TERRAN)
                            {
                                CubeMap.GL_Texture.Draw(projected, ColorTint, TTileFace);
                            }
                        }

                        trict++;
                    }



                }

            }
            else if ((int)mode_adj == 3)
            {
                //GL.DeleteTexture(CubeMap.GL_Texture.ID);
                GL.BindTexture(TextureTarget.Texture2D, 0);

                GL.Begin(PrimitiveType.Lines);

                for (int i = 0; i < limit_true; i++)
                {
                    Vector2 p0, p1, p2, p3;

                    p0 = MainCamera.PointToScreen(trix[i].Points[0]);
                    p1 = MainCamera.PointToScreen(trix[i].Points[1]);
                    p2 = MainCamera.PointToScreen(trix[i].Points[2]);
                    p3 = MainCamera.PointToScreen(trix[i].Points[3]);


                    if (p0.Form != Vector2.VectorForm.INFINITY
                        && p1.Form != Vector2.VectorForm.INFINITY
                        && p2.Form != Vector2.VectorForm.INFINITY
                        && p3.Form != Vector2.VectorForm.INFINITY
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

            //RenderAfterEffects(MainCamera);

            triangles = quadcount;
        }

        private Color QuickGreyscale(int value)
        {
            return Color.FromArgb(value, value, value);
        }

        private void RenderAfterEffects(Camera MainCamera)
        {
            //GL.Begin(PrimitiveType.Triangles);

            if (RenderType == CRO_Type.STAR_SMALL)
            {
                //For star halo:
                double dist = 1.325;
                //double dist = 1.425;

                double d = Vector3.Distance(MainCamera.position, MESH.RenderCenter);
                //double scale = 12 / Math.Sqrt(d);
                double rr = RenderRadius * 2;
                double anfle = 1 * Math.Atan(rr / Math.Sqrt(d * d - rr * rr));
                double scale = dist * anfle / MainCamera.fov;
                REPTsysWindow.WriteToTitle += (180 * anfle / Math.PI) + " degrees.  " + d;

                Vector2 center = MainCamera.PointToScreen(MESH.RenderCenter);

                if    (true)
                {
                    double ratio = (double)MainCamera.Height / MainCamera.Width;
                    Vector2[] points = new Vector2[]
                    {
                        new Vector2( 1 * ratio * scale + center.X, -1 * scale + center.Y),
                        new Vector2( 1 * ratio * scale + center.X,  1 * scale + center.Y),
                        new Vector2(-1 * ratio * scale + center.X,  1 * scale + center.Y),
                        new Vector2(-1 * ratio * scale + center.X, -1 * scale + center.Y)
                    };
                    
                    HaloTexture.Draw(points, Hue_Alteration.ToSystemColor());

                }

                //GL.End();
            }
            if (RenderType == CRO_Type.PLANET_TERRAN)
            {
                //For planet halo:

                Vector3 topLeft, topRight, bottomRight, bottomLeft;
                double dist = 1.255;
                //double dist = 1.325;

                double d = Vector3.Distance(MainCamera.position, MESH.RenderCenter);
                //double scale = 12 / Math.Sqrt(d);
                //double scale = 2 * MainCamera.fov / Math.Acos(RenderRadius / d);
                double anfle = (Math.PI / 2) - Math.Acos(2 * RenderRadius / d);
                double s = d * Math.Tan(anfle);
                //REPTsysWindow.WriteToTitle += anfle + " degrees.  " + d;
                //double scale = 0.83 * (1.0 / 30) * (90 - 180 * Math.Acos(2 * RenderRadius / d) / Math.PI);
                double scale = 1.43 * (1.0 / 30) * (90 - 180 * Math.Acos(2 * RenderRadius / d) / Math.PI);
                //scale = 1;

                topLeft = MESH.RenderCenter + s * dist * (MainCamera.CameraY - MainCamera.CameraX);
                topRight = MESH.RenderCenter + s * dist * (MainCamera.CameraY + MainCamera.CameraX);
                bottomRight = MESH.RenderCenter + s * dist * (-1 * MainCamera.CameraY + MainCamera.CameraX);
                bottomLeft = MESH.RenderCenter + s * dist * (-1 * MainCamera.CameraY - MainCamera.CameraX);


                Vector2[] points = new Vector2[]
                {
                    MainCamera.PointToScreen(topLeft),
                    MainCamera.PointToScreen(topRight),
                    MainCamera.PointToScreen(bottomRight),
                    MainCamera.PointToScreen(bottomLeft)
                };

                if (points[0].Form != Vector2.VectorForm.INFINITY
                    && points[1].Form != Vector2.VectorForm.INFINITY
                    && points[2].Form != Vector2.VectorForm.INFINITY
                    && points[3].Form != Vector2.VectorForm.INFINITY
                    )
                {
                    double ratio = (double)MainCamera.Height / MainCamera.Width;
                    points = new Vector2[]
                    {
                        MainCamera.PointToScreen(topLeft),
                        MainCamera.PointToScreen(topRight),
                        MainCamera.PointToScreen(bottomRight),
                        MainCamera.PointToScreen(bottomLeft)
                        //new Vector2( 1 * ratio * scale , -1 * scale),
                        //new Vector2( 1 * ratio * scale ,  1 * scale),
                        //new Vector2(-1 * ratio * scale ,  1 * scale),
                        //new Vector2(-1 * ratio * scale , -1 * scale)
                    };

                    HaloTexture.Draw(points, Hue_Alteration.ToSystemColor());

                }

                //GL.End();
            }
        }

    }

    public enum CRO_Type
    {
        DEFAULT        = 0,
        PLANET_TERRAN  = 1,
        PLANET_GASEOUS = 2,
        PLANETOID      = 3,
        IRREGULAR_OBJ  = 4,
        STAR_SMALL     = 5,
        STAR_MEDIUM    = 6,
        STAR_LARGE     = 7,
        STARSYSTEM     = 8,
        STARFIELDS     = 9
    }
}
