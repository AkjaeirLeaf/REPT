using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.MathR;
using Kirali.Light;

namespace Kirali.Environment.Render.Primatives
{
    public partial class Triangle3DMesh
    {
        protected int[] point_ptr;
        protected Vector3[] points_cache;
        protected PartialTriangleMesh[] meshLayers;
        public int[] TriangleData { get { return point_ptr; } }
        public Vector3[] PointData { get { return points_cache; } }

        public int TriangleCount
        { get { return point_ptr.Length / 3; } }
        protected int[] sortedPointers;

        public Triangle3DMesh()
        {
            points_cache = new Vector3[0];
            point_ptr = new int[0];
            sortedPointers = new int[0];
        }

        public Triangle3D GetTriangle(int index)
        {
            if (index < TriangleCount)
            {
                Vector3[] tpoints = new Vector3[3];
                tpoints[0] = points_cache[point_ptr[index * 3 + 0]];
                tpoints[1] = points_cache[point_ptr[index * 3 + 1]];
                tpoints[2] = points_cache[point_ptr[index * 3 + 2]];

                return new Triangle3D(tpoints);
            }
            throw new IndexOutOfRangeException("Referenced triangle does not exist in the mesh. Use TriangleCount to check size of mesh.");
        }

        public void AddTriangle(Triangle3D tri)
        {
            if(points_cache.Length >= 98) 
            { }


            //first copy old pointers
            int[] temp_point_ptr = new int[point_ptr.Length + 3];
            for (int ix = 0; ix < point_ptr.Length; ix++) { temp_point_ptr[ix] = point_ptr[ix]; }
            int[] temp_sortPointer = new int[sortedPointers.Length + 1];
            for (int ix = 0; ix < sortedPointers.Length; ix++) { temp_sortPointer[ix] = sortedPointers[ix]; }
            temp_sortPointer[sortedPointers.Length] = TriangleCount - 1;
            sortedPointers = temp_sortPointer;


            //then check if any of the new points already exist in points_cache
            bool[] fp = new bool[3]; fp[0] = false; fp[1] = false; fp[2] = false;
            int new_addCache = 0;
            for (int pc = 0; pc < points_cache.Length; pc++)
            {
                if (tri.Points[0] == points_cache[pc]) { temp_point_ptr[point_ptr.Length + 0] = pc; fp[0] = true; }
                if (tri.Points[1] == points_cache[pc]) { temp_point_ptr[point_ptr.Length + 1] = pc; fp[1] = true; }
                if (tri.Points[2] == points_cache[pc]) { temp_point_ptr[point_ptr.Length + 2] = pc; fp[2] = true; }
            }
            //the check total of how many new points are needed
            //add pointers to end of temp pointer array.
            new_addCache = 0;
            if (!fp[0]) { temp_point_ptr[point_ptr.Length + 0] = new_addCache + points_cache.Length; new_addCache++; }
            
            if (!fp[1]) { temp_point_ptr[point_ptr.Length + 1] = new_addCache + points_cache.Length; new_addCache++; }
            
            if (!fp[2]) { temp_point_ptr[point_ptr.Length + 2] = new_addCache + points_cache.Length; new_addCache++; }
            point_ptr = temp_point_ptr; //we are now done with this array and can discard
                                        //the old pointer array for the new one.


            //make temp cache array and copy old array:
            Vector3[] temp_points_cache = new Vector3[points_cache.Length + new_addCache];
            for (int opc = 0; opc < points_cache.Length; opc++)
            { temp_points_cache[opc] = points_cache[opc]; } //old vals copied.
            new_addCache = 0; //reset repurpose as indexer for new vals;
            if (!fp[0]) { temp_points_cache[points_cache.Length + new_addCache] = tri.Points[0]; new_addCache++; }
            if (!fp[1]) { temp_points_cache[points_cache.Length + new_addCache] = tri.Points[1]; new_addCache++; }
            if (!fp[2]) { temp_points_cache[points_cache.Length + new_addCache] = tri.Points[2]; }
            points_cache = temp_points_cache; //all complete!!!
        }

