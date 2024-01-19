using OpenTK.Mathematics;
using rt004.Textures;

namespace rt004.Solids
{


    /// <summary>
    /// Interface for 3D scene solids.
    /// </summary>
    public interface ISolid
    {
        /// <summary>
        /// Defines how light interacts with the solid.
        /// </summary>
        IMaterial Material { get; }

        
        /// <summary>
        /// Checks if this solid intersects with the ray.
        /// </summary>
        /// <param name="ray">Ray to check.</param>
        /// <returns>Distance from the solids intersected point to the rays origin. Or null if the ray doesn't intersect solid.</returns>
        double? Intersection(Ray ray);


        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="point"></param>
        /// <param name="isInside"></param>
        /// <returns></returns>
        Vector2d GetUVCoords(Vector3d point, bool isInside);


        /// <summary>
        /// Finds the normal vector of the solids surface at the point. 
        /// </summary>
        /// <param name="point">The point from which the normal vector is computed</param>
        /// <param name="isInside">The normal vector is inside or outside the solid.</param>
        /// <returns></returns>
        Vector3d GetNormal(Vector3d point, bool isInside);
    }
}
