using Util;
using OpenTK.Mathematics;
using rt004.Solids;
using rt004.Textures;
//using System.Numerics;

namespace rt004;

internal class Program
{
    static void Main(string[] args)
    {
        bool db = true;

        if (db)
        {
            TestScenes ts = new TestScenes();
            ts.Test();
        }
        else
        {
            string inputDir = "Scenes";
            Console.Write("Enter the config file name from the " + '"' + inputDir + '"' + " directory:\n");

            string? configFile = Console.ReadLine();
            if (configFile is null) throw new ArgumentNullException();

            string configPath = inputDir + "/" + configFile;
            if (!File.Exists(configPath))
            {
                Console.Write("Config file of that name does not exist.\nAbort\n");
                throw new Exception("Config file '" + configFile + "' does not exist.");
            }

            Scene scene = JsonParser.ParseJsonConfig(configPath);

            FloatImage fi = new FloatImage(1200, 1200, 3); //todo: should parametrize into its own function
            int px = 1;         //modifies pixel size 
            int sampleSize = 1; //sample per pixel

            Vector3d[,] grid = scene.Camera.ParallelRayCastHierarchy(scene, sampleSize, fi.Width / px, fi.Height / px);

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
}
