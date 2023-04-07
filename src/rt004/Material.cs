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
        public Vector3d Color { get; set; }
        public double DiffuseCoefficient { get; set; }
        public double SpecularCoefficient { get; set; }
        public double Glossiness { get; set; }

        public Material(Vector3d color, double diffuseCoeficient, double specularCoeficient, double glossiness)
        {
            Color = color;
            DiffuseCoefficient = diffuseCoeficient;
            SpecularCoefficient = specularCoeficient;
            Glossiness = glossiness;
        }
    }
}
