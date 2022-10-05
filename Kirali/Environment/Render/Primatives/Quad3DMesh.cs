using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.MathR;
using Kirali.Light;

namespace Kirali.Environment.Render.Primatives
{
    public partial class Quad3DMesh
    {
        protected int[] point_ptr;
        protected Vector3[] points_cache;
        protected PartialQuadMesh[] meshLayers;

        private Vector3 rotation = new Vector3();
        public Vector3 Rotation { get { return rotation; } set { rotation = value; } }
        public int[] TriangleData { get { return point_ptr; } }
        public Vector3[] PointData { get { return points_cache; } }

        public int QuadCount
        { get { return point_ptr.Length / 6; } }
        public int TriangleCount
        { get { return point_ptr.Length / 6; } }
        protected int[] sortedPointers;

        public Quad3DMesh()
        {
            points_cache = new Vector3[0];
            point_ptr = new int[0];
            sortedPointers = new int[0];
            meshLayers = new PartialQuadMesh[0];
        }

        public Quad3D GetQuad(int index)
        {
            if (index < QuadCount)
            {
                Vector3[] tpoints = new Vector3[4];
                tpoints[0] = points_cache[point_ptr[index * 4 + 0]];
                tpoints[1] = points_cache[point_ptr[index * 4 + 1]];
                tpoints[2] = points_cache[point_ptr[index * 4 + 2]];
                tpoints[3] = points_cache[point_ptr[index * 4 + 3]];

                return new Quad3D(tpoints);
            }
            throw new IndexOutOfRangeException("Referenced quad does not exist in the mesh. Use TriangleCount to check size of mesh.");
        }

        public void AddQuad(Quad3D quad)
        {
            //first copy old pointers
            int[] temp_point_ptr = new int[point_ptr.Length + 4];
            for (int ix = 0; ix < point_ptr.Length; ix++) { temp_point_ptr[ix] = point_ptr[ix]; }
            int[] temp_sortPointer = new int[sortedPointers.Length + 1];
            for (int ix = 0; ix < sortedPointers.Length; ix++) { temp_sortPointer[ix] = sortedPointers[ix]; }
            temp_sortPointer[sortedPointers.Length] = QuadCount - 1;
            sortedPointers = temp_sortPointer;


            //then check if any of the new points already exist in points_cache
            bool[] fp = new bool[4]; fp[0] = false; fp[1] = false; fp[2] = false; fp[3] = false;
            int new_addCache = 0;
            for (int pc = 0; pc < points_cache.Length; pc++)
            {
                if (quad.Points[0] == points_cache[pc]) { temp_point_ptr[point_ptr.Length + 0] = pc; fp[0] = true; }
                if (quad.Points[1] == points_cache[pc]) { temp_point_ptr[point_ptr.Length + 1] = pc; fp[1] = true; }
                if (quad.Points[2] == points_cache[pc]) { temp_point_ptr[point_ptr.Length + 2] = pc; fp[2] = true; }
                if (quad.Points[3] == points_cache[pc]) { temp_point_ptr[point_ptr.Length + 3] = pc; fp[3] = true; }
            }
            //the check total of how many new points are needed
            //add pointers to end of temp pointer array.
            new_addCache = 0;
            if (!fp[0]) { temp_point_ptr[point_ptr.Length + 0] = new_addCache + points_cache.Length; new_addCache++; }

            if (!fp[1]) { temp_point_ptr[point_ptr.Length + 1] = new_addCache + points_cache.Length; new_addCache++; }
            
            if (!fp[2]) { temp_point_ptr[point_ptr.Length + 2] = new_addCache + points_cache.Length; new_addCache++; }

            if (!fp[3]) { temp_point_ptr[point_ptr.Length + 3] = new_addCache + points_cache.Length; new_addCache++; }
            point_ptr = temp_point_ptr; //we are now done with this array and can discard
                                        //the old pointer array for the new one.


            //make temp cache array and copy old array:
            Vector3[] temp_points_cache = new Vector3[points_cache.Length + new_addCache];
            for (int opc = 0; opc < points_cache.Length; opc++)
            { temp_points_cache[opc] = points_cache[opc]; } //old vals copied.
            new_addCache = 0; //reset repurpose as indexer for new vals;
            if (!fp[0]) { temp_points_cache[points_cache.Length + new_addCache] = quad.Points[0]; new_addCache++; }
            if (!fp[1]) { temp_points_cache[points_cache.Length + new_addCache] = quad.Points[1]; new_addCache++; }
            if (!fp[2]) { temp_points_cache[points_cache.Length + new_addCache] = quad.Points[2]; new_addCache++; }
            if (!fp[3]) { temp_points_cache[points_cache.Length + new_addCache] = quad.Points[3]; }
            points_cache = temp_points_cache; //all complete!!!
        }

