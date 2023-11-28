using OpenTK.Mathematics;

namespace rt004.Solids
{
    public class Cylinder : ISolid
    {
        public IMaterial Material => throw new NotImplementedException();

        public Vector3d GetNormal(Vector3d point, bool isInside)
        {
            throw new NotImplementedException();
        }

        public Vector2d GetUVCoords(Vector3d point, bool isInside)
        {
            throw new NotImplementedException();
        }

        public double? Intersection(Ray ray)
        {
            throw new NotImplementedException();
        }
    }

}