        public void RemoveTriangle(int index)
        {
            //check if triangle even exists:
            if (index < TriangleCount)
            {
                //remove sort pointer
                int[] temp_sortPointer = new int[sortedPointers.Length - 1];
                for (int ix = 0; ix < index; ix++) { temp_sortPointer[ix] = sortedPointers[ix]; }
                for (int ix = index + 1; ix < TriangleCount - 1; ix++) { temp_sortPointer[ix - 1] = sortedPointers[ix]; }
                sortedPointers = temp_sortPointer;


                //Get triangle indeces
                int[] thisIndex = new int[3];
                thisIndex[0] = point_ptr[index * 3 + 0];
                thisIndex[1] = point_ptr[index * 3 + 1];
                thisIndex[2] = point_ptr[index * 3 + 2];

                //next, check how many times each point in this triangle is used through the mesh:
                int[] usedDup = new int[3];
                int[] location = new int[3];
                for (int pt = 0; pt < point_ptr.Length; pt++)
                {
                    if (thisIndex[0] == point_ptr[pt]) { location[0] = pt; usedDup[0]++; }
                    if (thisIndex[1] == point_ptr[pt]) { location[1] = pt; usedDup[1]++; }
                    if (thisIndex[2] == point_ptr[pt]) { location[2] = pt; usedDup[2]++; }
                } // now we should know if the point can be completely removed or not, but first,
                  // we can remove those pesky pointers from the pointer array now:
                int[] temp_pointers = new int[point_ptr.Length - 3];
                for (int ptx = 0; ptx < index * 3; ptx++)
                { temp_pointers[ptx] = point_ptr[ptx]; } //copy before
                for (int ptx = index * 3; ptx < temp_pointers.Length; ptx++)
                { temp_pointers[ptx] = point_ptr[ptx - 3]; } //copy afterd
                point_ptr = temp_pointers; //replace old pointer array!
                

                //how many points can be removed from the points cache?
                int removeCount = 0;
                if (usedDup[0] == 1) { removeCount++; }
                if (usedDup[1] == 1) { removeCount++; }
                if (usedDup[2] == 1) { removeCount++; }

                //create temp cache:
                Vector3[] temp_pointCache = new Vector3[points_cache.Length - removeCount];
                removeCount = 0; //reset and use as a counter
                //now we must surgically remove any points within the cache we no longer need.
                for (int tpc = 0; tpc < points_cache.Length; tpc++)
                {
                    if (location[0] == tpc && usedDup[0] == 1) { removeCount++; }
                    else if (location[1] == tpc && usedDup[1] == 1) { removeCount++; }
                    else if (location[2] == tpc && usedDup[2] == 1) { removeCount++; }
                    else
                    {
                        temp_pointCache[tpc - removeCount] = points_cache[tpc];
                    }
                }
                points_cache = temp_pointCache; //all done, replace array!
            }
            //Triangle doesn't exist!!!
            else { throw new IndexOutOfRangeException("Referenced triangle does not exist in the mesh. Use TriangleCount to check size of mesh."); }
        }

