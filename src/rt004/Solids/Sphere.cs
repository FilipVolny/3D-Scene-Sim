using OpenTK.Mathematics;

namespace rt004.Solids
{
    public class Sphere : ISolid
    {
        public IMaterial Material { get; }
        public Vector3d Origin { get; set; }
        public double Size { get; set; }
        public Sphere(IMaterial material, Vector3d origin, double size)
        {
            Material = material;
            Origin = origin;
            Size = size;
        }

        public double? Intersection(Ray ray)
        {
            double a = Vector3d.Dot(ray.Direction, ray.Direction);
            double b = 2 * Vector3d.Dot(ray.Direction, ray.Origin - Origin);
            double c = Vector3d.Dot(ray.Origin - Origin, ray.Origin - Origin) - Size * Size;

            double d = b * b - 4 * a * c;
            if (d < 0)
            {
                return null;
            }
            else
            {
                double t1 = (-b + Math.Sqrt(d)) / (2 * a);
                double t2 = (-b - Math.Sqrt(d)) / (2 * a);

                double offset = Math.Min(t1, t2);


                if (offset < 0)
                {
                    offset = Math.Max(t1, t2);
                    if (offset < 0) { return null; }
                }

                return offset;
            }
        }

        public Vector3d GetNormal(Vector3d point, bool isInside)
        {
            if (isInside)
            {
                return Vector3d.Subtract(point, Origin).Normalized();
            }

            return Vector3d.Subtract(Origin, point).Normalized();
        }
        public Vector2d GetUVCoords(Vector3d point, bool isInside)
        {
            Vector3d normal = GetNormal(point, isInside);
            return new Vector2d(Math.Atan2(normal.X, normal.Y) / 2 * Math.PI + 0.5, normal.Z + 0.5);
        }
    }
}
