using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    public class LightSource
    {
        public Vector3d Origin { get; set; }
        public Vector3d Direction { get; set; }
        public double Intensity { get; set; }
        public Vector3d Color { get; set; }
        public LightSource(Vector3d origin, Vector3d direction, Vector3d color, double intensity)
        {
            Origin = origin;
            Direction = direction.Normalized();
            Color = color;
            Intensity = intensity;
        }
    }
}
