﻿using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    public interface ISolid
    {
        IMaterial Material { get; }
        double? Intersection(Ray ray);
        Vector2d GetUVCoords(Vector3d point, bool isInside);
        Vector3d GetNormal(Vector3d point, bool isInside);
    }

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

        public Vector3d GetNormal(Vector3d point, bool isInside)
        {
            if(isInside)
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
    /*
    public class Cylinder : ISolid
    {
        public Material Material { get; }
        public Vector3d Origin { get; }
        public double Height { get; }
        public double Width { get; }
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
    */
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
            if(Vector3d.Dot(_e2, point) is double.NaN)
            {
                return new(0, 0);
            }

            return new Vector2d(Vector3d.Dot(_e2, point), Vector3d.Dot(_e1, point));
        }


    }
}
