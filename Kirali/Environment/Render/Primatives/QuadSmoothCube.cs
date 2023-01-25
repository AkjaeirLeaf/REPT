using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.MathR;
using Kirali.Light;

namespace Kirali.Environment.Render.Primatives
{
    public class QuadSmoothCube : Quad3DMesh
    {
        private int baseSubdivisionIndex = 1;

        private double renderBackwardsLimiter = 0.1;

        private double baseRadius = 2;

        public Vector3 RenderCenter = Vector3.Zero;

        public QuadSmoothCube()
        {
            //DEF POINTS

            //            \/\/\/\/\/\/\/\/
            //    DEV SAYS PUT PIDGEON IN STOOPID HAT
            //            /\/\/\/\/\/\/\/\

            Vector3[] Points = new Vector3[8];

            Points[0] = new Vector3( -1,  1,  1);
            Points[1] = new Vector3(  1,  1,  1);
            Points[2] = new Vector3(  1, -1,  1);
            Points[3] = new Vector3( -1, -1,  1);
            Points[4] = new Vector3( -1,  1, -1);
            Points[5] = new Vector3(  1,  1, -1);
            Points[6] = new Vector3(  1, -1, -1);
            Points[7] = new Vector3( -1, -1, -1);

            Quad3D[] Q = new Quad3D[6];

            Q[0] = new Quad3D(Points[0], Points[1], Points[2], Points[3]);
            Q[1] = new Quad3D(Points[2], Points[1], Points[5], Points[6]);
            Q[2] = new Quad3D(Points[5], Points[6], Points[4], Points[7]);
            Q[3] = new Quad3D(Points[1], Points[0], Points[4], Points[5]);
            Q[4] = new Quad3D(Points[0], Points[3], Points[7], Points[4]);
            Q[5] = new Quad3D(Points[3], Points[2], Points[6], Points[7]);



            sortedPointers = new int[60];
            for (int i = 0; i < 60; i++)
            {
                sortedPointers[i] = i;
            }

            meshLayers = new PartialQuadMesh[1];
            meshLayers[0] = new PartialQuadMesh();
            meshLayers[0].points_cache = Points;
            meshLayers[0].normal_cache = Kirali.Framework.ArrayHandler.SetAll(8, Vector3.Zero);
            meshLayers[0].normal_count = Kirali.Framework.ArrayHandler.SetAll(8, 0);

            meshLayers[0].AddPartialQuad(Q[0]);
            meshLayers[0].AddPartialQuad(Q[1]);
            meshLayers[0].AddPartialQuad(Q[2]);
            meshLayers[0].AddPartialQuad(Q[3]);
            meshLayers[0].AddPartialQuad(Q[4]);
            meshLayers[0].AddPartialQuad(Q[5]);

        }


        public int GetShownTrianglesCount()
        {
            int count = 0;
            for (int layer = 0; layer < meshLayers.Length; layer++)
            {
                for (int si = 0; si < meshLayers[layer].show_hide.Length; si++)
                {
                    if (meshLayers[layer].show_hide[si]) count++;
                }
            }
            return count;
        }

        private double SubdivideMinDist(Quad3D quad)
        {
            return Vector3.Distance(quad.Points[0], quad.Points[2]) * 3;
        }

        public Quad3D[] AllMeshLayerQuad(out int quadCount, bool usePointer = false)
        {
            int count = GetShownTrianglesCount();


            Quad3D[] op = new Quad3D[count];

            int absolute = 0;
            for (int layer = 0; layer < meshLayers.Length; layer++)
            {
                for (int l = 0; l < meshLayers[layer].point_ptr.Length / 4; l++)
                {
                    if (meshLayers[layer].show_hide[l])
                    {
                        op[absolute] = GetMeshLayerQuad(layer, l);
                        absolute++;
                    }
                }
            }
            quadCount = count;
            return op;
        }

