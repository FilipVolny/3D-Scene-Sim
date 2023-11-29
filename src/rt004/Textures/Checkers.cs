using OpenTK.Mathematics;

namespace rt004.Textures
{
    public class Checkers : IMaterial
    {
        public double modulo; // must be an even number


        public Vector3d Color { get; }
        public Vector3d Colour(Vector2d uv)
        {
            double halfPoint = modulo / 2;

            double u = uv.X;
            if (u < 0) { u = -u + halfPoint; } // used to center the checker texture properly at u=0 or v=0 
            u %= modulo;

            double v = uv.Y;
            if (v < 0) { v = -v + halfPoint; } // used to center the checker texture properly at u=0 or v=0 
            v %= modulo;

            if (u <= halfPoint && v >= halfPoint || u >= halfPoint && v <= halfPoint)
            {
                return new(1, 1, 1); //could possibly return any colour
            }
            return new(0, 0, 0); //could possibly return any colour


        }

        public double DiffusionCoefficient { get; }
        public double SpecularCoefficient { get; }
        public double Glossiness { get; }
        public double Transparency { get; }
        public double RefractiveIndex { get; }
        public Checkers(Vector3d color, double diffuseCoeficient, double specularCoeficient, double glossiness, double transparency, double refractiveIndex, double size)
        {
            Color = color;
            DiffusionCoefficient = diffuseCoeficient;
            SpecularCoefficient = specularCoeficient;
            Glossiness = glossiness;
            Transparency = transparency;
            RefractiveIndex = refractiveIndex;
            modulo = size * 2; //modulo must even
        }
    }

}
