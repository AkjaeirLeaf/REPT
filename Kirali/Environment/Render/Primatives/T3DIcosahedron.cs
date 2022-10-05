using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.MathR;
using Kirali.Light;

namespace Kirali.Environment.Render.Primatives
{
    public class T3DIcosahedron : Triangle3DMesh
    {
        private int baseSubdivisionIndex = 1;
        

        private double baseRadius = 2;

        public T3DIcosahedron()
        {
            //DEF POINTS
            Vector3[] Points = new Vector3[12];

            double t = (1.0 + Math.Sqrt(5.0)) / 2.0;

            Points[00] = new Vector3(-1d,   t,  0d) * baseRadius;
            Points[01] = new Vector3( 1d,   t,  0d) * baseRadius;
            Points[02] = new Vector3(-1d,  -t,  0d) * baseRadius;
            Points[03] = new Vector3( 1d,  -t,  0d) * baseRadius;
            Points[04] = new Vector3( 0d, -1d,   t) * baseRadius;
            Points[05] = new Vector3( 0d,  1d,   t) * baseRadius;
            Points[06] = new Vector3( 0d, -1d,  -t) * baseRadius;
            Points[07] = new Vector3( 0d,  1d,  -t) * baseRadius;
            Points[08] = new Vector3(  t,  0d, -1d) * baseRadius;
            Points[09] = new Vector3(  t,  0d,  1d) * baseRadius;
            Points[10] = new Vector3( -t,  0d, -1d) * baseRadius;
            Points[11] = new Vector3( -t,  0d,  1d) * baseRadius;

            Triangle3DMesh mesh = new Triangle3DMesh();
            points_cache = Points;
            point_ptr = new int[60];

            point_ptr[00] = 00; point_ptr[01] = 11; point_ptr[02] = 05;
            point_ptr[03] = 00; point_ptr[04] = 05; point_ptr[05] = 01;
            point_ptr[06] = 07; point_ptr[08] = 01; point_ptr[09] = 07;
            point_ptr[09] = 00; point_ptr[10] = 07; point_ptr[11] = 10;
            point_ptr[12] = 00; point_ptr[13] = 10; point_ptr[14] = 11;
            point_ptr[15] = 01; point_ptr[16] = 05; point_ptr[17] = 09;
            point_ptr[18] = 05; point_ptr[19] = 11; point_ptr[20] = 04;
            point_ptr[21] = 11; point_ptr[22] = 10; point_ptr[23] = 02;
            point_ptr[24] = 10; point_ptr[25] = 07; point_ptr[26] = 06;
            point_ptr[27] = 07; point_ptr[28] = 01; point_ptr[29] = 08;
            point_ptr[30] = 03; point_ptr[31] = 09; point_ptr[32] = 04;
            point_ptr[33] = 03; point_ptr[34] = 04; point_ptr[35] = 02;
            point_ptr[36] = 03; point_ptr[37] = 02; point_ptr[38] = 06;
            point_ptr[39] = 03; point_ptr[40] = 06; point_ptr[41] = 08;
            point_ptr[42] = 03; point_ptr[43] = 08; point_ptr[44] = 09;
            point_ptr[45] = 04; point_ptr[46] = 09; point_ptr[47] = 05;
            point_ptr[48] = 02; point_ptr[49] = 04; point_ptr[50] = 11;
            point_ptr[51] = 06; point_ptr[52] = 02; point_ptr[53] = 10;
            point_ptr[54] = 08; point_ptr[55] = 06; point_ptr[55] = 07;
            point_ptr[57] = 09; point_ptr[58] = 08; point_ptr[59] = 01;

            sortedPointers = new int[60];
            for (int i = 0; i < 60; i++)
            {
                sortedPointers[i] = i;
            }

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

        private double SubdivideMinDist(Triangle3D tri)
        {
            return Vector3.Distance(tri.Points[0], tri.Points[1]);
        }

        public Triangle3D[] AllMeshLayerTriangles(out int triangleCount, bool usePointer = false)
        {
            int count = GetShownTrianglesCount();
            

            Triangle3D[] op = new Triangle3D[count];

            int absolute = 0;
            for (int layer = 0; layer < meshLayers.Length; layer++)
            {
                for (int l = 0; l < meshLayers[layer].point_ptr.Length / 3; l++)
                {
                    if (meshLayers[layer].show_hide[l])
                    {
                        op[absolute] = GetMeshLayerTriangle(layer, l);
                        absolute++;
                    }
                }
            }
            triangleCount = count;
            return op;
        }

        public Triangle3D[] AllMeshLayers_Dynamic(Vector3 cameraPosition, out int triangleCount)
        {
            // How many triangles in last layer?
            int lastLayer_count = meshLayers[meshLayers.Length - 1].show_hide.Length;
            int count = lastLayer_count; //copy value


            //find out how many triangles Should be drawn instead, add them if needed.
            PartialTriangleMesh temp = new PartialTriangleMesh();
            for(int v = 0; v < lastLayer_count; v++)
            {
                Triangle3D mlt1 = GetMeshLayerTriangle(meshLayers.Length - 1, v);
                if (Vector3.Distance(mlt1.Middle, cameraPosition) < SubdivideMinDist(mlt1))
                {
                    //Do subdivide
                    count += 3;
                    meshLayers[meshLayers.Length - 1].show_hide[v] = false;

                    Triangle3D[] ts = GetSubdvTri(mlt1);
                    AddPartialTriangle(temp, ts[0]);
                    AddPartialTriangle(temp, ts[1]);
                    AddPartialTriangle(temp, ts[2]);
                    AddPartialTriangle(temp, ts[3]);
                }
                else { meshLayers[meshLayers.Length - 1].show_hide[v] = true; } //assure show last triangle.

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
                        Triangle3D mlt1 = GetMeshLayerTriangle(temp, v);
                        if (Vector3.Distance(mlt1.Middle, cameraPosition) < SubdivideMinDist(mlt1))
                        {
                            //Do subdivide
                            count += 3;
                            closeCount++;
                            temp.show_hide[v] = false;

                            Triangle3D[] ts = GetSubdvTri(mlt1);
                            AddPartialTriangle(temp, ts[0]);
                            AddPartialTriangle(temp, ts[1]);
                            AddPartialTriangle(temp, ts[2]);
                            AddPartialTriangle(temp, ts[3]);
                        }
                        else { temp.show_hide[v] = true; } //assure show last triangle.

                    }

                }

                if (closeCount == 0) { break; }
            }


