using OpenTK.Mathematics;
using rt004.Solids;
using rt004.Textures;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//file to have scene instances at one place. Used to compare outputs of hiearchical version and non-hiearchical version.
namespace rt004
{

    public class TestScenes
    {
        public Scene nhs1;
        public Scene? nhs2;
        public Scene? nhs3;

        public Scene hs1;
        public Scene? hs2;
        public Scene? hs3;

        public TestScenes()
        {

            Material blu = new Material(new Vector3d(0.3, 0.3, 1), 1, 0, 5, 0, 0); //color, diffusionCoeff, specularCoeff, glossiness 
            Material green = new Material(new Vector3d(0, 1, 0), 0.5, 0.5, 5, 0, 0);
            Material red = new Material(new Vector3d(1, 0, 0), 0.1, 0.9, 500, 0.2, 5);
            Material plush = new Material((1, 0.5, 0), 0.9, 0.1, 500, 0, 0);
            Material glass = new Material(new Vector3d(0.1, 0, 0), 1, 0, 5, 1, 1.05);
            Checkers check = new(new(1, 0, 0), 1, 0, 5, 0, 0, 3);
            PerlinNoise PeerlinNoise = new((0, 2, 0), 0.5, 0.1, 500, 0, 0);

            Plane pFloor = new Plane(check, new(0, 40, -25), new Vector3d(0, 0, -1).Normalized());
            Sphere spherocious = new Sphere(blu, new Vector3d(-25, -40, -5), 15); //material, origin, size
            Sphere spherocious2 = new Sphere(glass, new Vector3d(-20, 50, -5), 15);
            Sphere babz = new Sphere(green, new Vector3d(25, 40, -10), 10);
            Sphere Bleep = new Sphere(plush, new Vector3d(-10, 60, -5), 5);
            Box Adam = new Box(blu, new Vector3d(10, 50, -20), 10);

            SphericalLightSource light = new((-100, -50, 30), (1, 1, 1), 1, 50); //origin, direction, color, intensity, radius
            
            nhs1 = new(0.2, new((0, -10, -20), (0, 1, 0), 0, 2, 2));
            nhs1.Solids.Add(pFloor); //nhs1.Solids.Add(Adam);
            nhs1.LightSources.Add(light);

            hs1 = new(0.2, new((0, -10, -20), (0, 1, 0), 0, 2, 2));
            hs1.LightSources.Add(light);
            Node floor = new(Matrix4d.Identity, new List<Node>(), new Plane(check, new(0, 40, -25), new Vector3d(0, 0, -1).Normalized()));
            Node r1 = new(Matrix4d.Identity, new List<Node>(), null) ;
            r1.Nodes.Add(floor);
            hs1.Root = r1;


        }

    }
}
