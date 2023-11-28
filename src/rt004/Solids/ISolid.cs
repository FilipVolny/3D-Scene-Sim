using OpenTK.Mathematics;

namespace rt004.Solids
{
    public interface ISolid
    {
        IMaterial Material { get; }
        double? Intersection(Ray ray);
        Vector2d GetUVCoords(Vector3d point, bool isInside);
        Vector3d GetNormal(Vector3d point, bool isInside);
    }
}
