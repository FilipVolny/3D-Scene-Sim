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

        private Ray GetRayFromCamera(int x, int y, int width, int height)
        {
            Ray ray = new Ray(Origin, (Forward + (x - width / 2) * (Width / width) * Right + (y - height / 2) * (Height / height) * Up).Normalized());
            return ray;
        }

        public Vector3d[,] RayCast(Scene scene, int width, int height)
        {
            Vector3d[,] pixels = new Vector3d[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Ray ray = GetRayFromCamera(x, y, width, height);

                    (ISolid?, double?) intersection = Phong.ThrowRay(ray, scene.Solids);

                    if (intersection.Item1 != null && intersection.Item2 != null)
                    {
                        Vector3d intersectionPoint = (Vector3d)(ray.Origin + (intersection.Item2 * ray.Direction));
                        pixels[x, y] = Phong.Shade(scene, ray, 0, MaxRayTracingDepth);
                    }
                }
            }
            return pixels;
        }
    }
}
