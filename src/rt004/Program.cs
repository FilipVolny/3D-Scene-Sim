using Util;
using OpenTK.Mathematics;
//using System.Numerics;

namespace rt004;

internal class Program
{
    private static FloatImage GenerateCoolImage(FloatImage image)
    {
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                float value1 = (float)(Math.Cos(y / 2));
                float value2 = (float)(Math.Cos(x*x));
                float[] pixel = new float[3] { value1, value1 * value2, value2 };
                image.PutPixel(x, y, pixel);
            }
        }
        return image;
    }

    static void Main(string[] args)
    {
        // Parameters.
        // TODO: parse command-line arguments and/or your config file.
        //Config config = new Config();
        //config.LoadFromFile("config.txt");
        /*
        Scene sceneFromParser = new Scene("config.txt");
        
        Scene demo = new Scene(0.2, new Camera((0,0,0), (0,1,0), 0, 2, 2)); //ambientLight, camera(origin, forward, up, width, height)

        Material mat1 = new Material(new Vector3d(0, 0, 1), 0.4, 0.4, 50); //color, diffusionCoeff, specularCoeff, glossiness 
        Material mat2 = new Material(new Vector3d(0, 1.5, 0), 0.4, 0.5, 5);
        Material mat3 = new Material(new Vector3d(1, 0, 0), 0.4, 0.9, 5);

        Plane planius = new Plane(mat3, (0, 100, 0), new Vector3d(0, 1, 0).Normalized()); //material, origin, normalVector
        Plane planius2 = new Plane(mat1, (0, 100, 0), new Vector3d(1, 1, 0).Normalized());

        Sphere spherocious = new Sphere(mat1, new Vector3d(-25, 40, -10), 10); //material, origin, size
        Sphere spherocious2 = new Sphere(mat3, new Vector3d(0, 40, 0), 10);
        Sphere babz = new Sphere(mat2, new Vector3d(25, 40, 10), 10);

        DirectionLightSource light = new((-1, 0, 0), (1, 1, 1), 0.5); //origin, direction, color, intensity
        DirectionLightSource light2 = new((0, 500, 300), (1, 1, 1), 0.5);

        demo.Solids.Add(planius);
        //demo.Solids.Add(planius2);
        
        demo.Solids.Add(spherocious);
        demo.Solids.Add(spherocious2);
        demo.Solids.Add(babz);
        
        demo.LightSources.Add(light);
        demo.LightSources.Add(light2);

        Material plush = new Material((1, 0.5, 0), 0.9, 0.1, 500);

        Sphere Bleep = new Sphere(plush, new(25, 40, -25), 5);

        demo.Solids.Add(Bleep);
        */
        FloatImage fi = new FloatImage(1200, 1200, 3);
        Scene demo = JsonParser.ParseJsonConfig("config.json");

        Vector3d[,] grid = demo.Camera.RayCast(demo, fi.Width/5, fi.Height/5);
        

        static void ParallelImage(FloatImage image, Vector3d[,] grid)
        {

            var offsets = Enumerable.Range(0, 8);
            Parallel.ForEach(offsets, FillImagePart =>
            {
                for(int y =  0; y < image.Height / 8; ++y)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        float[] color = new float[3];
                        color[0] = (float)grid[x / 5, y / 5].X;
                        color[1] = (float)grid[x / 5, y / 5].Y;
                        color[2] = (float)grid[x / 5, y / 5].Z;
                        image.PutPixel(x, y, color);
                    }
                }
            });
        }
        /*
        for (int y = 0; y < fi.Height; y++)
        {
            for (int x = 0; x < fi.Width; x++)
            {
                float[] color = new float[3];
                color[0] = (float)grid[x/5, y/5].X;
                color[1] = (float)grid[x/5, y/5].Y;
                color[2] = (float)grid[x/5, y/5].Z;

                fi.PutPixel(x, y, color);
            }
        }
        */
        ParallelImage(fi, grid);
        string fileName = "config.pfm";
        /* HDR image.
        FloatImage fi = new FloatImage(wid, hei, 3);
        fi = GenerateCoolImage(fi);
        */


        // TODO: put anything interesting into the image.
        // TODO: use fi.PutPixel() function, pixel should be a float[3] array [R, G, B]

        //fi.SaveHDR(fileName);   // Doesn't work well yet...
        
        fi.SavePFM(fileName);

        Console.WriteLine("HDR image is finished.");
    }
}
