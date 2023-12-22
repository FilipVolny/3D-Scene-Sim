using OpenTK.Mathematics;
using rt004.Solids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    public struct Ray
    {
        public Vector3d Origin { get; set; }
        public Vector3d Direction { get; set; }
        public ISolid? OriginSolid { get; set; }
        //public Matrix4d InverseTransformationMatrix {get; set;} // redundant do not use
        public Ray(Vector3d origin, Vector3d direction, ISolid? originSolid)
        {
            Origin = origin;
            Direction = direction.Normalized();
            OriginSolid = originSolid;
            //InverseTransformationMatrix = Matrix4d.Identity;
        }
        /*
        public Ray(Vector3d origin, Vector3d direction, ISolid? originSolid, Matrix4d inverseTransformationMatrix)
        {
            Origin = origin;
            Direction = direction.Normalized();
            OriginSolid = originSolid;
            //InverseTransformationMatrix = inverseTransformationMatrix;
        }*/
        public Ray TransformOrigin(Matrix4d TransformationMatrix)
        {
            Vector4d tmpOrigin = new Vector4d(Origin, 1);
            tmpOrigin = TransformationMatrix * tmpOrigin;

            Vector3d transformedOrigin = tmpOrigin.Xyz;

            return new Ray(transformedOrigin, Direction, OriginSolid);
        }
        public void TransformedOrigin(Matrix4d TransformationMatrix)
        {
            Vector4d tmpOrigin = new Vector4d(Origin, 1);
            tmpOrigin = TransformationMatrix * tmpOrigin;

            Vector3d transformedOrigin = tmpOrigin.Xyz;

            Origin = transformedOrigin;
        }

        public Ray TransformDirection(Matrix4d TransformationMatrix)
        {
            Vector4d tmpDirection = new Vector4d(Direction, 0);
            tmpDirection = TransformationMatrix * tmpDirection;

            Vector3d transformedDirection = tmpDirection.Xyz;

            return new(Origin, transformedDirection, OriginSolid);
        }
        public void TransformedDirection(Matrix4d TransformationMatrix)
        {
            Vector4d tmpDirection = new Vector4d(Direction, 0);
            tmpDirection = TransformationMatrix * tmpDirection;

            Vector3d transformedDirection = tmpDirection.Xyz;

            Direction = transformedDirection;
        }

        public Ray TransformRay(Matrix4d TransformationMatrix)
        {
            Ray transformedRay = new(Origin, Direction, OriginSolid);
            transformedRay.TransformedOrigin(TransformationMatrix);
            transformedRay.TransformedDirection(TransformationMatrix);
            return transformedRay;
        }
    }
}
