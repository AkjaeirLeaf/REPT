using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kirali.MathR;

namespace Kirali.Environment.Render.Primatives
{
    public class Quad3D
    {
        // Assign topleft, topright, bottomright, bottomleft
        private Vector3[] points = new Vector3[4];
        private Vector3[] pointNormals = new Vector3[4];
        public Vector3[] Points { get { return points; } }
        public Vector3[] PointNormals { get { return pointNormals; } }

        private int linkageTex = -1;
        public int TextureLink { get { return linkageTex; } set { linkageTex = value; } }

        public Triangle3D Left
        {
            get { return new Triangle3D(points[0], points[1], points[2]); }
        }

        public Triangle3D Right
        {
            get { return new Triangle3D(points[2], points[3], points[0]); }
        }

        private Vector3 normal;
        public Vector3 Normal
        {
            get
            { return 0.5 * (Left.Normal + Right.Normal); }
        }

        public Vector3 Middle
        {
            get
            {
                return new Vector3((points[0].X + points[1].X + points[2].X + points[3].X) / 4,
                                   (points[0].Y + points[1].Y + points[2].Y + points[3].Y) / 4,
                                   (points[0].Z + points[1].Z + points[2].Z + points[3].Z) / 4);
            }
        }

        public Quad3D RotateAbout(Vector3 rotation)
        {
            Vector3 Theta = Vector3.Zaxis;
            Vector3 Phi   = Vector3.RotateU(Vector3.Xaxis, Theta, rotation.X);
            Vector3 R     = Vector3.RotateU(Vector3.RotateU(Vector3.Yaxis, Theta, rotation.X), Phi, rotation.Y);

            points[0] = Vector3.RotateU(Vector3.RotateU(Vector3.RotateU(points[0], Theta, rotation.X), Phi, rotation.Y), R, rotation.Z);
            points[1] = Vector3.RotateU(Vector3.RotateU(Vector3.RotateU(points[1], Theta, rotation.X), Phi, rotation.Y), R, rotation.Z);
            points[2] = Vector3.RotateU(Vector3.RotateU(Vector3.RotateU(points[2], Theta, rotation.X), Phi, rotation.Y), R, rotation.Z);
            points[3] = Vector3.RotateU(Vector3.RotateU(Vector3.RotateU(points[3], Theta, rotation.X), Phi, rotation.Y), R, rotation.Z);

            return this;
        }

        public Quad3D SafeRotateAbout(Vector3 rotation)
        {
            Quad3D duplicate = (Quad3D)MemberwiseClone();

            Vector3 Theta = Vector3.Zaxis;
            Vector3 Phi = Vector3.RotateU(Vector3.Xaxis, Theta, rotation.X);
            Vector3 R = Vector3.RotateU(Vector3.RotateU(Vector3.Yaxis, Theta, rotation.X), Phi, rotation.Y);

            duplicate.points[0] = Vector3.RotateU(Vector3.RotateU(Vector3.RotateU(points[0], Theta, rotation.X), Phi, rotation.Y), R, rotation.Z);
            duplicate.points[1] = Vector3.RotateU(Vector3.RotateU(Vector3.RotateU(points[1], Theta, rotation.X), Phi, rotation.Y), R, rotation.Z);
            duplicate.points[2] = Vector3.RotateU(Vector3.RotateU(Vector3.RotateU(points[2], Theta, rotation.X), Phi, rotation.Y), R, rotation.Z);
            duplicate.points[3] = Vector3.RotateU(Vector3.RotateU(Vector3.RotateU(points[3], Theta, rotation.X), Phi, rotation.Y), R, rotation.Z);

            return duplicate;
        }


        public Quad3D()
        {
            points[0] = new Vector3(-1,  1, 0);
            points[1] = new Vector3( 1,  1, 0);
            points[2] = new Vector3( 1, -1, 0);
            points[3] = new Vector3(-1, -1, 0);
            RecalculateNormal();
        }

        public Quad3D(Vector3[] points_in)
        {
            points[0] = points_in[0];
            points[1] = points_in[1];
            points[2] = points_in[2];
            points[3] = points_in[3];
            RecalculateNormal();
        }

        public Quad3D(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3)
        {
            points[0] = P0;
            points[1] = P1;
            points[2] = P2;
            points[3] = P3;
            RecalculateNormal();
        }

        public void SetPointNormals (Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3)
        {
            pointNormals[0] = P0;
            pointNormals[1] = P1;
            pointNormals[2] = P2;
            pointNormals[3] = P3;
        }

        private void RecalculateNormal()
        {
            Vector3 n = 0.5 * (Left.Normal + Right.Normal);

            normal = n;
            normal.Form = Vector3.VectorForm.NORMAL;
        }

        public double Area()
        {
            return (Left.Area() + Right.Area());
        }
    }
}