        public void RemoveQuad(int index)
        {
            //check if triangle even exists:
            if (index < TriangleCount)
            {
                //remove sort pointer
                int[] temp_sortPointer = new int[sortedPointers.Length - 1];
                for (int ix = 0; ix < index; ix++) { temp_sortPointer[ix] = sortedPointers[ix]; }
                for (int ix = index + 1; ix < QuadCount - 1; ix++) { temp_sortPointer[ix - 1] = sortedPointers[ix]; }
                sortedPointers = temp_sortPointer;


                //Get triangle indeces
                int[] thisIndex = new int[4];
                thisIndex[0] = point_ptr[index * 4 + 0];
                thisIndex[1] = point_ptr[index * 4 + 1];
                thisIndex[2] = point_ptr[index * 4 + 2];
                thisIndex[3] = point_ptr[index * 4 + 3];

                //next, check how many times each point in this triangle is used through the mesh:
                int[] usedDup  = new int[4];
                int[] location = new int[4];
                for (int pt = 0; pt < point_ptr.Length; pt++)
                {
                    if (thisIndex[0] == point_ptr[pt]) { location[0] = pt; usedDup[0]++; }
                    if (thisIndex[1] == point_ptr[pt]) { location[1] = pt; usedDup[1]++; }
                    if (thisIndex[2] == point_ptr[pt]) { location[2] = pt; usedDup[2]++; }
                    if (thisIndex[3] == point_ptr[pt]) { location[3] = pt; usedDup[3]++; }
                } // now we should know if the point can be completely removed or not, but first,
                  // we can remove those pesky pointers from the pointer array now:
                int[] temp_pointers = new int[point_ptr.Length - 4];
                for (int ptx = 0; ptx < index * 4; ptx++)
                { temp_pointers[ptx] = point_ptr[ptx]; } //copy before
                for (int ptx = index * 4; ptx < temp_pointers.Length; ptx++)
                { temp_pointers[ptx] = point_ptr[ptx - 4]; } //copy afterd
                point_ptr = temp_pointers; //replace old pointer array!


                //how many points can be removed from the points cache?
                int removeCount = 0;
                if (usedDup[0] == 1) { removeCount++; }
                if (usedDup[1] == 1) { removeCount++; }
                if (usedDup[2] == 1) { removeCount++; }
                if (usedDup[3] == 1) { removeCount++; }

                //create temp cache:
                Vector3[] temp_pointCache = new Vector3[points_cache.Length - removeCount];
                removeCount = 0; //reset and use as a counter
                //now we must surgically remove any points within the cache we no longer need.
                for (int tpc = 0; tpc < points_cache.Length; tpc++)
                {
                    if (location[0] == tpc && usedDup[0] == 1) { removeCount++; }
                    else if (location[1] == tpc && usedDup[1] == 1) { removeCount++; }
                    else if (location[2] == tpc && usedDup[2] == 1) { removeCount++; }
                    else if (location[3] == tpc && usedDup[3] == 1) { removeCount++; }
                    else
                    {
                        temp_pointCache[tpc - removeCount] = points_cache[tpc];
                    }
                }
                points_cache = temp_pointCache; //all done, replace array!
            }
            //Triangle doesn't exist!!!
            else { throw new IndexOutOfRangeException("Referenced quad does not exist in the mesh. Use TriangleCount to check size of mesh."); }
        }

        


