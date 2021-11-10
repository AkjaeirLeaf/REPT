using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirali.MathR
{
    /// <summary>
    /// <tooltip>Object for compact calculation and double storage. Can convert easily between Vector and Matrix form as well as be used for 3D object transformations.</tooltip>
    /// </summary>
    public class Matrix
    {
        public readonly int sizeX;
        public readonly int sizeY;
        private double[,] content;

        #region MatrixConstruction
        /// <summary>
        /// <tooltip>Creates a Matrix Object with 2D size X, Y.</tooltip>
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public Matrix(int X, int Y)
        {
            content = new double[X, Y];
            sizeX = X;
            sizeY = Y;
        }

        /// <summary>
        /// <tooltip>Creates a Matrix Object with 2D size X, Y from a 1D array of values.</tooltip>
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="value"></param>
        public Matrix(int X, int Y, double[] value)
        {
            content = new double[X, Y];
            sizeX = X;
            sizeY = Y;

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    content[x, y] = value[y * sizeX + x];
                }
            }
        }


        //ROTATION MATRICES:::
        /// <summary>
        /// <tooltip>Constructs a Rotation Matrix for a rotation some angle about the X-axis.</tooltip>
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Matrix RotationX(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            Matrix Xrotation = new Matrix(3, 3);
            Xrotation.Set(new double[9] 
                { 1, 0, 0,
                0, cos, -1 * sin,
                0, sin, cos });
            return Xrotation;
        }

        /// <summary>
        /// <tooltip>Constructs a Rotation Matrix for a rotation some angle about the Y-axis.</tooltip>
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Matrix RotationY(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            Matrix Yrotation = new Matrix(3, 3);
            Yrotation.Set(new double[9] 
                { cos, 0, sin,
                0, 1, 0,
                -sin, 0, cos });
            return Yrotation;
        }

        /// <summary>
        /// <tooltip>Constructs a Rotation Matrix for a rotation some angle about the Z-axis.</tooltip>
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Matrix RotationZ(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            Matrix Zrotation = new Matrix(3, 3);
            Zrotation.Set(new double[9] { cos, -sin, 0,
                sin, cos, 0,
                0, 0, 1 });
            return Zrotation;
        }

        /// <summary>
        /// <tooltip>Constructs a Rotation Matrix for a rotation some angle about the given, normalized U-axis.</tooltip>
        /// </summary>
        /// <param name="U"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Matrix RotationU(Vector3 U, double angle)
        {
            Matrix uRotation = new Matrix(3, 3);
            Vector3 nor = new Vector3(U).Normalize();
            double ux = nor.X;
            double uy = nor.Y;
            double uz = nor.Z;
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);

            double lcos = 1 - cos;

            uRotation.Set(new double[9] {
            cos + (ux * ux * lcos), (ux * uy * lcos) - (uz * sin), (ux * uz * lcos) + (uy * sin),
            (uy * ux * lcos) + (uz * sin), cos + (uy * uy * lcos), (uy * uz * lcos) - (ux * sin),
            (uz * ux * lcos) - (uy * sin), (uz * uy * lcos) + (ux * sin), cos + (uz * uz * lcos)
            });


            return uRotation;
        }

        #endregion MatrixConstruction

        #region MatrixOperations
        /// <summary>
        /// <tooltip>Multiplies two compatable matrix objects.</tooltip>
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
            Matrix res;
            if (matrix1.sizeX == matrix2.sizeY)
            {
                res = new Matrix(matrix2.sizeX, matrix1.sizeY);
                for (int y = 0; y < res.sizeY; y++)
                {
                    for (int x = 0; x < res.sizeX; x++)
                    {
                        double sum = 0;
                        for (int x1 = 0; x1 < matrix1.sizeX; x1++)
                        {
                            sum += (matrix1.Get(x1, y) * matrix2.Get(x, x1));
                        }
                        res.Set(x, y, sum);
                    }
                }
                return res;
            }
            else
            {
                throw new Exception("Attempted invalid multiplication of matrices");
            }
        }

        /// <summary>
        /// <tooltip>Adds the aligning matrix values in a resulting same-size matrix.</tooltip>
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        public static Matrix operator +(Matrix matrix1, Matrix matrix2)
        {
            Matrix res;
            if(matrix1.sizeX == matrix2.sizeX && matrix1.sizeY == matrix2.sizeY)
            {
                res = new Matrix(matrix1.sizeX, matrix1.sizeY);
                for (int y = 0; y < res.sizeY; y++)
                {
                    for (int x = 0; x < res.sizeX; x++)
                    {
                        res.Set(x, y, matrix1.Get(x, y) + matrix2.Get(x, y));
                    }
                }

                return res;
            }
            else
            {
                throw new Exception("Attempted invalid addition of matrices");
            }
        }

        /// <summary>
        /// <tooltip>Returns the difference between the two matrices.</tooltip>
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        public static Matrix operator -(Matrix matrix1, Matrix matrix2)
        {
            Matrix res;
            if (matrix1.sizeX == matrix2.sizeX && matrix1.sizeY == matrix2.sizeY)
            {
                res = new Matrix(matrix1.sizeX, matrix1.sizeY);
                for (int y = 0; y < res.sizeY; y++)
                {
                    for (int x = 0; x < res.sizeX; x++)
                    {
                        res.Set(x, y, matrix1.Get(x, y) - matrix2.Get(x, y));
                    }
                }

                return res;
            }
            else
            {
                throw new Exception("Attempted invalid subtraction of matrices");
            }
        }

        //CONVERSION FORMS
        public static explicit operator Vector3(Matrix p) => p.ToVector3();

        /// <summary>
        /// <tooltip>Converts a 1x3 matrix object to a Vector3 object.</tooltip>
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Vector3 ToVector3()
        {
            if(sizeX == 1 && sizeY == 3)
            {
                Vector3 res = new Vector3();
                res.X = Get(0, 0);
                res.Y = Get(0, 1);
                res.Z = Get(0, 2);
                return res;
            }
            else if(sizeX == 3 && sizeY == 1)
            {
                Vector3 res = new Vector3();
                res.X = Get(0, 0);
                res.Y = Get(1, 0);
                res.Z = Get(2, 0);
                return res;
            }
            else
            {
                throw new Exception("Matrix must be of dimensions 1x3 to convert to a Vector3.");
            }
        }

        /// <summary>
        /// <tooltip>Converts a 1x3 matrix object to a Vector3 object.</tooltip>
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3 ToVector3(Matrix matrix)
        {
            if (matrix.sizeX == 1 && matrix.sizeY == 3)
            {
                Vector3 res = new Vector3();
                res.X = matrix.Get(0, 0);
                res.Y = matrix.Get(0, 1);
                res.Z = matrix.Get(0, 2);
                return res;
            }
            else
            {
                throw new Exception("Matrix must be of dimensions 1x3 to convert to a Vector3.");
            }
        }


        #endregion MatrixOperations

        #region SetAndGet
        /// <summary>
        /// <tooltip>Set the value of the position X, Y in the matrix object.</tooltip>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        public void Set(int x, int y, double value)
        {
            if (x < sizeX && y < sizeY)
            {
                content[x, y] = value;
            }
        }

        /// <summary>
        /// <tooltip>Sets the values in a 2D matrix using a 1D array of values.</tooltip>
        /// </summary>
        /// <param name="value"></param>
        public void Set(double[] value)
        {
            if(sizeX * sizeY == value.Length)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        content[x, y] = value[y * sizeX + x];
                    }
                }
            }
            else
            {
                throw new Exception("Target content is not the same size as input.");
            }
        }

        /// <summary>
        /// <tooltip>Get the value of the position X, Y in the matrix object.</tooltip>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public double Get(int x, int y)
        {
            if (x < sizeX && y < sizeY)
            {
                return content[x, y];
            }
            else { return 0; }
        }

        /// <summary>
        /// <tooltip>Flips the matrix across the diagonal.</tooltip>
        /// </summary>
        /// <returns></returns>
        public Matrix Flip()
        {
            Matrix resultant = new Matrix(sizeY, sizeX);
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    resultant.Set(y, x, content[x, y]);
                }
            }
            return resultant;
        }
        #endregion SetAndGet

    }
}
