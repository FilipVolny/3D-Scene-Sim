using OpenTK.Mathematics;
using rt004.Solids;

//todo line 43,39, its written there what needs to be done
namespace rt004
{
    /// <summary>
    /// A camera for a 3D scene.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// The origin point of the camera in a 3D space.
        /// </summary>
        Vector3d Origin { get; }

        /// <summary>
        /// The direction the camera is facing in a 3D space.
        /// </summary>
        Vector3d ForwardDirection { get; }

        /// <summary>
        /// The rotation of the cameras view in 3D space.
        /// </summary>
        double Rotation { get; }

        /// <summary>
        /// The direction the top of the camera is facing in 3D space.
        /// </summary>
        private Vector3d UpDirection { get; }

        /// <summary>
        /// The direction the right side of the camera is facing in 3D space.
        /// </summary>
        private Vector3d RightDirection { get; }

        /// <summary>
        /// Vertical fov of the camera.
        /// </summary>
        double Height { get; }

        /// <summary>
        /// Horizontal fov of the camera.
        /// </summary>
        double Width { get; }

        /// <summary>
        /// Maximum number of bounces for a ray.
        /// </summary>
        int MaxRayTracingDepth { get; set; }

        /// <summary>
        /// Constructor for the Camera object used in a 3D scene.
        /// </summary>
        /// <param name="origin">Origin point of the Camera object in a 3D scene.</param>
        /// <param name="forwardDirection">The direction the Camera is facing.</param>
        /// <param name="rotation">The rotation of the </param>
        /// <param name="width"></param>
        /// <param name="height"></param>
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
        }

        /// <summary>
        /// Creates an instance of a Ray in a particular direction.
        /// </summary>
        /// <param name="pixelWidth">Which column of pixels is used to cast the ray</param>
        /// <param name="pixelHeight"></param>
        /// <param name="xPart">Used to compute the width part of anti-aliasing</param>
        /// <param name="yPart"></param>
        /// <returns></returns>
        private Ray _getRayFromCameraAntiAliasing(int pixelWidth, int pixelHeight, double xPart, double yPart)
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
        
        /// <summary>
        /// Creates a grid of pixels and computes color in Vector3d for each of them from a 3D scene. Utilizes anti-aliasing.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="sampleSize"></param>
        /// <param name="pixelWidth"></param>
        /// <param name="pixelHeight"></param>
        /// <returns> Vector3d matrix of colors. </returns>
        /// <remarks> Utilizes multiple CPUs. </remarks>
        public Vector3d[,] ParallelRayCast(Scene scene, int sampleSize, int pixelWidth, int pixelHeight)
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
                        for (int u = 0; u < sampleSize; u++)
                        {
                            possibleRows.Add(u);
                        }
                        for (int u = 0; u < sampleSize; u++)
                        {
                            int index = rnd.Next(0, possibleRows.Count);
                            double xPart = x - 0.5 - pixelWidth / 2 + possibleRows[index] / sampleSize + rnd.NextDouble() / sampleSize;
                            possibleRows.RemoveAt(index);

                            double yPart = y - 0.5 - pixelHeight / 2 + u / sampleSize + rnd.NextDouble() / sampleSize;
                            Ray ray = _getRayFromCameraAntiAliasing(pixelWidth, pixelHeight, xPart, yPart);

                            (ISolid?, double?) intersection = Phong.ThrowRay(ray, scene.Solids);
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
                            color += Phong.Shade(scene, ray, 0, MaxRayTracingDepth) ;
                        }
                        pixels[x, y] = color / sampleSize;
                    }
                }
            });
            return pixels;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="sampleSize"></param>
        /// <param name="pixelWidth"></param>
        /// <param name="pixelHeight"></param>
        /// <returns></returns>
        public Vector3d[,] ParallelRayCastHierarchy(Scene scene, int sampleSize, int pixelWidth, int pixelHeight) //Yes Anti-aliasing
        {
            int numberOfAvailableProcessors = Environment.ProcessorCount - 1;

            Random rnd = new();
            Vector3d[,] pixels = new Vector3d[pixelWidth, pixelHeight];

            Parallel.For(0, numberOfAvailableProcessors, i =>
            {
                for (int x = i; x < pixelWidth; x += numberOfAvailableProcessors)
                {

                    for (int y = 0; y < pixelHeight; y++)
                    {
                        Vector3d color = Vector3d.Zero;
                        List<int> possibleRows = new();
                        for (int u = 0; u < sampleSize; u++)
                        {
                            possibleRows.Add(u);
                        }
                        for (int u = 0; u < sampleSize; u++)
                        {
                            int index = rnd.Next(0, possibleRows.Count);
                            double xPart = x - 0.5 - pixelWidth / 2 + possibleRows[index] / sampleSize + rnd.NextDouble() / sampleSize;
                            possibleRows.RemoveAt(index);

                            double yPart = y - 0.5 - pixelHeight / 2 + u / sampleSize + rnd.NextDouble() / sampleSize;
                            Ray ray = _getRayFromCameraAntiAliasing(pixelWidth, pixelHeight, xPart, yPart);

                            color += Phong.ShadeHierarchy(scene, ray, 0, sampleSize, MaxRayTracingDepth);
                        }
                        pixels[x, y] = color / sampleSize;
                    }
                }
            });
            return pixels;
        }
        /* redundant
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
        */
    }
}