            //create final array
            Triangle3D[] op = new Triangle3D[count];

            int absolute = 0;
            //first add all triangles from the final mesh layer:
            for (int l = 0; l < meshLayers[meshLayers.Length - 1].point_ptr.Length / 3; l++)
            {
                if(meshLayers[meshLayers.Length - 1].show_hide[l])
                {
                    op[absolute] = GetMeshLayerTriangle(meshLayers.Length - 1, l);
                    absolute++;
                }
            }

            //next add all the subdivided triangles!
            for(int x = 0; x < temp.show_hide.Length; x++)
            {
                if (temp.show_hide[x])
                {
                    op[absolute] = GetMeshLayerTriangle(temp, x);
                    absolute++;
                }
            }

            //Return!
            triangleCount = count;
            return op;
        }



        /// <summary>
        /// <tooltip>Creates a new Icosphere with subdivision. 1 or 0 makes an Icosahedron, 2+ will start subdivision.</tooltip>
        /// </summary>
        /// <param name="baseSubdivisions"></param>
        public T3DIcosahedron(int baseSubdivisions, double radius)
        {
            baseRadius = radius;

            //DEF POINTS
            #region basepoints
            Vector3[] Points = new Vector3[12];

            double t = (1.0 + Math.Sqrt(5.0)) / 2.0;

            Points[00] = new Vector3(-1d,   t,  0d) * baseRadius;
            Points[01] = new Vector3( 1d,   t,  0d) * baseRadius;
            Points[02] = new Vector3(-1d,  -t,  0d) * baseRadius;
            Points[03] = new Vector3( 1d,  -t,  0d) * baseRadius;
            Points[04] = new Vector3( 0d, -1d,   t) * baseRadius;
            Points[05] = new Vector3( 0d,  1d,   t) * baseRadius;
            Points[06] = new Vector3( 0d, -1d,  -t) * baseRadius;
            Points[07] = new Vector3( 0d,  1d,  -t) * baseRadius;
            Points[08] = new Vector3(  t,  0d, -1d) * baseRadius;
            Points[09] = new Vector3(  t,  0d,  1d) * baseRadius;
            Points[10] = new Vector3( -t,  0d, -1d) * baseRadius;
            Points[11] = new Vector3( -t,  0d,  1d) * baseRadius;

            Triangle3DMesh mesh = new Triangle3DMesh();
            points_cache = Points;
            point_ptr = new int[60];

            point_ptr[00] = 00; point_ptr[01] = 11; point_ptr[02] = 05;
            point_ptr[03] = 00; point_ptr[04] = 05; point_ptr[05] = 01;
            point_ptr[06] = 07; point_ptr[08] = 01; point_ptr[09] = 07;
            point_ptr[09] = 00; point_ptr[10] = 07; point_ptr[11] = 10;
            point_ptr[12] = 00; point_ptr[13] = 10; point_ptr[14] = 11;
            point_ptr[15] = 01; point_ptr[16] = 05; point_ptr[17] = 09;
            point_ptr[18] = 05; point_ptr[19] = 11; point_ptr[20] = 04;
            point_ptr[21] = 11; point_ptr[22] = 10; point_ptr[23] = 02;
            point_ptr[24] = 10; point_ptr[25] = 07; point_ptr[26] = 06;
            point_ptr[27] = 07; point_ptr[28] = 01; point_ptr[29] = 08;
            point_ptr[30] = 03; point_ptr[31] = 09; point_ptr[32] = 04;
            point_ptr[33] = 03; point_ptr[34] = 04; point_ptr[35] = 02;
            point_ptr[36] = 03; point_ptr[37] = 02; point_ptr[38] = 06;
            point_ptr[39] = 03; point_ptr[40] = 06; point_ptr[41] = 08;
            point_ptr[42] = 03; point_ptr[43] = 08; point_ptr[44] = 09;
            point_ptr[45] = 04; point_ptr[46] = 09; point_ptr[47] = 05;
            point_ptr[48] = 02; point_ptr[49] = 04; point_ptr[50] = 11;
            point_ptr[51] = 06; point_ptr[52] = 02; point_ptr[53] = 10;
            point_ptr[54] = 08; point_ptr[55] = 06; point_ptr[55] = 07;
            point_ptr[57] = 09; point_ptr[58] = 08; point_ptr[59] = 01;

            sortedPointers = new int[60];
            for (int i = 0; i < 60; i++)
            {
                sortedPointers[i] = i;
            }
            #endregion basepoints

            int iter = baseSubdivisions - 1;
            baseSubdivisionIndex = baseSubdivisions;
            meshLayers = Kirali.Framework.ArrayHandler.SetAll(baseSubdivisions, new PartialTriangleMesh());
            

            meshLayers[0].points_cache = points_cache;
            meshLayers[0].point_ptr = point_ptr;
            if(baseSubdivisions > 1)
            {
                meshLayers[0].show_hide = Kirali.Framework.ArrayHandler.SetAll(TriangleCount, false);
            }
            else
            {
                meshLayers[0].show_hide = Kirali.Framework.ArrayHandler.SetAll(TriangleCount, true);
            }



            //Make smooother later maybe :/
            // 12  42  162  642
            // 12 +30 +120 +480
            // 12     4*30 *4
            int[] iso_defSizes = new int[] { 12, 42, 162, 642, 2562 };
            int points_unique = points_cache.Length;
            int newPointCount = iso_defSizes[baseSubdivisions];

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
                    Triangle3DMesh t3d = new Triangle3DMesh();
                    int facecount = (int)(20 * Math.Pow(4, modMeshLayer));

                    Triangle3D[] temp_ = new Triangle3D[facecount];
                    //for(int k = 0; k < 1; k++)
                    for (int k = 0; k < meshLayers[modMeshLayer - 1].show_hide.Length; k++)
                    {
                        Triangle3D tri = GetMeshLayerTriangle(modMeshLayer - 1, k);
                        Triangle3D[] subs = GetSubdvTri(tri);

                        t3d.AddTriangle(subs[0]);
                        t3d.AddTriangle(subs[1]);
                        t3d.AddTriangle(subs[2]);
                        t3d.AddTriangle(subs[3]);
                    }
                    //Set arrays to new data
                    meshLayers[modMeshLayer].points_cache = t3d.PointData;
                    meshLayers[modMeshLayer].point_ptr    = t3d.TriangleData;
                    meshLayers[modMeshLayer].show_hide = Kirali.Framework.ArrayHandler.SetAll(meshLayers[modMeshLayer].point_ptr.Length / 3, false);

                    //points_cache = t3d.PointData;
                    //point_ptr = t3d.TriangleData;
                    remaining--;
                    modMeshLayer++;
                }
                meshLayers[meshLayers.Length - 1].show_hide = Kirali.Framework.ArrayHandler.SetAll(meshLayers[meshLayers.Length - 1].point_ptr.Length / 3, true);
                //I think that's all for now?
            }
        }

        private Triangle3D[] GetSubdvTri(Triangle3D tri)
        {
            Triangle3D[] res = new Triangle3D[4];
            
            Vector3 a = Vector3.Average(tri.Points[0], tri.Points[1]);
            Vector3 b = Vector3.Average(tri.Points[1], tri.Points[2]);
            Vector3 c = Vector3.Average(tri.Points[2], tri.Points[0]);

            a = (baseRadius * 2 / (Vector3.Distance(a, Vector3.Zero))) * a;
            b = (baseRadius * 2 / (Vector3.Distance(b, Vector3.Zero))) * b;
            c = (baseRadius * 2 / (Vector3.Distance(c, Vector3.Zero))) * c;

            Vector3 p1 = (baseRadius * 2 / (Vector3.Distance(tri.Points[0], Vector3.Zero))) * tri.Points[0];
            Vector3 p2 = (baseRadius * 2 / (Vector3.Distance(tri.Points[1], Vector3.Zero))) * tri.Points[1];
            Vector3 p3 = (baseRadius * 2 / (Vector3.Distance(tri.Points[2], Vector3.Zero))) * tri.Points[2];

            res[0] = new Triangle3D(p1, a, c);
            res[1] = new Triangle3D(p2, b, a);
            res[2] = new Triangle3D(p3, c, b);
            res[3] = new Triangle3D (a, b, c);

            return res;
        }

        public void TestRestrictShow()
        {
            meshLayers[2].show_hide = Kirali.Framework.ArrayHandler.SetAll(meshLayers[2].show_hide.Length, false);
            
            meshLayers[0].show_hide[0] = true;
            meshLayers[1].show_hide[4] = true;
            meshLayers[1].show_hide[5] = true;
            meshLayers[1].show_hide[6] = true;
            meshLayers[1].show_hide[7] = true;
            meshLayers[2].show_hide[15] = true;
            meshLayers[2].show_hide[16] = true;
            meshLayers[2].show_hide[17] = true;
            meshLayers[2].show_hide[18] = true;
            meshLayers[2].show_hide[19] = true;
            meshLayers[2].show_hide[20] = true;
            meshLayers[2].show_hide[21] = true;
            meshLayers[2].show_hide[22] = true;
        }


        public Triangle3D GetMeshLayerTriangle(int mesh, int index)
        {
            try
            {
                if (index < meshLayers[mesh].point_ptr.Length / 3)
                {
                    Vector3[] tpoints = new Vector3[3];
                    tpoints[0] = meshLayers[mesh].points_cache[meshLayers[mesh].point_ptr[index * 3 + 0]];
                    tpoints[1] = meshLayers[mesh].points_cache[meshLayers[mesh].point_ptr[index * 3 + 1]];
                    tpoints[2] = meshLayers[mesh].points_cache[meshLayers[mesh].point_ptr[index * 3 + 2]];

                    return new Triangle3D(tpoints);
                }
            }
            catch
            {
                
            }
            throw new IndexOutOfRangeException("Referenced triangle does not exist in the mesh. Use TriangleCount to check size of mesh.");
        }

        public Triangle3D GetMeshLayerTriangle(PartialTriangleMesh mesh, int index)
        {
            try
            {
                if (index < mesh.point_ptr.Length / 3)
                {
                    Vector3[] tpoints = new Vector3[3];
                    tpoints[0] = mesh.points_cache[mesh.point_ptr[index * 3 + 0]];
                    tpoints[1] = mesh.points_cache[mesh.point_ptr[index * 3 + 1]];
                    tpoints[2] = mesh.points_cache[mesh.point_ptr[index * 3 + 2]];

                    return new Triangle3D(tpoints);
                }
            }
            catch
            {

            }
            throw new IndexOutOfRangeException("Referenced triangle does not exist in the mesh. Use TriangleCount to check size of mesh.");
        }
    }
}