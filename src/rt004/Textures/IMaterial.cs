using OpenTK.Mathematics;

namespace rt004.Textures
{
    public interface IMaterial
    {
        public Vector3d Colour(Vector2d uv);
        public Vector3d Color { get; }
        public double DiffusionCoefficient { get; }
        public double SpecularCoefficient { get; }
        public double Glossiness { get; }
        public double Transparency { get; }
        public double RefractiveIndex { get; }
    }

}
