using Util;
using OpenTK.Mathematics;
using static System.Net.Mime.MediaTypeNames;
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
        
        //Scene sceneFromParser = new Scene("config.txt");
        
        Scene demo = new Scene(0.2, new Camera((0,0,0), (0,1,0), 0, 2, 2)); //ambientLight, camera(origin, forward, up, width, height)

        Material blu = new Material(new Vector3d(0.3, 0.3, 1), 1, 0, 5, 0, 0); //color, diffusionCoeff, specularCoeff, glossiness 
        Material green = new Material(new Vector3d(0, 1, 0), 0.5, 0.5, 5, 0, 0);
        Material red = new Material(new Vector3d(1, 0, 0), 0.1, 0.9, 500, 0.2, 5);
        Material glass = new Material(new Vector3d(0.1, 0, 0), 1, 0, 5, 1, 1.05);


        Material matPlane = new Material(new Vector3d(0.3, 0.3, 0.3), 0.99, 0.01, 500, 0, 0);
        Material matPlane2 = new Material(new Vector3d(0.3, 0.3, 0.3), 0.99, 0.01, 500, 0, 0);

        PerlinNoise PeerlinNoise = new((0, 2, 0), 0.5, 0.1, 500, 0, 0);

        Plane pBack = new Plane(matPlane, (0, 100, 0), new Vector3d(0, 1, 0).Normalized()); //material, origin, normalVector
        Plane pFloor = new Plane(PeerlinNoise, (0, 40, -25), new Vector3d(0, 0, -100).Normalized());
        Plane pRight = new Plane(blu, (200, 0, 0), new Vector3d(1, 0, 0).Normalized());
        Plane pLeft = new Plane(matPlane, (-200, 0, 0), new Vector3d(-1, 0, 0).Normalized());
        Plane pCeiling = new Plane(matPlane, (0, 0, 500), new Vector3d(0, 0, 1).Normalized());

        Plane frontWall = new Plane(blu, new Vector3d(0, -100, 0), new Vector3d(0, -1, 0).Normalized());

        Sphere spherocious = new Sphere(blu, new Vector3d(-25, -40, -5), 15); //material, origin, size
        Sphere spherocious2 = new Sphere(glass, new Vector3d(-20, 50, -5), 15);
        Sphere babz = new Sphere(green, new Vector3d(25, 40, -10), 10);
        
        SphericalLightSource light = new((-100, -50, 20), (1, 1, 1), 1, 40); //origin, direction, color, intensity, radius
        PointLightSource pointLt = new((-50, 0, 0), (0.2, 0.2, 0.2), 0.2);
        //DirectionLightSource light2 = new((0, 500, 300), (1, 1, 1), 0.25);

        //demo.Solids.Add(pBack);
        demo.Solids.Add(pFloor);
        //demo.Solids.Add(pRight);
        //demo.Solids.Add(pLeft);
        //demo.Solids.Add(pCeiling);
        //demo.Solids.Add(frontWall);

        demo.Solids.Add(spherocious);
        demo.Solids.Add(spherocious2);
        demo.Solids.Add(babz);
        demo.LightSources.Add(light);
        //demo.LightSources.Add(pointLt);
        //demo.LightSources.Add(light2);

        Material plush = new Material((1, 0.5, 0), 0.9, 0.1, 500, 0, 0);
        Sphere Bleep = new Sphere(plush, new Vector3d(-10, 60, -5), 5);

        demo.Solids.Add(Bleep);
        
        FloatImage fi = new FloatImage(1200, 1200, 3);
        //Scene demo = JsonParser.ParseJsonConfig("config.json");
        int px = 1;
        Vector3d[,] grid = demo.Camera.ParallelRayCast(demo, fi.Width/px, fi.Height/px);

        for (int y = 0; y < fi.Height; y++)
        {
            for (int x = 0; x < fi.Width; x++)
            {
                float[] color = new float[3];
                color[0] = (float)grid[x/px, y/px].X;
                color[1] = (float)grid[x/px, y/px].Y;
                color[2] = (float)grid[x/px, y/px].Z;

                fi.PutPixel(x, y, color);
            }
        }
        string fileName = "refraction.pfm";
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
