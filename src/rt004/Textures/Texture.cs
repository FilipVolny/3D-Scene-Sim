using OpenTK.Mathematics;

namespace rt004.Textures
{
    public class Texture : IMaterial
    {
        public Vector3d Color { get; }
        public Vector3d Colour(Vector2d uv)
        {
            return Color;
        }

        public double DiffusionCoefficient { get; }
        public double SpecularCoefficient { get; }
        public double Glossiness { get; }
        public double Transparency { get; }
        public double RefractiveIndex { get; }

        public Texture(Vector3d color, double diffuseCoeficient, double specularCoeficient, double glossiness, double transparency, double refractiveIndex)
        {
            Color = color;
            DiffusionCoefficient = diffuseCoeficient;
            SpecularCoefficient = specularCoeficient;
            Glossiness = glossiness;
            Transparency = transparency;
            RefractiveIndex = refractiveIndex;
        }
    }

}