        public Quad3D[] AllMeshLayers_Dynamic(Vector3 cameraPosition, out int quadcount)
        {
            // How many triangles in last layer?
            int lastLayer_count = meshLayers[meshLayers.Length - 1].show_hide.Length;
            int count = lastLayer_count; //copy value


            //find out how many triangles Should be drawn instead, add them if needed.
            PartialQuadMesh temp = new PartialQuadMesh();
            for(int v = 0; v < lastLayer_count; v++)
            {
                Quad3D mlt1 = GetMeshLayerQuad(meshLayers.Length - 1, v);
                Quad3D mltrot = GetMeshLayerQuad(meshLayers.Length - 1, v).SafeRotateAbout(Rotation);
                if (Vector3.Dot(cameraPosition - (mltrot.Middle + RenderCenter), mltrot.Normal) > renderBackwardsLimiter)
                {
                    meshLayers[meshLayers.Length - 1].show_hide[v] = false;
                    count--;
                }
                else
                {
                    if (Vector3.Distance(mltrot.Middle + RenderCenter, cameraPosition) < SubdivideMinDist(mlt1))
                    {
                        //Do subdivide
                        count += 3;
                        meshLayers[meshLayers.Length - 1].show_hide[v] = false;

                        Quad3D[] ts = GetSubdvQuad(mlt1);
                        temp.AddPartialQuad(ts[0]);
                        temp.AddPartialQuad(ts[1]);
                        temp.AddPartialQuad(ts[2]);
                        temp.AddPartialQuad(ts[3]);
                    }
                    else { meshLayers[meshLayers.Length - 1].show_hide[v] = true; } //assure show last triangle.
                }
            }

            int closeCount;
            //PartialMesh temp_temp = new PartialMesh();
            while (true)
            {
                closeCount = 0;

                for (int v = 0; v < temp.show_hide.Length; v++)
                {
                    if (temp.show_hide[v])
                    {
                        Quad3D mlt1 = GetMeshLayerQuad(temp, v);
                        Quad3D mltrot = GetMeshLayerQuad(temp, v).SafeRotateAbout(Rotation);
                        if (Vector3.Dot(cameraPosition - (mltrot.Middle + RenderCenter), mltrot.Normal) > renderBackwardsLimiter)
                        {
                            temp.show_hide[v] = false;
                            count--;
                        }
                        else
                        {
                            if (Vector3.Distance(mltrot.Middle + RenderCenter, cameraPosition) < SubdivideMinDist(mlt1))
                            {
                                //Do subdivide
                                count += 3;
                                closeCount++;
                                temp.show_hide[v] = false;

                                Quad3D[] ts = GetSubdvQuad(mlt1);
                                temp.AddPartialQuad(ts[0]);
                                temp.AddPartialQuad(ts[1]);
                                temp.AddPartialQuad(ts[2]);
                                temp.AddPartialQuad(ts[3]);
                            }
                            else { temp.show_hide[v] = true; } //assure show last triangle.
                        }
                    }

                }

                if (closeCount == 0) { break; }
            }


            //create final array
            Quad3D[] op = new Quad3D[count];

            int absolute = 0;
            //first add all triangles from the final mesh layer:
            for (int l = 0; l < meshLayers[meshLayers.Length - 1].point_ptr.Length / 4; l++)
            {
                if(meshLayers[meshLayers.Length - 1].show_hide[l])
                {
                    op[absolute] = GetMeshLayerQuad(meshLayers.Length - 1, l).SafeRotateAbout(Rotation);
                    op[absolute].Translate(RenderCenter);
                    absolute++;
                }
            }

            //next add all the subdivided triangles!
            for(int x = 0; x < temp.show_hide.Length; x++)
            {
                if (temp.show_hide[x])
                {
                    op[absolute] = GetMeshLayerQuad(temp, x).SafeRotateAbout(Rotation);
                    op[absolute].Translate(RenderCenter);
                    absolute++;
                }
            }

            //Return!
            quadcount = count;
            return op;
        }


