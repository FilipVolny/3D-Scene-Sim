using OpenTK.Mathematics;
using rt004.Solids;
using rt004.Textures;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Util;
//file to have scene instances at one place. Used to compare outputs of hiearchical version and non-hiearchical version.
namespace rt004
{

    public class TestScenes
    {
        //this should've been in .gitIgnore

        public int nhThrowRaySucces;
        public int hThrowRaySucces;
        public Scene nhs1;
        public Scene? nhs2;
        public Scene? nhs3;

        public Scene hs1;
        public Scene? hs2;
        public Scene? hs3;

        public Scene? ths1;
        public Scene? tnhs1;
        public TestScenes()
        {
            Material blu = new Material(new Vector3d(0.3, 0.3, 1), 1, 0, 5, 0, 0); //color, diffusionCoeff, specularCoeff, glossiness 
            Material green = new Material(new Vector3d(0, 1, 0), 0.5, 0.5, 5, 0, 0);
            Material red = new Material(new Vector3d(1, 0, 0), 0.1, 0.9, 500, 0.2, 5);
            Material plush = new Material((1, 0.5, 0), 0.9, 0.1, 500, 0, 0);
            Material glass = new Material(new Vector3d(0.1, 0, 0), 1, 0, 5, 1, 1.05);
            Checkers check = new(new(1, 0, 0), 1, 0, 5, 0, 0, 3);
            PerlinNoise PeerlinNoise = new((0, 2, 0), 0.5, 0.1, 500, 0, 0);

            Plane pFloor = new Plane(blu, new(0, 40, -25), new Vector3d(0, 0, -1).Normalized());
            Sphere spherocious = new Sphere(blu, new Vector3d(-25, -40, -5), 15); //material, origin, size
            Sphere spherocious2 = new Sphere(glass, new Vector3d(-20, 50, -5), 15);
            Sphere babz = new Sphere(green, new Vector3d(25, 40, -10), 10);
            Sphere Bleep = new Sphere(plush, new Vector3d(-10, 60, -5), 5);
            Box Adam = new Box(green, new Vector3d(10, 50, -20), 10);

            SphericalLightSource light = new((-100, -50, 30), (1, 1, 1), 1, 50); //origin, direction, color, intensity, radius
            /*
             * ABOUT MATRIX ORDER
                (unsure about this matrix order, revise later, it might be important what the context is,
                and I am unsure if there is one right, single way for the matrices to be ordered)
             * 1st translate 
             * 2nd rotate (and with the rotation, the order of rotations around different axes matters too)
             * 3rd scale

             * ABOUT PARAMETERS AND TRANSFORMATION MATRICES
             * Solid parameters, like position, size, rotation are redundant for computation,
                 and only cause more complications
             * They can be used as a simple user interface though, as it is easier to think about it that way,
                 and they can be recomputed inside the program to their matrix counterparts
            */
            //nhs1
            nhs1 = new(0.1, new((0, 0, 0), (0, 1, 0), 0, 2, 2));
            nhs1.Solids.Add(pFloor); nhs1.Solids.Add(Adam);
            nhs1.LightSources.Add(light);
            //hs1
            hs1 = new(0.1, new((0, 0, 0), (0, 1, 0), 0, 2, 2));
            hs1.LightSources.Add(light);
            Matrix4d t = Matrix4d.Transpose(Matrix4d.CreateTranslation(10, 50, -20));
            Matrix4d s = Matrix4d.Scale(10, 10, 10);
            Node adam = new(t * s, new List<Node>(), new Box(green, new(0, 0, 0), 1));

            Matrix4d pl = Matrix4d.Transpose(Matrix4d.CreateTranslation(0, 40, -25));
            Node floor = new(pl, new List<Node>(), new Plane(blu, new(0, 0, 0), new Vector3d(0, 0, -1).Normalized()));

            Matrix4d wt = new(1, 0, 0, 0,
                              0, 1, 0, 0,
                              0, 0, 1, 0,
                              0, 0, 0, 1);
            Node r1 = new(wt, new List<Node>(), null);
            r1.Nodes.Add(floor);
            r1.Nodes.Add(adam);
            hs1.Root = r1;
            //second set -----------------------------------------
            //nhs2
            nhs2 = new(0.1, new((0, 0, 0), (0, 1, 0), 0, 2, 2));
            Sphere s2r = new(red, new(0, 50, -20), 5);
            Sphere s2l = new(red, new(-2.6, 45, -23.5), 1.5);
            Box b2l = new Box(green, new Vector3d(10, 50, -20), 10);
            Box b2m = new(blu, new(12.5, 52.5, -12.5), 5);
            Plane f2 = new Plane(check, new(0, 40, -25), new Vector3d(0, 0, -1).Normalized());
            nhs2.Solids.Add(f2); nhs2.Solids.Add(b2l); nhs2.Solids.Add(b2m); nhs2.Solids.Add(s2r); nhs2.Solids.Add(s2l);
            nhs2.LightSources.Add(light);
            //hs2
            hs2 = new(0.1, new((0, 0, 0), (0, 1, 0), 0, 2, 2));
            hs2.LightSources.Add(light);

            r1 = new(wt, new List<Node>(), null);

            Matrix4d b1t = Matrix4d.Transpose(Matrix4d.CreateTranslation(10, 50, -20));
            Matrix4d b1s = Matrix4d.Scale(10);
            Box b1 = new(green, Vector3d.Zero, 1);
            Node nb1 = new(b1t * b1s, new(), b1);

            Matrix4d b2t = Matrix4d.Transpose(Matrix4d.CreateTranslation(7.5, 27.5, -2.5));
            Matrix4d b2s = Matrix4d.Scale(0.5);
            Box b2 = new(blu, Vector3d.Zero, 1);
            Node nb2 = new(b2t * b2s, new(), b2);
            nb1.Nodes.Add(nb2);

            Matrix4d s1t = Matrix4d.Transpose(Matrix4d.CreateTranslation(0,50,-20));
            Matrix4d s1s = Matrix4d.Scale(5);
            Sphere s1 = new(red, Vector3d.Zero, 1);
            Node ns1 = new(s1t * s1s, new(), s1);

            Matrix4d s2t = Matrix4d.Transpose(Matrix4d.CreateTranslation(-2.6, 45, -23.5));
            Matrix4d s2s = Matrix4d.Scale(1.5);
            Sphere s2 = new(red, Vector3d.Zero, 1);
            Node ns2 = new(s2t * s2s, new(), s2);

            Matrix4d pt = Matrix4d.Transpose(Matrix4d.CreateTranslation(0, 40, -25));
            Plane p = new(check, Vector3d.Zero, new(0, 0, -1));
            Node np = new(pt, new(), p);


            hs2.Root = r1;
            r1.Nodes.Add(nb1); r1.Nodes.Add(np); r1.Nodes.Add(ns1); r1.Nodes.Add(ns2);


            //testScene------------------------------------------------
            ths1 = new(0.1, new((0, 0, 0), (0, 1, 0), 0, 2, 2));
            ths1.LightSources.Add(light);

            r1 = new(wt, new List<Node>(), null);

            b1t = Matrix4d.Transpose(Matrix4d.CreateTranslation(10, 50, -20));
            b1s = Matrix4d.Scale(10);
            Matrix4d b1m = b1t* b1s;
            b1 = new(green, Vector3d.Zero, 1);
            nb1 = new(b1m, new(), null);

            b2t = Matrix4d.Transpose(Matrix4d.CreateTranslation(7.5, 27.5, -2.5));
            b2s = Matrix4d.Scale(0.5);
            Matrix4d b2mt = b2t * b2s;
            b2 = new(green, Vector3d.Zero, 1);
            nb2 = new(b2mt, new(), b2);
            nb1.Nodes.Add(nb2);

            pt = Matrix4d.Transpose(Matrix4d.CreateTranslation(0, 40, -25));
            p = new(blu, Vector3d.Zero, new(0, 0, -1));
            np = new(pt, new(), p);

            ths1.Root = r1;
            r1.Nodes.Add(nb1); r1.Nodes.Add(np);
        }

