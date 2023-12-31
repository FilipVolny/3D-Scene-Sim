﻿using Util;
using OpenTK.Mathematics;
using rt004.Solids;
using rt004.Textures;
//using System.Numerics;

namespace rt004;

internal class Program
{
    /*
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
    */
    static void Main(string[] args)
    {
        
        Scene tmp = new Scene(0.2, new Camera((0,-10,-20), (0,1,0), 0, 2, 2)); //ambientLight, camera(origin, forward, up, width, height)

        Material blu = new Material(new Vector3d(0.3, 0.3, 1), 1, 0, 5, 0, 0); //color, diffusionCoeff, specularCoeff, glossiness 
        Material green = new Material(new Vector3d(0, 1, 0), 0.5, 0.5, 5, 0, 0);
        Material red = new Material(new Vector3d(1, 0, 0), 0.1, 0.9, 500, 0.2, 5);
        Material glass = new Material(new Vector3d(0.1, 0, 0), 1, 0, 5, 1, 1.05);


        Material matPlane = new Material(new Vector3d(0.3, 0.3, 0.3), 0.99, 0.01, 500, 0, 0);
        Material matPlane2 = new Material(new Vector3d(0.3, 0.3, 0.3), 0.99, 0.01, 500, 0, 0);

        PerlinNoise PeerlinNoise = new((0, 2, 0), 0.5, 0.1, 500, 0, 0);


        //testing checkers
        Checkers check = new(new(1,0,0), 1, 0, 5, 0, 0, 3);
        //


        Plane pBack = new Plane(matPlane, (0, 100, 0), new Vector3d(0, 1, 0).Normalized()); //material, origin, normalVector
        Plane pFloor = new Plane(check, (0, 40, -25), new Vector3d(0, 0, -1).Normalized());
        Plane pRight = new Plane(blu, (200, 0, 0), new Vector3d(1, 0, 0).Normalized());
        Plane pLeft = new Plane(matPlane, (-200, 0, 0), new Vector3d(-1, 0, 0).Normalized());
        Plane pCeiling = new Plane(matPlane, (0, 0, 500), new Vector3d(0, 0, 1).Normalized());

        Plane frontWall = new Plane(blu, new Vector3d(0, -100, 0), new Vector3d(0, -1, 0).Normalized());

        Sphere spherocious = new Sphere(blu, new Vector3d(-25, -40, -5), 15); //material, origin, size
        Sphere spherocious2 = new Sphere(glass, new Vector3d(-20, 50, -5), 15);
        Sphere babz = new Sphere(green, new Vector3d(25, 40, -10), 10);
        
        SphericalLightSource light = new((-100, -50, 30), (1, 1, 1), 1, 50); //origin, direction, color, intensity, radius
        //PointLightSource pointLt = new((-50, 0, 0), (0.2, 0.2, 0.2), 0.2);
        //DirectionLightSource light2 = new((0, 500, 300), (1, 1, 1), 0.25);

        //demo.Solids.Add(pBack);
        tmp.Solids.Add(pFloor);
        //demo.Solids.Add(pRight);
        //demo.Solids.Add(pLeft);
        //demo.Solids.Add(pCeiling);
        //demo.Solids.Add(frontWall);

        //tmp.Solids.Add(spherocious);
        //tmp.Solids.Add(spherocious2);
        //tmp.Solids.Add(babz);
        tmp.LightSources.Add(light);

        //testing boxes
        Material boxGlass = new(new Vector3d(0, 0, 0), 1, 0, 5, 1, 10);

        Box Adam = new Box(blu, new Vector3d(10, 50, -20), 10);
        tmp.Solids.Add(Adam);
        Box Steve = new Box(blu, new Vector3d(20, 50, 20), 10);
        //tmp.Solids.Add(Steve);
        Box Eve = new Box(blu, new Vector3d(20, 50, 20), 10);
        //tmp.Solids.Add(Eve);
        Box Sadam = new Box(blu, new Vector3d(0, 50, 10), 10);
        //tmp.Solids.Add(Sadam);
        //demo.LightSources.Add(pointLt);
        //demo.LightSources.Add(light2);

        Material plush = new Material((1, 0.5, 0), 0.9, 0.1, 500, 0, 0);
        Sphere Bleep = new Sphere(plush, new Vector3d(-10, 60, -5), 5);

        //tmp.Solids.Add(Bleep);

        ///////////////////////////////////////////




        //////////////////////////////
        /*
        string inputDir = "Scenes";
        Console.Write("Enter the config file name from the " + '"' + inputDir + '"' + " directory:\n");
        
        //string? configFileName = Console.ReadLine();
        string? configFile = "ctrans.json"; // for testing
        string? configPath = inputDir + "/" + configFile;
        if (!File.Exists(configPath))
        {
            Console.Write("Config file does not exist.\nAbort\n");
            return;
        }

        Scene scene = JsonParser.ParseJsonConfig(configPath);
        */
        //testing; delete later, everything else stays the same 
        Scene scene = tmp; //
        string configFile = "nonHiearchyModel_box0";
        //


        FloatImage fi = new FloatImage(1200, 1200, 3);
        int px = 1;
        int sampleSize = 10; //sample per pixel
        Vector3d[,] grid = scene.Camera.ParallelRayCast(scene, sampleSize, fi.Width / px, fi.Height / px);

        for (int y = 0; y < fi.Height; y++)
        {
            for (int x = 0; x < fi.Width; x++)
            {
                float[] color = new float[3];
                color[0] = (float)grid[x / px, y / px].X;
                color[1] = (float)grid[x / px, y / px].Y;
                color[2] = (float)grid[x / px, y / px].Z;

                fi.PutPixel(x, y, color);
            }
        }

        string outputFile = configFile.Split('.')[0] + ".pfm";
        string outputDir = "Output";

        fi.SavePFM(outputDir + "/" + outputFile);
        
        Console.WriteLine("HDR image is finished.");
        Console.WriteLine("File was saved as " + '"' + outputFile + '"' + " in the " + '"' + outputDir + '"' + " directory.");
    }
}