        public QuadSmoothCube(int baseSubdivisions, double radius)
        {
            baseRadius = radius;

            //DEF POINTS

            //            \/\/\/\/\/\/\/\/
            //    DEV SAYS PUT PIDGEON IN STOOPID HAT
            //            /\/\/\/\/\/\/\/\


            #region basepoints


            Vector3[] Points = new Vector3[8];

            Points[0] = new Vector3(-1 * radius,  1 * radius,  1 * radius);
            Points[1] = new Vector3( 1 * radius,  1 * radius,  1 * radius);
            Points[2] = new Vector3( 1 * radius, -1 * radius,  1 * radius);
            Points[3] = new Vector3(-1 * radius, -1 * radius,  1 * radius);
            Points[4] = new Vector3(-1 * radius,  1 * radius, -1 * radius);
            Points[5] = new Vector3( 1 * radius,  1 * radius, -1 * radius);
            Points[6] = new Vector3( 1 * radius, -1 * radius, -1 * radius);
            Points[7] = new Vector3(-1 * radius, -1 * radius, -1 * radius);

            Quad3D[] Q = new Quad3D[6];

            Q[0] = new Quad3D(Points[0], Points[1], Points[2], Points[3]); Q[0].TextureLink = 1;
            Q[1] = new Quad3D(Points[2], Points[1], Points[5], Points[6]); Q[1].TextureLink = 2;
            Q[2] = new Quad3D(Points[6], Points[5], Points[4], Points[7]); Q[2].TextureLink = 3;
            Q[3] = new Quad3D(Points[1], Points[0], Points[4], Points[5]); Q[3].TextureLink = 4;
            Q[4] = new Quad3D(Points[0], Points[3], Points[7], Points[4]); Q[4].TextureLink = 5;
            Q[5] = new Quad3D(Points[3], Points[2], Points[6], Points[7]); Q[5].TextureLink = 6;



            sortedPointers = new int[60];
            for (int i = 0; i < 60; i++)
            {
                sortedPointers[i] = i;
            }

            meshLayers = Kirali.Framework.ArrayHandler.SetAll(baseSubdivisions, new PartialQuadMesh());
            meshLayers[0] = new PartialQuadMesh();
            meshLayers[0].points_cache = Points;
            meshLayers[0].normal_cache = Kirali.Framework.ArrayHandler.SetAll(8, Vector3.Zero);
            meshLayers[0].normal_count = Kirali.Framework.ArrayHandler.SetAll(8, 0);

            meshLayers[0].AddPartialQuad(Q[0]); 
            meshLayers[0].AddPartialQuad(Q[1]);
            meshLayers[0].AddPartialQuad(Q[2]);
            meshLayers[0].AddPartialQuad(Q[3]);
            meshLayers[0].AddPartialQuad(Q[4]);
            meshLayers[0].AddPartialQuad(Q[5]);
            #endregion basepoints

            int iter = baseSubdivisions - 1;
            baseSubdivisionIndex = baseSubdivisions;
            
            if(baseSubdivisions > 1)
            {
                meshLayers[0].show_hide = Kirali.Framework.ArrayHandler.SetAll(meshLayers[0].point_ptr.Length / 4, false);
            }
            else
            {
                meshLayers[0].show_hide = Kirali.Framework.ArrayHandler.SetAll(meshLayers[0].point_ptr.Length / 4, true);
            }



            //Make smooother later maybe :/
            int[] cub_defSizes = new int[] { 8, 26, 98, 386, 1538, 6146 };
            int points_unique = points_cache.Length;
            int newPointCount = cub_defSizes[baseSubdivisions];

            newPointCount = points_unique; //temp
            Vector3[] newCache = new Vector3[newPointCount];

            int ix;
            for(ix = 0; ix < points_cache.Length; ix++)
            {
                newCache[ix] = points_cache[ix];
            }

            
            //t3d.DirectSetPointCache(newCache);

            int modMeshLayer = 0;

            if(iter >= 1)
            {
                modMeshLayer = 1;
                int remaining = iter;
                while(remaining > 0)
                {
                    PartialQuadMesh t3d = new PartialQuadMesh();
                    int facecount = (int)(20 * Math.Pow(4, modMeshLayer));

                    Quad3D[] temp_ = new Quad3D[facecount];
                    //for(int k = 0; k < 1; k++)
                    for (int k = 0; k < meshLayers[modMeshLayer - 1].show_hide.Length; k++)
                    {
                        Quad3D tri = GetMeshLayerQuad(modMeshLayer - 1, k);
                        Quad3D[] subs = GetSubdvQuad(tri);

                        t3d.AddPartialQuad(subs[0]);
                        t3d.AddPartialQuad(subs[1]);
                        t3d.AddPartialQuad(subs[2]);
                        t3d.AddPartialQuad(subs[3]);
                    }
                    //Set arrays to new data
                    meshLayers[modMeshLayer] = t3d;
                    meshLayers[modMeshLayer].show_hide = Kirali.Framework.ArrayHandler.SetAll(meshLayers[modMeshLayer].point_ptr.Length / 4, false);

                    //points_cache = t3d.PointData;
                    //point_ptr = t3d.TriangleData;
                    remaining--;
                    modMeshLayer++;
                }
                meshLayers[meshLayers.Length - 1].show_hide = Kirali.Framework.ArrayHandler.SetAll(meshLayers[meshLayers.Length - 1].point_ptr.Length / 4, true);
                //I think that's all for now?
            }
        }