        protected void RemoveAllCache()
        {

        }

        public void DirectSetPointCache(Vector3[] points)
        {
            points_cache = points;
        }

        public bool DoesCollide(Vector3 initPos, Vector3 rayCast, double zlimiter = 0.0005)
        {
            for (int q = 0; q < QuadCount; q++)
            {
                Quad3D c = GetQuad(q);
                if (c.Left.RayDoesIntersect(initPos, rayCast) || c.Right.RayDoesIntersect(initPos, rayCast))
                {
                    return true;
                }
            }
            return false;
        }

        public static Quad3DMesh GridAutoMesh(int x, int y, Vector3[] quickPoints)
        {
            Quad3DMesh new_mesh = new Quad3DMesh();

            if (quickPoints.Length != x * y) { throw new Exception("You must enter x * y points into the array"); }

            for (int yc = 0; yc < y - 1; yc++)
            {
                for (int xc = 0; xc < x - 1; xc++)
                {
                    int index = xc + yc * x;
                    new_mesh.AddQuad(new Quad3D(quickPoints[index], quickPoints[index + 1], quickPoints[index + x], quickPoints[index + x + 1]));
                }
            }

            return new_mesh;
        }

        public Quad3D[] AllQuads(bool usePointer = false)
        {
            Quad3D[] op = new Quad3D[QuadCount];
            if (usePointer)
            {
                for (int l = 0; l < TriangleCount; l++)
                {
                    op[l] = GetQuad(sortedPointers[l]);
                }
            }
            else
            {
                for (int l = 0; l < TriangleCount; l++)
                {
                    op[l] = GetQuad(l);
                }
            }
            return op;
        }

        

        //    GENERATE   PRIMATIVE   MESHES !!!


        public static Triangle3DMesh Icosahedron
        {
            get
            {
                return new T3DIcosahedron();
            }
        }
    }

    /// <summary>
    /// <tooltip>Use this struct to store points/triangles and activate/deactivate them at will.</tooltip>
    /// </summary>
    public class PartialQuadMesh
    {
        public int[] point_ptr          = new int[0];
        public bool[] show_hide         = new bool[0];
        public Vector3[] points_cache   = new Vector3[0];
        
        //IMPLEMENT NORMALCACHE ( 1x per vertex in points_cache )
        public Vector3[] normal_cache  = new Vector3[0];
        public int[]     normal_count  = new int[0];

        //TEXTURE PARTIAL LINK ( 1x per quad assoc with show_hide )
        public int[]     texture_links  = new int[0];

        //GET NORMAL USING BLEND
        public Vector3 PointBlendNormal(int index)
        {
            return new Vector3((1.0 / normal_count[index]) * normal_cache[index]);
        }
        public Vector3 PointBlendNormal(int quadIndex, int point)
        {
            int indexpt = point_ptr[quadIndex * 4 + point];
            return PointBlendNormal(indexpt);
        }

