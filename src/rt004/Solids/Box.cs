using OpenTK.Mathematics;
using rt004.Textures;

namespace rt004.Solids
{
    public class Box : ISolid
    {
        public IMaterial Material { get; }
        public Vector3d Origin { get; set; }
        public double Size { get; set; }
        public Vector3d MinVertex { get; set; }
        public Vector3d MaxVertex { get; set; }
        public Box(IMaterial material, Vector3d origin, double size)
        {
            Material = material;
            Origin = origin;
            Size = size;

            MinVertex = new Vector3d(-0.5, -0.5, -0.5) * Size + Origin; //not sure if this is correct
            MaxVertex = new Vector3d(0.5, 0.5, 0.5) * Size + Origin;
        }

        public Vector3d GetNormal(Vector3d point, bool isInside)
        {
            Vector3d tmp = point - Origin;
            Vector3d normal = Vector3d.Zero;
            double maxCoord = Math.Max(Math.Max(Math.Abs(tmp.X), Math.Abs(tmp.Y)), Math.Abs(tmp.Z));
            if (maxCoord == Math.Abs(tmp.X)) { normal.X = Math.Sign(tmp.X) * -1; }
            if (maxCoord == Math.Abs(tmp.Y)) { normal.Y = Math.Sign(tmp.Y) * -1; }
            if (maxCoord == Math.Abs(tmp.Z)) { normal.Z = Math.Sign(tmp.Z) * -1; }
            if (isInside)
            {
                return -normal;
            }

            if (normal == Vector3d.Zero) { throw new Exception("Box.GetNormal() want's to return a zero vector, THIS SHOULD NOT HAPPEN"); }

            return normal;

        }

        public Vector2d GetUVCoords(Vector3d point, bool isInside)
        {

            return new Vector2d(0, 0);
            throw new NotImplementedException();
        }

        public double? Intersection(Ray ray)
        {
            double tx1 = (MinVertex.X - ray.Origin.X) / ray.Direction.X;
            double tx2 = (MaxVertex.X - ray.Origin.X) / ray.Direction.X;

            double tmin = Math.Min(tx1, tx2);
            double tmax = Math.Max(tx1, tx2);

            double ty1 = (MinVertex.Y - ray.Origin.Y) / ray.Direction.Y;
            double ty2 = (MaxVertex.Y - ray.Origin.Y) / ray.Direction.Y;

            tmin = Math.Max(tmin, Math.Min(ty1, ty2));
            tmax = Math.Min(tmax, Math.Max(ty1, ty2));

            double tz1 = (MinVertex.Z - ray.Origin.Z) / ray.Direction.Z;
            double tz2 = (MaxVertex.Z - ray.Origin.Z) / ray.Direction.Z;

            tmin = Math.Max(tmin, Math.Min(tz1, tz2));
            tmax = Math.Min(tmax, Math.Max(tz1, tz2));

            if (tmin > tmax || tmax < 0)
            {
                return null;
            }

            if (tmin < 0)
            {
                return tmax;
            }

            return tmin;
        }

    }
}