        public static PartialTriangleMesh AddPartialTriangle(PartialTriangleMesh mesh, Triangle3D tri)
        {
            //first copy old pointers
            int triangleCount = mesh.show_hide.Length;
            int[] temp_point_ptr = new int[mesh.point_ptr.Length + 3];
            for (int ix = 0; ix < mesh.point_ptr.Length; ix++) { temp_point_ptr[ix] = mesh.point_ptr[ix]; }
            bool[] temp_showhide = new bool[mesh.show_hide.Length + 1];
            for (int ix = 0; ix < mesh.show_hide.Length; ix++) { temp_showhide[ix] = mesh.show_hide[ix]; }
            temp_showhide[mesh.show_hide.Length] = true;
            mesh.show_hide = temp_showhide;


            //then check if any of the new points already exist in points_cache
            bool[] fp = new bool[3]; fp[0] = false; fp[1] = false; fp[2] = false;
            int new_addCache = 0;
            for (int pc = 0; pc < mesh.points_cache.Length; pc++)
            {
                if (tri.Points[0] == mesh.points_cache[pc]) { temp_point_ptr[mesh.point_ptr.Length + 0] = pc; fp[0] = true; }
                if (tri.Points[1] == mesh.points_cache[pc]) { temp_point_ptr[mesh.point_ptr.Length + 1] = pc; fp[1] = true; }
                if (tri.Points[2] == mesh.points_cache[pc]) { temp_point_ptr[mesh.point_ptr.Length + 2] = pc; fp[2] = true; }
            }
            //the check total of how many new points are needed
            //add pointers to end of temp pointer array.
            new_addCache = 0;
            if (!fp[0]) { temp_point_ptr[mesh.point_ptr.Length + 0] = new_addCache + mesh.points_cache.Length; new_addCache++; }

            if (!fp[1]) { temp_point_ptr[mesh.point_ptr.Length + 1] = new_addCache + mesh.points_cache.Length; new_addCache++; }

            if (!fp[2]) { temp_point_ptr[mesh.point_ptr.Length + 2] = new_addCache + mesh.points_cache.Length; new_addCache++; }
            mesh.point_ptr = temp_point_ptr; //we are now done with this array and can discard
                                        //the old pointer array for the new one.


            //make temp cache array and copy old array:
            Vector3[] temp_points_cache = new Vector3[mesh.points_cache.Length + new_addCache];
            for (int opc = 0; opc < mesh.points_cache.Length; opc++)
            { temp_points_cache[opc] = mesh.points_cache[opc]; } //old vals copied.
            new_addCache = 0; //reset repurpose as indexer for new vals;
            if (!fp[0]) { temp_points_cache[mesh.points_cache.Length + new_addCache] = tri.Points[0]; new_addCache++; }
            if (!fp[1]) { temp_points_cache[mesh.points_cache.Length + new_addCache] = tri.Points[1]; new_addCache++; }
            if (!fp[2]) { temp_points_cache[mesh.points_cache.Length + new_addCache] = tri.Points[2]; }
            mesh.points_cache = temp_points_cache; //all complete!!!

            return mesh;
        }

        public static PartialTriangleMesh RemovePartialTriangle(PartialTriangleMesh mesh, int index)
        {
            //check if triangle even exists:
            if (index < mesh.show_hide.Length)
            {
                int TriCount = mesh.show_hide.Length; //ORIGINAL (not - 1)
                //remove sort pointer
                bool[] temp_showhide = new bool[mesh.show_hide.Length - 1];
                for (int ix = 0; ix < index; ix++) { temp_showhide[ix] = mesh.show_hide[ix]; }
                for (int ix = index + 1; ix < TriCount - 1; ix++) { temp_showhide[ix - 1] = mesh.show_hide[ix]; }
                mesh.show_hide = temp_showhide;


                //Get triangle indeces
                int[] thisIndex = new int[3];
                thisIndex[0] = mesh.point_ptr[index * 3 + 0];
                thisIndex[1] = mesh.point_ptr[index * 3 + 1];
                thisIndex[2] = mesh.point_ptr[index * 3 + 2];

                //next, check how many times each point in this triangle is used through the mesh:
                int[] usedDup = new int[3];
                int[] location = new int[3];
                for (int pt = 0; pt < mesh.point_ptr.Length; pt++)
                {
                    if (thisIndex[0] == mesh.point_ptr[pt]) { location[0] = pt; usedDup[0]++; }
                    if (thisIndex[1] == mesh.point_ptr[pt]) { location[1] = pt; usedDup[1]++; }
                    if (thisIndex[2] == mesh.point_ptr[pt]) { location[2] = pt; usedDup[2]++; }
                } // now we should know if the point can be completely removed or not, but first,
                  // we can remove those pesky pointers from the pointer array now:
                int[] temp_pointers = new int[mesh.point_ptr.Length - 3];
                for (int ptx = 0; ptx < index * 3; ptx++)
                { temp_pointers[ptx] = mesh.point_ptr[ptx]; } //copy before
                for (int ptx = index * 3; ptx < temp_pointers.Length; ptx++)
                { temp_pointers[ptx] = mesh.point_ptr[ptx - 3]; } //copy afterd
                mesh.point_ptr = temp_pointers; //replace old pointer array!


                //how many points can be removed from the points cache?
                int removeCount = 0;
                if (usedDup[0] == 1) { removeCount++; }
                if (usedDup[1] == 1) { removeCount++; }
                if (usedDup[2] == 1) { removeCount++; }

                //create temp cache:
                Vector3[] temp_pointCache = new Vector3[mesh.points_cache.Length - removeCount];
                removeCount = 0; //reset and use as a counter
                //now we must surgically remove any points within the cache we no longer need.
                for (int tpc = 0; tpc < mesh.points_cache.Length; tpc++)
                {
                    if (location[0] == tpc && usedDup[0] == 1) { removeCount++; }
                    else if (location[1] == tpc && usedDup[1] == 1) { removeCount++; }
                    else if (location[2] == tpc && usedDup[2] == 1) { removeCount++; }
                    else
                    {
                        temp_pointCache[tpc - removeCount] = mesh.points_cache[tpc];
                    }
                }
                mesh.points_cache = temp_pointCache; //all done, replace array!
            }
            //Triangle doesn't exist!!!
            else { throw new IndexOutOfRangeException("Referenced triangle does not exist in the mesh. Use TriangleCount to check size of mesh."); }
            return mesh;
        }


