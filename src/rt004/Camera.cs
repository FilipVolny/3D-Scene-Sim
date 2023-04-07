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
        Vector3d Forward { get; }
        Vector3d Right { get; } = new Vector3d(1, 0, 0).Normalized();
        Vector3d Up { get; }
        double Height { get; }
        double Width { get; }
        int MaxRayTracingDepth { get; set; }

        public Camera(Vector3d origin, Vector3d forward, Vector3d up, double height, double width) //todo rotation, up and right vector, user shouldnt be able to choose any vector as the up and right vector, they have to be perpendicular to each other
        {
            Origin = origin;
            Forward = forward.Normalized();
            Up = up.Normalized();
            //Right = right.Normalized();
            Height = height;
            Width = width;
            MaxRayTracingDepth = 10;
        }

        private Ray GetRayFromCamera(int x, int y, int pixelWidth, int pixelHeight)
        {
            Ray ray = new(Origin, (Forward + (x - pixelWidth / 2) * (Width / pixelWidth) * Right + (y - pixelHeight / 2) * (Height / pixelHeight) * Up).Normalized());
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
