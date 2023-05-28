using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace rt004
{
    public interface IMaterial
    {
        public Vector3d Colour(Vector2d uv);
        public Vector3d Color { get; }
        public double DiffusionCoefficient { get; }
        public double SpecularCoefficient { get; }
        public double Glossiness { get; }
    }

    public class Material : IMaterial
    {
        public Vector3d Color { get; }
        public Vector3d Colour(Vector2d uv)
        {
            return Color;
        }
        public double DiffusionCoefficient { get; }
        public double SpecularCoefficient { get; }
        public double Glossiness { get; }

        public Material(Vector3d color, double diffuseCoeficient, double specularCoeficient, double glossiness)
        {
            Color = color;
            DiffusionCoefficient = diffuseCoeficient;
            SpecularCoefficient = specularCoeficient;
            Glossiness = glossiness;
        }
    }
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

        public Texture(Vector3d color, double diffuseCoeficient, double specularCoeficient, double glossiness)
        {
            Color = color;
            DiffusionCoefficient = diffuseCoeficient;
            SpecularCoefficient = specularCoeficient;
            Glossiness = glossiness;
        }
    }

    public class Chessboard : IMaterial
    {
        public Vector3d Color { get; }
        public Vector3d Colour(Vector2d uv)
        {

            return Color;
        }

        public double DiffusionCoefficient { get; }
        public double SpecularCoefficient { get; }
        public double Glossiness { get; }

        public Chessboard(Vector3d color, double diffuseCoeficient, double specularCoeficient, double glossiness)
        {
            Color = color;
            DiffusionCoefficient = diffuseCoeficient;
            SpecularCoefficient = specularCoeficient;
            Glossiness = glossiness;
        }
    }




    public class PerlinNoise : IMaterial
    {
        public Vector3d Color { get; }
        public Vector3d Colour(Vector2d uv)
        {
            double coef = 1.29;

            double peerlinNose = 0;
            double peerlinNose1 = 0;
            double peerlinNose2 = 0;
            for (int i = 0; i < 8; i++)
            {
                peerlinNose += Math.Abs(Noise2D(uv.X * Math.Pow(coef, (i + 1)) , uv.Y * Math.Pow(coef, (i + 1)))) / Math.Pow(coef, (i + 1));
                peerlinNose1 += Math.Abs(Noise2D(uv.Y * Math.Pow(coef, (i + 1)), uv.X * Math.Pow(coef, (i + 1)))) / Math.Pow(coef, (i + 1));
                peerlinNose2 += Math.Abs(Noise2D(uv.X * Math.Pow(coef, (i + 1)), uv.X * Math.Pow(coef, (i + 1)))) / Math.Pow(coef, (i + 1));
            }
            
            Vector3d coulour = new Vector3d(0, 0, 0);
            coulour.X = peerlinNose ;
            coulour.Y = peerlinNose1 ;
            coulour.Z = peerlinNose2  ;

            return coulour;
        }

        public double DiffusionCoefficient { get; }
        public double SpecularCoefficient { get; }
        public double Glossiness { get; }

        public PerlinNoise(Vector3d color, double diffuseCoeficient, double specularCoeficient, double glossiness)
        {
            Color = color;
            DiffusionCoefficient = diffuseCoeficient;
            SpecularCoefficient = specularCoeficient;
            Glossiness = glossiness;
        }

        int[] pee = new int[512]  { 151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
            129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
            49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180, 151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
            190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
            88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
            77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
            102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
            135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
            223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
            129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
            251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
            49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
        };


        Vector2d GetConstantVector(int v)
        {
            int h = v & 3;
            if (h == 0)
            {
                return new Vector2d(1, 1);
            }
            else if (h == 1)
            {
                return new Vector2d(-1, 1);
            }
            else if (h == 2)
            {
                return new Vector2d(-1, -1);
            }
            else { return new Vector2d(1, -1); }
        }

        double Fade(double t)
        {
            return ((6 * t - 15) * t + 10) * t * t * t;
        }

        double Lerp(double t, double a, double b)
        {
            return a + t * (b - a);
        }
        double Noise2D(double x, double y)
        {
            int X = (int)Math.Floor(x) & 255;
            int Y = (int)Math.Floor(y) & 255;

            double xf = x-Math.Floor(x);
            double yf = y-Math.Floor(y);

            Vector2d topRight = new Vector2d(xf - 1, yf - 1);
            Vector2d topLeft = new Vector2d(xf, yf  - 1);
            Vector2d bottomRight = new Vector2d(xf - 1, yf);
            Vector2d bottomLeft = new Vector2d(xf, yf);

            int valueTopRight = pee[pee[X + 1] + Y + 1];
            int valueTopLeft = pee[pee[X] + Y + 1];
            int valueBottomRight = pee[pee[X + 1] + Y];
            int valueBottomLeft = pee[pee[X] + Y];

            double dotTopRight = Vector2d.Dot(topRight, GetConstantVector(valueTopRight));
            double dotTopLeft = Vector2d.Dot(topLeft, GetConstantVector(valueTopLeft));
            double dotBottomRight = Vector2d.Dot(bottomRight, GetConstantVector(valueBottomRight));
            double dotBottomLeft = Vector2d.Dot(bottomLeft, GetConstantVector(valueBottomLeft));

            double u = Fade(xf);
            double v = Fade(yf);

            return Lerp(u,
                Lerp(v, dotBottomLeft, dotTopLeft),
                Lerp(v, dotBottomRight, dotTopRight)
            );
        }
    }
}