        protected void RemoveAllCache()
        {

        }

        public void DirectSetPointCache(Vector3[] points)
        {
            points_cache = points;
        }

        public Triangle3D FirstHitTriangle(Vector3 position, Vector3 incoming, out Vector3 hit, out int index)
        {
            bool[] didHit = new bool[TriangleCount];
            Vector3[] hitstack = new Vector3[TriangleCount];
            int indexClosest = -1;
            double distClosest = -1;
            Triangle3D closest = null;

            for(int trisc = 0; trisc < TriangleCount; trisc++)
            {
                Triangle3D cont = GetTriangle(trisc);
                if(cont.RayDoesIntersect(position, incoming))
                {
                    didHit[trisc] = true;
                    hitstack[trisc] = cont.Hit(position, incoming);
                    double r = Vector3.Distance(hitstack[trisc], position);
                    if (r < distClosest) { indexClosest = trisc;
                        distClosest = r; closest = cont;
                    }
                }
                else { didHit[trisc] = false; }
            }

            if(indexClosest == -1)
            {
                hit = Vector3.INFINITY;
                index = -1;
                return closest;
            }

            hit = hitstack[indexClosest];
            index = indexClosest;
            return closest;
        }

        public Triangle3D[] AllHitTriangles(Vector3 position, Vector3 incoming, bool sort, out Vector3[] hit, out int[] index, out double[] distances)
        {
            int hitcount = 0;
            Triangle3D[]  tristack    = new Triangle3D[TriangleCount];
            int[]         indexStack  = new int[TriangleCount];
            Vector3[]     hitstack    = new Vector3[TriangleCount];
            double[]      diststack   = new double[TriangleCount];

            //find and plop all data into arrays
            for (int trisc = 0; trisc < TriangleCount; trisc++)
            {
                Triangle3D cont = GetTriangle(trisc);
                if (cont.RayDoesIntersect(position, incoming))
                {
                    tristack   [hitcount] = cont;
                    indexStack [hitcount] = trisc;
                    hitstack   [hitcount] = cont.Hit(position, incoming);
                    diststack  [hitcount] = Vector3.Distance(hitstack[hitcount], position);
                    hitcount++;
                }
            }

            //now we gotta both truncate and also organize arrays
            Triangle3D[]  trunc_tristack    = new Triangle3D  [hitcount];
            int[]         trunc_indexStack  = new int         [hitcount];
            Vector3[]     trunc_hitstack    = new Vector3     [hitcount];
            double[]      trunc_diststack   = new double      [hitcount];
            for(int ht = 0; ht < hitcount; ht++)
            {
                trunc_tristack    [ht] = tristack   [ht];
                trunc_indexStack  [ht] = indexStack [ht];
                trunc_hitstack    [ht] = hitstack   [ht];
                trunc_diststack   [ht] = diststack  [ht];
            }

            //sort if desired
            if (sort)
            {
                Triangle3D[]  sort_tristack    = new Triangle3D  [hitcount];
                int[]         sort_indexStack  = new int         [hitcount];
                Vector3[]     sort_hitstack    = new Vector3     [hitcount];
                double[]      sort_diststack   = new double      [hitcount];

                int[] places_dir = new int[hitcount];
                int errors;
                while(true)
                {
                    errors = 0;
                    for (int p0 = 0; p0 < hitcount - 1; p0++)
                    {
                        if (trunc_diststack[p0] > trunc_diststack[p0 + 1])
                        {
                            double tmp = trunc_diststack[p0 + 1];
                            trunc_diststack[p0 + 1] = trunc_diststack[p0];
                            trunc_diststack[p0] = tmp;
                            places_dir[p0] = p0 + 1; places_dir[p0 + 1] = p0;
                            errors++;
                        }
                    }
                    if(errors == 0) { break; }
                }

                for (int p0 = 0; p0 < hitcount - 1; p0++)
                {
                    sort_tristack   [p0] = trunc_tristack   [places_dir[p0]];
                    sort_indexStack [p0] = trunc_indexStack [places_dir[p0]]; 
                    sort_hitstack   [p0] = trunc_hitstack   [places_dir[p0]]; 
                    sort_diststack  [p0] = trunc_diststack  [places_dir[p0]];
                }

                hit         = sort_hitstack;
                index       = sort_indexStack;
                distances   = sort_diststack;
                return    sort_tristack;
            }
            else
            {
                hit       = trunc_hitstack;
                index     = trunc_indexStack;
                distances = trunc_diststack;
                return trunc_tristack;
            }
        }

