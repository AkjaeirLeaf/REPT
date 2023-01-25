using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.MathR;
using Kirali.Light;

namespace Kirali.Environment.Render.Primatives
{
    public class Triangle3D
    {
        private Vector3[] points = new Vector3[3];
        private Vector3[] pointnormals = new Vector3[3];
        public Vector3[] Points { get { return points; } }
        public Vector3[] Point_Normals { get { return pointnormals; } set { pointnormals = value; } }

        private Vector3 normal;
        public Vector3 Normal
        {
            get
            {
                if(normal != null)
                {
                    if(normal.Form == Vector3.VectorForm.NORMAL)
                    {
                        return normal;
                    }
                    else
                    {
                        Vector3 v1 = new Vector3(Points[1] - Points[0]);
                        Vector3 v2 = new Vector3(Points[2] - Points[0]);

                        return Vector3.Cross(v1, v2).Normalize();
                    }
                }
                else
                {
                    Vector3 v1 = new Vector3(Points[1] - Points[0]);
                    Vector3 v2 = new Vector3(Points[2] - Points[0]);

                    return Vector3.Cross(v1, v2).Normalize();
                }
            }
        }
        public Vector3 Middle
        {
            get
            {
                return new Vector3((points[0].X + points[1].X + points[2].X) / 3,
                    (points[0].Y + points[1].Y + points[2].Y) / 3,
                    (points[0].Z + points[1].Z + points[2].Z) / 3);
            }
        }
        private int[] tex_link = new int[3];
        public Vector2[] UV_Link = new Vector2[3];
        public int[] TextureLink { get { return tex_link; } set { tex_link = value; } }

        //CONSTRUCTION
        public Triangle3D()
        {
            points[0] = new Vector3( 0,  1, 0);
            points[1] = new Vector3(-1, -1, 0);
            points[2] = new Vector3( 1, -1, 0);
            RecalculateNormal();
        }
        public Triangle3D(Vector3[] pointList)
        {
            points[0] = pointList[0];
            points[1] = pointList[1];
            points[2] = pointList[2];

            RecalculateNormal();
        }
        public Triangle3D(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            points[0] = p0;
            points[1] = p1;
            points[2] = p2;

            RecalculateNormal();
        }
        private void RecalculateNormal()
        {
            Vector3 v1 = new Vector3(Points[1] - Points[0]);
            Vector3 v2 = new Vector3(Points[2] - Points[0]);

            normal = Vector3.Cross(v1, v2).Normalize();
            normal.Form = Vector3.VectorForm.NORMAL;
        }
        public void SetPoints(Vector3[] pointList)
        {
            points[0] = pointList[0];
            points[1] = pointList[1];
            points[2] = pointList[2];

            RecalculateNormal();
        }
        public double Area()
        {
            Vector3 v1 = new Vector3(Points[1] - Points[0]);
            Vector3 v2 = new Vector3(Points[2] - Points[0]);

            normal = Vector3.Cross(v1, v2);
            return normal.Length();
        }

        public bool RayDoesIntersect(Vector3 initpos, Vector3 incoming)
        {
            Vector3 N = new Vector3(normal);
            //Test if parallel to triangle surface
            if(Math.Abs(Vector3.Dot(N, incoming)) == 0)
            {
                return false;
            }
            double D = -Vector3.Dot(N, points[0]); 
            double t = -(Vector3.Dot(N, initpos) + D) / Vector3.Dot(N, incoming);
            if (t < 0) 
                return false;
            
            Vector3 P = initpos + incoming * t;
            Vector3 C;

            Vector3 e0 = points[1] - points[0];
            Vector3 c0 = P - points[0];
            C = Vector3.Cross(e0, c0);
            if(Vector3.Dot(N, C) < 0)
            { return false; }

            Vector3 e1 = points[2] - points[1];
            Vector3 c1 = P - points[1];
            C = Vector3.Cross(e1, c1);
            if (Vector3.Dot(N, C) < 0)
            { return false; }

            Vector3 e2 = points[0] - points[2];
            Vector3 c2 = P - points[2];
            C = Vector3.Cross(e2, c2);
            if (Vector3.Dot(N, C) < 0)
            { return false; }

            return true;
        }
        public Vector3 Hit(Vector3 initpos, Vector3 incoming)
        {
            Vector3 hitPointInf = new Vector3(0.0, 0.0, 0.0, Vector3.VectorForm.INFINITY); //Presume the ray never hits.
            Vector3 raydir = new Vector3(incoming).Normalize();
            double EPSILON = 0.0000001;

            Vector3 v0 = points[0];
            Vector3 v1 = points[1];
            Vector3 v2 = points[2];

            Vector3 edge1 = v1 - v0;
            Vector3 edge2 = v2 - v0;

            Vector3 h = Vector3.Cross(raydir, edge2);
            double a = Vector3.Dot(edge1, h);
            if(a > - EPSILON && a < EPSILON) { return hitPointInf; }

            double f = 1.0 / a;
            Vector3 s = initpos - v0;
            double u = f * Vector3.Dot(s, h);

            if (u < 0.0 || u > 1.0) { return hitPointInf; }
            Vector3 q = Vector3.Cross(s, edge1);
            double v = f * Vector3.Dot(incoming, q);
            if(v < 0.0 || u + v > 1.0) { return hitPointInf; }

            double t = f * Vector3.Dot(edge2, q);
            if(t > EPSILON)
            {
                Vector3 hit = initpos + raydir * t;
                hit.Form = Vector3.VectorForm.POSITION;
                return hit;
            }
            return hitPointInf;
        }

        public void Transform(Vector3 transform)
        {
            points[0] += transform;
            points[1] += transform;
            points[2] += transform;
        }

        public void Scale(double factor)
        {
            points[0] *= factor;
            points[1] *= factor;
            points[2] *= factor;
        }
        public KColor4 GetDiffuseColor(Vector3 point, string colorMode = "")
        {
            KColor4 diffuse = new KColor4(1.0, 1.0, 1.0);

            switch (colorMode)
            {
                case "white":
                    return new KColor4(1.0, 1.0, 1.0);
                case "colorwheel":
                    double d0 = ((points[0] - point).Length()) / (0.5 * ((points[2] - points[0]).Length() + (points[1] - points[0]).Length()));
                    double d1 = ((points[1] - point).Length()) / (0.5 * ((points[0] - points[1]).Length() + (points[2] - points[1]).Length()));
                    double d2 = ((points[2] - point).Length()) / (0.5 * ((points[1] - points[2]).Length() + (points[0] - points[2]).Length()));
                    return new KColor4(d0, d1, d2) * 2;
                case "checkers":
                    double sinchx = Math.Cos(point.X * 0.5 * Math.PI);
                    double sinchy = Math.Cos(point.Y * 0.5 * Math.PI);

                    double checkers = 0.5 + Math.Ceiling(sinchx * sinchy);
                    return new KColor4(checkers, checkers, checkers);
                default:
                    return new KColor4(1.0, 1.0, 1.0);
            }
        }
    }
}