        public void AddPartialQuad(Quad3D quad)
        {
            //first copy old pointers
            int quadCount = show_hide.Length;
            int[] temp_point_ptr = new int[point_ptr.Length + 4];
            for (int ix = 0; ix < point_ptr.Length; ix++) { temp_point_ptr[ix] = point_ptr[ix]; }

            //Fill show/hide
            bool[] temp_showhide = new bool[show_hide.Length + 1];
            for (int ix = 0; ix < show_hide.Length; ix++) { temp_showhide[ix] = show_hide[ix]; }
            temp_showhide[show_hide.Length] = true;
            show_hide = temp_showhide;

            //Address info
            int[] temp_texlink = new int[texture_links.Length + 1];
            for (int ix = 0; ix < texture_links.Length; ix++) { temp_texlink[ix] = texture_links[ix]; }
            temp_texlink[texture_links.Length] = quad.TextureLink;
            texture_links = temp_texlink;


            //then check if any of the new points already exist in points_cache
            //If point already exists, add quad normal and add normal count.
            bool[] fp = new bool[4]; fp[0] = false; fp[1] = false; fp[2] = false;
            int new_addCache;
            for (int pc = 0; pc < points_cache.Length; pc++)
            {
                if (quad.Points[0] == points_cache[pc]) { temp_point_ptr[point_ptr.Length + 0] = pc; fp[0] = true;
                    normal_cache[pc] += quad.Normal;
                    normal_count[pc]++;
                }
                if (quad.Points[1] == points_cache[pc]) { temp_point_ptr[point_ptr.Length + 1] = pc; fp[1] = true;
                    normal_cache[pc] += quad.Normal;
                    normal_count[pc]++;
                }
                if (quad.Points[2] == points_cache[pc]) { temp_point_ptr[point_ptr.Length + 2] = pc; fp[2] = true;
                    normal_cache[pc] += quad.Normal;
                    normal_count[pc]++;
                }
                if (quad.Points[3] == points_cache[pc]) { temp_point_ptr[point_ptr.Length + 3] = pc; fp[3] = true;
                    normal_cache[pc] += quad.Normal;
                    normal_count[pc]++;
                }
            }
            //the check total of how many new points are needed
            //add pointers to end of temp pointer array.
            new_addCache = 0;
            if (!fp[0]) { temp_point_ptr[point_ptr.Length + 0] = new_addCache + points_cache.Length; new_addCache++; }

            if (!fp[1]) { temp_point_ptr[point_ptr.Length + 1] = new_addCache + points_cache.Length; new_addCache++; }
            
            if (!fp[2]) { temp_point_ptr[point_ptr.Length + 2] = new_addCache + points_cache.Length; new_addCache++; }

            if (!fp[3]) { temp_point_ptr[point_ptr.Length + 3] = new_addCache + points_cache.Length; new_addCache++; }
            point_ptr = temp_point_ptr; //we are now done with this array and can discard
                                             //the old pointer array for the new one.


            //make temp cache array and copy old array:
            Vector3[] temp_points_cache = new Vector3[points_cache.Length + new_addCache];
            Vector3[] temp_normal_cache = new Vector3[points_cache.Length + new_addCache];
            int[]     temp_normal_count = new     int[points_cache.Length + new_addCache];
            for (int opc = 0; opc < points_cache.Length; opc++)
            { temp_points_cache[opc] = points_cache[opc]; }
            for (int opc = 0; opc < points_cache.Length; opc++)
            { temp_normal_cache[opc] = normal_cache[opc]; }
            for (int opc = 0; opc < points_cache.Length; opc++)
            { temp_normal_count[opc] = normal_count[opc]; } //old vals copied.
            new_addCache = 0; //reset repurpose as indexer for new vals;
            if (!fp[0]) {
                temp_points_cache[points_cache.Length + new_addCache] = quad.Points[0];
                temp_normal_cache[normal_cache.Length + new_addCache] = quad.Normal;
                temp_normal_count[normal_count.Length + new_addCache] = 1;
                new_addCache++;
            }
            if (!fp[1]) {
                temp_points_cache[points_cache.Length + new_addCache] = quad.Points[1];
                temp_normal_cache[normal_cache.Length + new_addCache] = quad.Normal;
                temp_normal_count[normal_count.Length + new_addCache] = 1;
                new_addCache++; 
            }
            if (!fp[2]) {
                temp_points_cache[points_cache.Length + new_addCache] = quad.Points[2];
                temp_normal_cache[normal_cache.Length + new_addCache] = quad.Normal;
                temp_normal_count[normal_count.Length + new_addCache] = 1;
                new_addCache++; 
            }
            if (!fp[3]) {
                temp_points_cache[points_cache.Length + new_addCache] = quad.Points[3];
                temp_normal_cache[normal_cache.Length + new_addCache] = quad.Normal;
                temp_normal_count[normal_count.Length + new_addCache] = 1;
            }
            points_cache = temp_points_cache;
            normal_cache = temp_normal_cache;
            normal_count = temp_normal_count;
            //all complete!!!
        }