        public bool DoesCollide(Vector3 initPos, Vector3 rayCast, double zlimiter = 0.0005)
        {
            for (int tri = 0; tri < TriangleCount; tri++)
            {
                Triangle3D c = GetTriangle(tri);
                if (c.RayDoesIntersect(initPos, rayCast))
                {
                    return true;
                }
            }
            return false;
        }

        public static Triangle3DMesh GridAutoMesh(int x, int y, Vector3[] quickPoints)
        {
            Triangle3DMesh new_mesh = new Triangle3DMesh();

            if(quickPoints.Length != x * y) { throw new Exception("You must enter x * y points into the array"); }

            for(int yc = 0; yc < y - 1; yc++)
            {
                for (int xc = 0; xc < x - 1; xc++)
                {
                    int index = xc + yc * x;
                    Triangle3D left = new Triangle3D
                        (quickPoints[index], quickPoints[index + 1], quickPoints[index + x]);
                    Triangle3D right = new Triangle3D
                        (quickPoints[index + 1], quickPoints[index + x + 1], quickPoints[index + x]);

                    new_mesh.AddTriangle(left);
                    new_mesh.AddTriangle(right);
                }
            }

            return new_mesh;
        }

        public Triangle3D[] AllTriangles(bool usePointer = false)
        {
            Triangle3D[] op = new Triangle3D[TriangleCount];
            if (usePointer)
            {
                for (int l = 0; l < TriangleCount; l++)
                {
                    op[l] = GetTriangle(sortedPointers[l]);
                }
            }
            else
            {
                for (int l = 0; l < TriangleCount; l++)
                {
                    op[l] = GetTriangle(l);
                }
            }
            return op;
        }

        private bool IsSharedPoint(double dist, int ind)
        {
            if (distancesStored[ind].X == dist
                || distancesStored[ind].Y == dist
                || distancesStored[ind].Z == dist)
            { return true; }
            else return false;
        }

        private bool IsAnyCloser(int tri1, int tri2)
        {
            //Tests if any point in triangle 1 is closer than any point in triangle 2.
            //If there is a closer point, tests if that point is shared in triangle 2.
            //If a point is closer and not shared then the first triangle should be considered closer (returns true).

            if (   distancesStored[tri1].X < distancesStored[tri2].X
                || distancesStored[tri1].X < distancesStored[tri2].Y
                || distancesStored[tri1].X < distancesStored[tri2].Z)
            {
                if(!IsSharedPoint(distancesStored[tri1].X, tri2)) { return true; }
            }
            if (   distancesStored[tri1].Y < distancesStored[tri2].X
                || distancesStored[tri1].Y < distancesStored[tri2].Y
                || distancesStored[tri1].Y < distancesStored[tri2].Z)
            {
                if (!IsSharedPoint(distancesStored[tri1].Y, tri2)) { return true; }
            }
            if (   distancesStored[tri1].Z < distancesStored[tri2].X
                || distancesStored[tri1].Z < distancesStored[tri2].Y
                || distancesStored[tri1].Z < distancesStored[tri2].Z)
            {
                if (!IsSharedPoint(distancesStored[tri1].Z, tri2)) { return true; }
            }

            return false;
        }

