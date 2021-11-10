using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirali.MathR
{
    public class RayPath
    {
        private Vector3 source;
        private uint rayID;

        public Vector3 Source { get { return source; } }

        public Vector3 Direction;
        public Vector3 Position;
        public double Magnitude;

        public uint RID { get { return RID; } }

        /// <summary>
        /// <tooltip>Create a new empty RayPath.</tooltip>
        /// </summary>
        public RayPath()
        {
            Direction = new Vector3(1, 0, 0, Vector3.VectorForm.DIRECTION);
            source = Position = Vector3.Zero();
            Magnitude = 0;
        }

        public RayPath(Vector3 direction)
        {
            Direction = new Vector3(1, 0, 0, Vector3.VectorForm.DIRECTION);
            Direction.Set(direction);
            source = Position = Vector3.Zero();
            Magnitude = 0;
        }

        public RayPath(Vector3 position, Vector3 direction)
        {
            Direction = new Vector3(1, 0, 0, Vector3.VectorForm.DIRECTION);
            Direction.Set(direction);
            Position = new Vector3(position);
            Magnitude = 0;
        }

        public RayPath(Vector3 position, Vector3 direction, double magnitude)
        {
            Direction = new Vector3(1, 0, 0, Vector3.VectorForm.DIRECTION);
            Direction.Set(direction);
            source = Position = new Vector3(position);
            Magnitude = magnitude;
        }


        /// <summary>
        /// <tooltip>Returns the distance from the RayPath position to given point.</tooltip>
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double Distance(Vector3 point)
        {
            return (Position - point).Length();
        }

        /// <summary>
        /// <tooltip>Evolution action increments RayPath based on Magnitude value.</tooltip>
        /// </summary>
        /// <returns></returns>
        public virtual RayPath Step()
        {
            Position.Add(Direction * Magnitude);
            return this;
        }

        /// <summary>
        /// <tooltip>Evolution action increments RayPath based on stepSize parameter.</tooltip>
        /// </summary>
        /// <param name="stepSize">Distance for which to evaluate new position in RayPath.</param>
        /// <returns></returns>
        public virtual RayPath Step(double stepSize)
        {
            Position.Add(Direction * stepSize);
            return this;
        }
        

        public virtual RayPath March(double minimum, double maximum)
        {
            return this;
        }
    }
}
