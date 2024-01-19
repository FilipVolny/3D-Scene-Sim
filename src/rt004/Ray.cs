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
    /// <summary>
    /// Represents a ray, defined by its origin and direction.
    /// </summary>
    public struct Ray
    {


        /// <summary>
        /// Represents rays origin in a 3D space.
        /// </summary>
        public Vector3d Origin { get; set; }


        /// <summary>
        /// Represents rays direction in a 3D space.
        /// </summary>
        public Vector3d Direction { get; set; }


        /// <summary>
        /// The solid the rays origin is placed in, in the 3D scene.
        /// </summary>
        public ISolid? OriginSolid { get; set; }


        /// <summary>
        /// Initializes a new instance of the ray struct.
        /// </summary>
        /// <param name="origin">Represents rays origin in a 3D space.</param>
        /// <param name="direction">Represents rays direction in a 3D space.</param>
        /// <param name="originSolid">The solid the rays origin is placed in, in the 3D scene.</param>
        public Ray(Vector3d origin, Vector3d direction, ISolid? originSolid)
        {
            Origin = origin;
            Direction = direction.Normalized();
            OriginSolid = originSolid;
        }


        /// <summary>
        /// Creates a copy of this ray and transforms its origin by a 4x4 matrix.
        /// </summary>
        /// <param name="TransformationMatrix"> A 4x4 matrix of type double</param>
        /// <returns>An instance of this ray with transformed origin.</returns>
        public Ray TransformOrigin(Matrix4d TransformationMatrix)
        {
            Vector4d tmpOrigin = new Vector4d(Origin, 1);
            tmpOrigin = TransformationMatrix * tmpOrigin;

            Vector3d transformedOrigin = tmpOrigin.Xyz;

            return new Ray(transformedOrigin, Direction, OriginSolid);
        }


        /// <summary>
        /// Transforms this rays origin by a given 4x4 matrix.
        /// </summary>
        /// <param name="TransformationMatrix">A 4x4 matrix of type double</param>
        public void TransformedOrigin(Matrix4d TransformationMatrix)
        {
            Vector4d tmpOrigin = new Vector4d(Origin, 1);
            tmpOrigin = TransformationMatrix * tmpOrigin;

            Vector3d transformedOrigin = tmpOrigin.Xyz;

            Origin = transformedOrigin;
        }


        /// <summary>
        /// Creates a copy of this ray and transforms its direction by a 4x4 matrix.
        /// </summary>
        /// <param name="TransformationMatrix">A 4x4 matrix of type double</param>
        /// <returns>An instance of this ray with transformed direction.</returns>
        public Ray TransformDirection(Matrix4d TransformationMatrix)
        {
            Vector4d tmpDirection = new Vector4d(Direction, 0);
            tmpDirection = TransformationMatrix * tmpDirection;

            Vector3d transformedDirection = tmpDirection.Xyz;

            return new(Origin, transformedDirection, OriginSolid);
        }


        /// <summary>
        /// Transforms this rays direction by a given 4x4 matrix.
        /// </summary>
        /// <param name="TransformationMatrix">A 4x4 matrix of type double</param>
        public void TransformedDirection(Matrix4d TransformationMatrix)
        {
            Vector4d tmpDirection = new Vector4d(Direction, 0);
            tmpDirection = TransformationMatrix * tmpDirection;

            Vector3d transformedDirection = tmpDirection.Xyz;

            Direction = transformedDirection;
        }


        /// <summary>
        /// Creates a copy of this ray and transforms its direction and origin by a 4x4 matrix.
        /// </summary>
        /// <param name="TransformationMatrix">A 4x4 matrix of type double</param>
        /// <returns>An instance of this ray with its direction and origin.</returns>
        public Ray TransformRay(Matrix4d TransformationMatrix)
        {
            Ray transformedRay = new(Origin, Direction, OriginSolid);
            transformedRay.TransformedOrigin(TransformationMatrix);
            transformedRay.TransformedDirection(TransformationMatrix);
            return transformedRay;
        }
    }
}
