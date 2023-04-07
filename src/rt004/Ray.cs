using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    public struct Ray
    {
        public Vector3d Origin { get; set; }
        public Vector3d Direction { get; set; }
        public Ray(Vector3d origin, Vector3d direction)
        {
            Origin = origin;
            Direction = direction.Normalized();
        }
    }
}