        private void _singleNonHierarchyTest(Scene scene, int sampleSize, string name)
        {
            Console.WriteLine("Testing " + name);
            FloatImage fi = new FloatImage(1200, 1200, 3); //todo: should parametrize into its own function
            int px = 1;         //modifies pixel size 

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

            string outputFile = name + ".pfm";
            string outputDir = "Output";

            fi.SavePFM(outputDir + "/" + outputFile);

            Console.WriteLine("HDR image is finished.");
            Console.WriteLine("File was saved as " + '"' + outputFile + '"' + " in the " + '"' + outputDir + '"' + " directory.");
        }
        private void _singleHierarchyTest(Scene scene, int sampleSize, string name)
        {
            Console.WriteLine("Testing " + name);
            FloatImage fi = new FloatImage(1200, 1200, 3); //todo: should parametrize into its own function
            int px = 1;         //modifies pixel size 

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

            string outputFile = name + ".pfm";
            string outputDir = "Output";

            fi.SavePFM(outputDir + "/" + outputFile);

            Console.WriteLine("HDR image is finished.");
            Console.WriteLine("File was saved as " + '"' + outputFile + '"' + " in the " + '"' + outputDir + '"' + " directory.");
        }

        public void Test()
        {
            int sampleSize = 1;

            _singleNonHierarchyTest(nhs1, sampleSize, "1nhs");
            _singleNonHierarchyTest(nhs2, sampleSize, "2nhs");
            _singleHierarchyTest(hs1, sampleSize, "1hs");
            _singleHierarchyTest(hs2, sampleSize, "2hs");

            //_singleHierarchyTest(ths1, sampleSize, "1ths");
            //_singleNonHierarchyTest(tnhs1, sampleSize, "1tnhs");
            return;
        }
    }
}
