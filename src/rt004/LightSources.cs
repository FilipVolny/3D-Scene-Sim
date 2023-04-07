using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    public interface ILightSource
    {
        double Intensity { get; }
        Vector3d Color { get; }
    }
    public class DirectionLightSource : ILightSource
    {
        public Vector3d Direction { get; }
        public double Intensity { get; }
        public Vector3d Color { get; }
        public DirectionLightSource(Vector3d direction, Vector3d color, double intensity)
        {
            Direction = direction.Normalized();
            Color = color;
            Intensity = intensity;
        }
    }
    public class PointLightSource : ILightSource
    {
        public Vector3d Origin { get; }
        public double Intensity { get; }
        public Vector3d Color { get; }
        public PointLightSource(Vector3d origin, Vector3d color, double intensity)
        {
            Origin = origin;
            Color = color;
            Intensity = intensity;
        }
    }
}
