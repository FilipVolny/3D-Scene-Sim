using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    public class Material
    {
        public Vector3d Color { get; }
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
}
