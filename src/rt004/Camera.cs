using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    public class Camera
    {
        Vector3d Origin { get; }
        Vector3d ForwardDirection { get; }
        double Rotation { get; }
        private Vector3d UpDirection { get; }
        private Vector3d RightDirection { get; }

        double Height { get; }
        double Width { get; }
        int MaxRayTracingDepth { get; set; }

        public Camera(Vector3d origin, Vector3d forwardDirection, double rotation, double height, double width) //todo rotation, up and right vector, user shouldnt be able to choose any vector as the up and right vector, they have to be perpendicular to each other
        {
            Origin = origin;
            ForwardDirection = forwardDirection.Normalized();
            Rotation = rotation;
            UpDirection = ((new Vector3d(0,0,-1) * Math.Cos(Rotation)) + Vector3d.Cross(new Vector3d(0, 0, -1), ForwardDirection) * Math.Sin(Rotation) + ForwardDirection * Vector3d.Dot(new Vector3d(0, 0, -1), ForwardDirection) * (1 - Math.Cos(Rotation))).Normalized();
            RightDirection = (Vector3d.Cross(UpDirection, ForwardDirection) + ForwardDirection * Vector3d.Dot(UpDirection, ForwardDirection)).Normalized();
            Height = height;
            Width = width;
            MaxRayTracingDepth = 10;
        }

        private Ray GetRayFromCamera(int x, int y, int pixelWidth, int pixelHeight)
        {
            Ray ray = new(Origin, (ForwardDirection + (x - pixelWidth / 2) * (Width / pixelWidth) * RightDirection + (y - pixelHeight / 2) * (Height / pixelHeight) * UpDirection).Normalized());
            return ray;
        }

        public Vector3d[,] RayCast(Scene scene, int pixelWidth, int pixelHeight)
        {
            Vector3d[,] pixels = new Vector3d[pixelWidth, pixelHeight];

            for (int x = 0; x < pixelWidth; x++)
            {
                for (int y = 0; y < pixelHeight; y++)
                {
                    Ray ray = GetRayFromCamera(x, y, pixelWidth, pixelHeight);

                    (ISolid?, double?) intersection = Phong.ThrowRay(ray, scene.Solids);

                    if (intersection.Item1 != null && intersection.Item2 != null)
                    {
                        pixels[x, y] = Phong.Shade(scene, ray, 0, MaxRayTracingDepth);
                    }
                }
            }
            return pixels;
        }
    }
}
