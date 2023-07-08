using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
//todo line 43,39, its written there what needs to be done
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
        int SamplePerPixel { get; } //needs cleanup
        public Camera(Vector3d origin, Vector3d forwardDirection, double rotation, double width, double height)
        {
            Origin = origin;
            ForwardDirection = forwardDirection.Normalized();
            Rotation = rotation;
            UpDirection = ((new Vector3d(0,0,-1) * Math.Cos(Rotation)) + Vector3d.Cross(new Vector3d(0, 0, -1), ForwardDirection) * Math.Sin(Rotation) + ForwardDirection * Vector3d.Dot(new Vector3d(0, 0, -1), ForwardDirection) * (1 - Math.Cos(Rotation))).Normalized();
            RightDirection = (Vector3d.Cross(UpDirection, ForwardDirection) + ForwardDirection * Vector3d.Dot(UpDirection, ForwardDirection)).Normalized();
            Height = height;
            Width = width;
            MaxRayTracingDepth = 10; //10
            SamplePerPixel = 1;     //40
        }

        private Ray GetRayFromCamera(int x, int y, int pixelWidth, int pixelHeight)
        {
            Ray ray = new(Origin, (ForwardDirection + (x - pixelWidth / 2) * (Width / pixelWidth) * RightDirection + (y - pixelHeight / 2) * (Height / pixelHeight) * UpDirection).Normalized(), null); //todo, what if the camera is inside a solid
            return ray;
        }
        private Ray GetRayFromCameraAntiAliasing(int pixelWidth, int pixelHeight, double xPart, double yPart)
        {
            return new Ray(Origin, (ForwardDirection + xPart * (Width / pixelWidth) * RightDirection + yPart * (Height / pixelHeight) * UpDirection).Normalized(), null); //todo what if the camera is inside a solid
        }

        /*
        public Vector3d[,] ParallelRayCast(Scene scene, int pixelWidth, int pixelHeight) //No Anti-Aliasing
        {
            Vector3d[,] pixels = new Vector3d[pixelWidth, pixelHeight];
            Parallel.For(0, 8, i =>
            {
                for (int x = i; x < pixelWidth; x += 8)
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
            });
            return pixels;
        }
        */
        
        public Vector3d[,] ParallelRayCast(Scene scene, int pixelWidth, int pixelHeight) //Yes Anti-aliasing
        {
            int numberOfAvailableProcessors = Environment.ProcessorCount - 1;

            Random rnd = new();
            Vector3d[,] pixels = new Vector3d[pixelWidth, pixelHeight];

            Parallel.For(0, numberOfAvailableProcessors, i =>
            {
                for (int x = i; x < pixelWidth; x += numberOfAvailableProcessors)
                {

                    for (int y = 0; y  < pixelHeight; y++)
                    {
                        Vector3d color = Vector3d.Zero;
                        List<int> possibleRows = new();
                        for (int u = 0; u < SamplePerPixel; u++)
                        {
                            possibleRows.Add(u);
                        }
                        for (int u = 0; u < SamplePerPixel; u++)
                        {
                            int index = rnd.Next(0, possibleRows.Count);
                            double xPart = x - 0.5 - pixelWidth / 2 + possibleRows[index] / SamplePerPixel + rnd.NextDouble() / SamplePerPixel;
                            possibleRows.RemoveAt(index);

                            double yPart = y - 0.5 - pixelHeight / 2 + u / SamplePerPixel + rnd.NextDouble() / SamplePerPixel;
                            Ray ray = GetRayFromCameraAntiAliasing(pixelWidth, pixelHeight, xPart, yPart);

                            (ISolid?, double?) intersection = Phong.ThrowRay(ray, scene.Solids);

                            if (intersection.Item1 != null && intersection.Item2 != null)
                            {
                                color += Phong.Shade(scene, ray, 0, MaxRayTracingDepth) ;
                            }
                        }
                        pixels[x, y] = color / SamplePerPixel;
                    }
                }
            });
            return pixels;
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
