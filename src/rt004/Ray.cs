using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public Ray(Vector3d origin, Vector3d direction, ISolid? originSolid, Matrix4d inverseTransformationMatrix)
        {
            Origin = origin;
            Direction = direction.Normalized();
            OriginSolid = originSolid;
            //InverseTransformationMatrix = inverseTransformationMatrix;
        }

        public Ray TransformRay(Matrix4d TransformationMatrix)
        {
            Vector4d tmpOrigin = new Vector4d(Origin, 1);
            tmpOrigin = TransformationMatrix * tmpOrigin;
            Vector3d transformedOrigin = new Vector3d(tmpOrigin.X, tmpOrigin.Y, tmpOrigin.Z);

            Vector4d tmpDirection = new Vector4d(Direction, 1);
            tmpDirection = TransformationMatrix * tmpDirection;
            Vector3d transformedDirection = new Vector3d(tmpDirection.X, tmpDirection.Y, tmpDirection.Z);

            return new Ray(transformedOrigin, transformedDirection, this.OriginSolid);

        }
    }
}
