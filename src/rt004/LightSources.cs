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
        Vector3d DirectionToLight(Vector3d point);
    }

    public class DirectionalLightSource : ILightSource
    {
        public Vector3d Direction { get; }
        public double Intensity { get; }
        public Vector3d Color { get; }
        public DirectionalLightSource(Vector3d direction, Vector3d color, double intensity)
        {
            Direction = direction.Normalized();
            Color = color;
            Intensity = intensity;
        }
        public Vector3d DirectionToLight(Vector3d point)
        {
            return -Direction;
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
        public Vector3d DirectionToLight(Vector3d point)
        {
            return Vector3d.Subtract(Origin, point);
        }
    }
    public class SphericalLightSource : ILightSource
    {
        public Vector3d Origin { get; }
        public double Intensity { get; }
        public Vector3d Color { get; }
        public double Radius { get; }
        private Random _rnd;
        public SphericalLightSource(Vector3d origin, Vector3d color, double intensity, double radius)
        {
            Origin = origin;
            Color = color;
            Intensity = intensity;
            _rnd = new Random();
            Radius = radius;
        }
        public Vector3d DirectionToLight(Vector3d point)
        {
            Vector3d randomOrigin = new(Origin.X + ((_rnd.NextDouble() - 0.5) * Radius / 2), Origin.Y + ((_rnd.NextDouble() - 0.5) * Radius / 2), Origin.Z + ((_rnd.NextDouble() - 0.5) * Radius / 2));
                
            return Vector3d.Subtract(randomOrigin, point).Normalized();
        }
    }

}
