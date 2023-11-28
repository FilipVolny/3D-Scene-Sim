using OpenTK.Mathematics;

namespace rt004.Solids
{
    public class Plane : ISolid
    {
        public IMaterial Material { get; }
        public Vector3d Origin { get; set; }
        public Vector3d NormalVector { get; set; }
        private readonly Vector3d _e1;
        private readonly Vector3d _e2;
        public Plane(IMaterial material, Vector3d origin, Vector3d normalVector)
        {
            Material = material;
            Origin = origin;
            NormalVector = normalVector.Normalized();

            _e1 = Vector3d.Cross(NormalVector, Vector3d.UnitX).Normalized(); //Vector here is the normal vector
            _e2 = Vector3d.Cross(NormalVector, _e1).Normalized();
        }

        public double? Intersection(Ray ray)
        {
            double d = -Vector3d.Dot(Origin, NormalVector);
            if (Vector3d.Dot(NormalVector, ray.Direction) == 0)
            {
                return null;
            }
            double offset = -(Vector3d.Dot(NormalVector, ray.Origin) + d) / Vector3d.Dot(NormalVector, ray.Direction);
            if (offset <= 0) { return null; } //changed from < to <=, not sure if it does anything made more sense in my head at time of writing
            return offset;
        }

        public Vector3d GetNormal(Vector3d point, bool isInside)
        {
            return NormalVector;
        }

        public Vector2d GetUVCoords(Vector3d point, bool isInside) //dolaterbater
        {
            if (Vector3d.Dot(_e2, point) is double.NaN)
            {
                return new(0, 0);
            }

            return new Vector2d(Vector3d.Dot(_e2, point), Vector3d.Dot(_e1, point));
        }
    }

}