        public void RemovePartialQuad(int index)
        {
            //check if triangle even exists:
            if (index < show_hide.Length)
            {
                int TriCount = show_hide.Length; //ORIGINAL (not - 1)
                //remove sort pointer
                bool[] temp_showhide = new bool[show_hide.Length - 1];
                for (int ix = 0; ix < index; ix++) { temp_showhide[ix] = show_hide[ix]; }
                for (int ix = index + 1; ix < TriCount - 1; ix++) { temp_showhide[ix - 1] = show_hide[ix]; }
                show_hide = temp_showhide;


                //Get triangle indeces
                int[] thisIndex = new int[3];
                thisIndex[0] = point_ptr[index * 4 + 0];
                thisIndex[1] = point_ptr[index * 4 + 1];
                thisIndex[2] = point_ptr[index * 4 + 2];
                thisIndex[3] = point_ptr[index * 4 + 3];

                //next, check how many times each point in this triangle is used through the mesh:
                int[] usedDup  = new int[4];
                int[] location = new int[4]; // for cache positions
                for (int pt = 0; pt < point_ptr.Length; pt++)
                {
                    if (thisIndex[0] == point_ptr[pt]) { location[0] = pt; usedDup[0]++; }
                    if (thisIndex[1] == point_ptr[pt]) { location[1] = pt; usedDup[1]++; }
                    if (thisIndex[2] == point_ptr[pt]) { location[2] = pt; usedDup[2]++; }
                    if (thisIndex[3] == point_ptr[pt]) { location[3] = pt; usedDup[3]++; }
                } // now we should know if the point can be completely removed or not, but first,
                  // we can remove those pesky pointers from the pointer array now:
                int[] temp_pointers = new int[point_ptr.Length - 3];
                for (int ptx = 0; ptx < index * 4; ptx++)
                { temp_pointers[ptx] = point_ptr[ptx]; } //copy before
                for (int ptx = index * 4; ptx < temp_pointers.Length; ptx++)
                { temp_pointers[ptx] = point_ptr[ptx - 4]; } //copy afterd
                point_ptr = temp_pointers; //replace old pointer array!


                //how many points can be removed from the points cache?
                int removeCount = 0;
                if (usedDup[0] == 1) { removeCount++; }
                if (usedDup[1] == 1) { removeCount++; }
                if (usedDup[2] == 1) { removeCount++; }
                if (usedDup[3] == 1) { removeCount++; }

                //create temp cache:
                Vector3[] temp_pointCache  = new Vector3[points_cache.Length - removeCount];
                Vector3[] temp_normalCache = new Vector3[normal_cache.Length - removeCount];
                int[]     temp_normalCount = new     int[normal_count.Length - removeCount];
                removeCount = 0; //reset and use as a counter
                //now we must surgically remove any points within the cache we no longer need.
                for (int tpc = 0; tpc < points_cache.Length; tpc++)
                {
                    if (location[0] == tpc && usedDup[0] == 1)      { removeCount++; }
                    else if (location[1] == tpc && usedDup[1] == 1) { removeCount++; }
                    else if (location[2] == tpc && usedDup[2] == 1) { removeCount++; }
                    else if (location[3] == tpc && usedDup[3] == 1) { removeCount++; }
                    else
                    {
                        temp_pointCache  [tpc - removeCount] = points_cache[tpc];
                        temp_normalCache [tpc - removeCount] = normal_cache[tpc];
                        temp_normalCount [tpc - removeCount] = normal_count[tpc];
                    }
                }
                points_cache = temp_pointCache; //all done, replace array!
            }
            //Triangle doesn't exist!!!
            else { throw new IndexOutOfRangeException("Referenced quad does not exist in the  Use TriangleCount to check size of "); }
        }
    }

}