        private Vector3[] distancesStored;
        public void StackDrawOrder(Vector3 position)
        {
            //collect pointers in original order
            int[] temp_sortPointer = new int[sortedPointers.Length];
            for (int ix = 0; ix < TriangleCount; ix++) { temp_sortPointer[ix] = sortedPointers[ix]; }
            
            //Store distances of points to camera as Vec3 in array
            distancesStored = new Vector3[TriangleCount];

            //Collect triangles in original order
            Triangle3D[] sort_tristack = new Triangle3D[TriangleCount];
            for (int v = 0; v < TriangleCount; v++)
            { 
                sort_tristack[v] = GetTriangle(v);
                Vector3 d = new Vector3
                    (
                        Vector3.Distance(sort_tristack[v].Points[0], position),
                        Vector3.Distance(sort_tristack[v].Points[1], position),
                        Vector3.Distance(sort_tristack[v].Points[2], position)
                    );
                distancesStored[v] = d;
            }

            int errors;
            while (true)
            {
                errors = 0;
                for (int p0 = 0; p0 < TriangleCount - 1; p0++)
                {
                    //Check if any point in triangle at p0 is closer than any point and the closer point is not shared, swap.
                    if (IsAnyCloser(p0 + 1, p0 + 0))
                    {
                        //swap triangle ref in:
                        //  distancesStored,
                        //  temp_sortPointer
                        //

                        Vector3 tempTempVec = distancesStored[p0];
                        distancesStored[p0] = distancesStored[p0 + 1];
                        distancesStored[p0 + 1] = tempTempVec;

                        int tempTemp_sort = temp_sortPointer[p0];
                        temp_sortPointer[p0] = temp_sortPointer[p0 + 1];
                        temp_sortPointer[p0 + 1] = tempTemp_sort;

                        errors++;
                    }
                }
                if (errors == 0) { break; }
            }

            sortedPointers = temp_sortPointer;
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
    public class PartialTriangleMesh
    {
        public int[] point_ptr = new int[0];
        public bool[] show_hide = new bool[0];
        public Vector3[] points_cache = new Vector3[0];

        public static PartialTriangleMesh AddPartialTriangle(PartialTriangleMesh mesh, Triangle3D tri)
        {
            //first copy old pointers
            int quadCount = mesh.show_hide.Length;
            int[] temp_point_ptr = new int[mesh.point_ptr.Length + 3];
            for (int ix = 0; ix < mesh.point_ptr.Length; ix++) { temp_point_ptr[ix] = mesh.point_ptr[ix]; }
            bool[] temp_showhide = new bool[mesh.show_hide.Length + 1];
            for (int ix = 0; ix < mesh.show_hide.Length; ix++) { temp_showhide[ix] = mesh.show_hide[ix]; }
            temp_showhide[mesh.show_hide.Length] = true;
            mesh.show_hide = temp_showhide;


            //then check if any of the new points already exist in points_cache
            bool[] fp = new bool[3]; fp[0] = false; fp[1] = false; fp[2] = false;
            int new_addCache = 0;
            for (int pc = 0; pc < mesh.points_cache.Length; pc++)
            {
                if (tri.Points[0] == mesh.points_cache[pc]) { temp_point_ptr[mesh.point_ptr.Length + 0] = pc; fp[0] = true; }
                if (tri.Points[1] == mesh.points_cache[pc]) { temp_point_ptr[mesh.point_ptr.Length + 1] = pc; fp[1] = true; }
                if (tri.Points[2] == mesh.points_cache[pc]) { temp_point_ptr[mesh.point_ptr.Length + 2] = pc; fp[2] = true; }
            }
            //the check total of how many new points are needed
            //add pointers to end of temp pointer array.
            new_addCache = 0;
            if (!fp[0]) { temp_point_ptr[mesh.point_ptr.Length + 0] = new_addCache + mesh.points_cache.Length; new_addCache++; }

            if (!fp[1]) { temp_point_ptr[mesh.point_ptr.Length + 1] = new_addCache + mesh.points_cache.Length; new_addCache++; }

            if (!fp[2]) { temp_point_ptr[mesh.point_ptr.Length + 2] = new_addCache + mesh.points_cache.Length; new_addCache++; }
            mesh.point_ptr = temp_point_ptr; //we are now done with this array and can discard
                                             //the old pointer array for the new one.


            //make temp cache array and copy old array:
            Vector3[] temp_points_cache = new Vector3[mesh.points_cache.Length + new_addCache];
            for (int opc = 0; opc < mesh.points_cache.Length; opc++)
            { temp_points_cache[opc] = mesh.points_cache[opc]; } //old vals copied.
            new_addCache = 0; //reset repurpose as indexer for new vals;
            if (!fp[0]) { temp_points_cache[mesh.points_cache.Length + new_addCache] = tri.Points[0]; new_addCache++; }
            if (!fp[1]) { temp_points_cache[mesh.points_cache.Length + new_addCache] = tri.Points[1]; new_addCache++; }
            if (!fp[2]) { temp_points_cache[mesh.points_cache.Length + new_addCache] = tri.Points[2]; }
            mesh.points_cache = temp_points_cache; //all complete!!!

            return mesh;
        }

        public static PartialTriangleMesh RemovePartialTriangle(PartialTriangleMesh mesh, int index)
        {
            //check if triangle even exists:
            if (index < mesh.show_hide.Length)
            {
                int TriCount = mesh.show_hide.Length; //ORIGINAL (not - 1)
                //remove sort pointer
                bool[] temp_showhide = new bool[mesh.show_hide.Length - 1];
                for (int ix = 0; ix < index; ix++) { temp_showhide[ix] = mesh.show_hide[ix]; }
                for (int ix = index + 1; ix < TriCount - 1; ix++) { temp_showhide[ix - 1] = mesh.show_hide[ix]; }
                mesh.show_hide = temp_showhide;


                //Get triangle indeces
                int[] thisIndex = new int[3];
                thisIndex[0] = mesh.point_ptr[index * 3 + 0];
                thisIndex[1] = mesh.point_ptr[index * 3 + 1];
                thisIndex[2] = mesh.point_ptr[index * 3 + 2];

                //next, check how many times each point in this triangle is used through the mesh:
                int[] usedDup = new int[3];
                int[] location = new int[3];
                for (int pt = 0; pt < mesh.point_ptr.Length; pt++)
                {
                    if (thisIndex[0] == mesh.point_ptr[pt]) { location[0] = pt; usedDup[0]++; }
                    if (thisIndex[1] == mesh.point_ptr[pt]) { location[1] = pt; usedDup[1]++; }
                    if (thisIndex[2] == mesh.point_ptr[pt]) { location[2] = pt; usedDup[2]++; }
                } // now we should know if the point can be completely removed or not, but first,
                  // we can remove those pesky pointers from the pointer array now:
                int[] temp_pointers = new int[mesh.point_ptr.Length - 3];
                for (int ptx = 0; ptx < index * 3; ptx++)
                { temp_pointers[ptx] = mesh.point_ptr[ptx]; } //copy before
                for (int ptx = index * 3; ptx < temp_pointers.Length; ptx++)
                { temp_pointers[ptx] = mesh.point_ptr[ptx - 3]; } //copy afterd
                mesh.point_ptr = temp_pointers; //replace old pointer array!


                //how many points can be removed from the points cache?
                int removeCount = 0;
                if (usedDup[0] == 1) { removeCount++; }
                if (usedDup[1] == 1) { removeCount++; }
                if (usedDup[2] == 1) { removeCount++; }

                //create temp cache:
                Vector3[] temp_pointCache = new Vector3[mesh.points_cache.Length - removeCount];
                removeCount = 0; //reset and use as a counter
                //now we must surgically remove any points within the cache we no longer need.
                for (int tpc = 0; tpc < mesh.points_cache.Length; tpc++)
                {
                    if (location[0] == tpc && usedDup[0] == 1) { removeCount++; }
                    else if (location[1] == tpc && usedDup[1] == 1) { removeCount++; }
                    else if (location[2] == tpc && usedDup[2] == 1) { removeCount++; }
                    else
                    {
                        temp_pointCache[tpc - removeCount] = mesh.points_cache[tpc];
                    }
                }
                mesh.points_cache = temp_pointCache; //all done, replace array!
            }
            //Triangle doesn't exist!!!
            else { throw new IndexOutOfRangeException("Referenced triangle does not exist in the mesh. Use TriangleCount to check size of mesh."); }
            return mesh;
        }
    }

}
