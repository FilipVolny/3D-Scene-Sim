using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace rt004
{
    public static class Matrix4dExtensions
    {
        public static Matrix4d ExtractTranslationMatrix(this Matrix4d matrix)
        {
            Matrix4d result = Matrix4d.Identity;
            result.Column3 = matrix.Column3;
            return result;
        }
        public static Matrix4d Transposed(this Matrix4d matrix)
        {
            matrix.Transpose();
            return matrix;
        }
    }
}