        private Quad3D[] GetSubdvQuad(Quad3D quad)
        {
            Quad3D[] res = new Quad3D[4];

            //  p0    a    p1
            //
            //   d    e    b
            //
            //  p3    c    p2
            
            Vector3 a = Vector3.Average(quad.Points[0], quad.Points[1]);
            Vector3 b = Vector3.Average(quad.Points[1], quad.Points[2]);
            Vector3 c = Vector3.Average(quad.Points[2], quad.Points[3]);
            Vector3 d = Vector3.Average(quad.Points[3], quad.Points[0]);
            Vector3 e = 0.25 * (quad.Points[0] + quad.Points[1] + quad.Points[2] + quad.Points[3]);

            a = (baseRadius * 2 / (Vector3.Distance(a, Vector3.Zero))) * a;
            b = (baseRadius * 2 / (Vector3.Distance(b, Vector3.Zero))) * b;
            c = (baseRadius * 2 / (Vector3.Distance(c, Vector3.Zero))) * c;
            d = (baseRadius * 2 / (Vector3.Distance(d, Vector3.Zero))) * d;
            e = (baseRadius * 2 / (Vector3.Distance(e, Vector3.Zero))) * e;

            Vector3 p0 = (baseRadius * 2 / (Vector3.Distance(quad.Points[0], Vector3.Zero))) * quad.Points[0];
            Vector3 p1 = (baseRadius * 2 / (Vector3.Distance(quad.Points[1], Vector3.Zero))) * quad.Points[1];
            Vector3 p2 = (baseRadius * 2 / (Vector3.Distance(quad.Points[2], Vector3.Zero))) * quad.Points[2];
            Vector3 p3 = (baseRadius * 2 / (Vector3.Distance(quad.Points[3], Vector3.Zero))) * quad.Points[3];

            res[0] = new Quad3D(p0,  a,  e,  d); res[0].TextureLink = quad.TextureLink * 10 + 1;
            res[1] = new Quad3D( a, p1,  b,  e); res[1].TextureLink = quad.TextureLink * 10 + 2;
            res[2] = new Quad3D( e,  b, p2,  c); res[2].TextureLink = quad.TextureLink * 10 + 3;
            res[3] = new Quad3D( d,  e,  c, p3); res[3].TextureLink = quad.TextureLink * 10 + 4;

            return res;
        }

        public Quad3D GetMeshLayerQuad(int mesh, int index)
        {
            if (index < meshLayers[mesh].point_ptr.Length / 4)
            {
                Vector3[] tpoints = new Vector3[4];
                tpoints[0] = meshLayers[mesh].points_cache[meshLayers[mesh].point_ptr[index * 4 + 0]];
                tpoints[1] = meshLayers[mesh].points_cache[meshLayers[mesh].point_ptr[index * 4 + 1]];
                tpoints[2] = meshLayers[mesh].points_cache[meshLayers[mesh].point_ptr[index * 4 + 2]];
                tpoints[3] = meshLayers[mesh].points_cache[meshLayers[mesh].point_ptr[index * 4 + 3]];
                Quad3D q = new Quad3D(tpoints);

                Vector3[] normals = new Vector3[4];
                normals[0] = meshLayers[mesh].PointBlendNormal(index, 0);
                normals[1] = meshLayers[mesh].PointBlendNormal(index, 1);
                normals[2] = meshLayers[mesh].PointBlendNormal(index, 2);
                normals[3] = meshLayers[mesh].PointBlendNormal(index, 3);
                q.SetPointNormals(normals[0], normals[1], normals[2], normals[3]);

                q.TextureLink = meshLayers[mesh].texture_links[index];

                return q;
            }
            try
            {
                
            }
            catch
            {
                
            }
            throw new IndexOutOfRangeException("Referenced quad does not exist in the mesh. Use TriangleCount to check size of mesh.");
        }

        public Quad3D GetMeshLayerQuad(PartialQuadMesh mesh, int index)
        {
            try
            {
                if (index < mesh.point_ptr.Length / 4)
                {
                    Vector3[] tpoints = new Vector3[4];
                    tpoints[0] = mesh.points_cache[mesh.point_ptr[index * 4 + 0]];
                    tpoints[1] = mesh.points_cache[mesh.point_ptr[index * 4 + 1]];
                    tpoints[2] = mesh.points_cache[mesh.point_ptr[index * 4 + 2]];
                    tpoints[3] = mesh.points_cache[mesh.point_ptr[index * 4 + 3]];
                    Quad3D q = new Quad3D(tpoints);

                    Vector3[] normals = new Vector3[4];
                    normals[0] = mesh.PointBlendNormal(index, 0);
                    normals[1] = mesh.PointBlendNormal(index, 1);
                    normals[2] = mesh.PointBlendNormal(index, 2);
                    normals[3] = mesh.PointBlendNormal(index, 3);
                    q.SetPointNormals(normals[0], normals[1], normals[2], normals[3]);

                    q.TextureLink = mesh.texture_links[index];

                    return q;
                }
            }
            catch
            {

            }
            throw new IndexOutOfRangeException("Referenced quad does not exist in the mesh. Use TriangleCount to check size of mesh.");
        }
    }
}