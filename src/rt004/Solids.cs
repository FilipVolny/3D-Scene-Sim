using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    public interface ISolid
    {
        Material Material { get; set; }
        double? Intersection(Ray ray);
        Vector3d GetNormal(Vector3d point);
    }

    public class Sphere : ISolid
    {
        public Material Material { get; set; }
        public Vector3d Origin { get; set; }
        public double Size { get; set; }
        public Sphere(Material material, Vector3d origin, double size)
        {
            Material = material;
            Origin = origin;
            Size = size;
        }

        public double? Intersection(Ray ray)
        {
            double a = Vector3d.Dot(ray.Direction, ray.Direction);
            double b = 2 * Vector3d.Dot(ray.Direction, ray.Origin - this.Origin);
            double c = Vector3d.Dot(ray.Origin - this.Origin, ray.Origin - this.Origin) - this.Size * this.Size;

            double d = b * b - (4 * a * c);
            if (d < 0)
            {
                return null;
            }
            else
            {
                double t1 = ((-b) + Math.Sqrt(d)) / (2 * a);
                double t2 = ((-b) - Math.Sqrt(d)) / (2 * a);

                double offset = Math.Min(t1, t2);


                if (offset < 0)
                {
                    offset = Math.Max(t1, t2);
                    if (offset < 0) { return null; }
                }

                return offset;
            }
        }

        public Vector3d GetNormal(Vector3d point)
        {
            return Vector3d.Subtract(Origin, point).Normalized();
        }
    }

    public class Cylinder : ISolid
    {
        public Material Material { get; set; }
        public Vector3d Origin { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public Cylinder(Material material, Vector3d origin, double height, double width)
        {
            Material = material;
            Origin = origin;
            Height = height;
            Width = width;
        }
        public double? Intersection(Ray ray)
        {
            return 0;
        }
        public Vector3d GetNormal(Vector3d point)
        {
            return point; //todo
        }
    }

    public class Plane : ISolid
    {
        public Material Material { get; set; }
        public Vector3d Origin { get; set; }
        public Vector3d Vector { get; set; }
        public Plane(Material material, Vector3d origin, Vector3d vector)
        {
            Material = material;
            Origin = origin;
            Vector = vector.Normalized();
        }

        public double? Intersection(Ray ray)
        {
            double d = -Vector3d.Dot(Origin, Vector);
            if (Vector3d.Dot(Vector, ray.Direction) == 0)
            {
                return null;
            }
            double offset = -(Vector3d.Dot(Vector, ray.Origin) + d) / Vector3d.Dot(Vector, ray.Direction);
            if (offset < 0) { return null; }
            return offset;
        }
        public Vector3d GetNormal(Vector3d point)
        {
            return Vector;
        }
    }

}
