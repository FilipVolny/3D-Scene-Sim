using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Util;
using OpenTK.Mathematics;
namespace rt004
{
    record ConfigItem(SceneItem scene, ImageItem imageConfig);

    record SceneItem(double ambientCoefficent, CameraItem camera, List<MaterialItem> materials, NodeItem node, List<LightSourceItem> lightSources);
    record CameraItem(List<double> origin, List<double> direction, double rotation, double width, double height );
    record MaterialItem(string name, List<double> color, double diffusionCoefficient, double specularCoefficient, double glossiness);
    record LightSourceItem(string type, List<double> direction, List<double> origin, List<double> color, double intensity);
    record ImageItem(string outputFile, string format, List<int> size);

    record SolidItem(string type, string materialName, List<double> origin, double size, List<double> normalVector);

    record NodeItem(List<List<double>> transformationMatrix, List<NodeItem> children, List<SolidItem> solids);

    internal static class JsonParser
    {
        internal static Vector3d ListToVector(List<double> dbl)
        {
            return new Vector3d(dbl[0], dbl[1], dbl[2]);
        }
        internal static Scene ParseJsonConfig(string fileName)
        {
            using FileStream json = File.OpenRead(fileName);
            ConfigItem config = JsonSerializer.Deserialize<ConfigItem>(json)!;
            
            //parse materials
            Dictionary<string, Material> materials = new Dictionary<string, Material>();
            foreach (var matJson in config.scene.materials)
            {
                materials.Add(matJson.name, new Material(ListToVector(matJson.color), matJson.diffusionCoefficient, matJson.specularCoefficient, matJson.glossiness));
            }
            //parse lights
            List<ILightSource> lightSources = new List<ILightSource>();
            foreach(var lgt in config.scene.lightSources)
            {
                if(lgt.type == "directional")
                {
                    lightSources.Add(new DirectionLightSource(ListToVector(lgt.direction), ListToVector(lgt.color), lgt.intensity));
                }
                if(lgt.type == "point")
                {
                    lightSources.Add(new DirectionLightSource(ListToVector(lgt.origin), ListToVector(lgt.color), lgt.intensity));
                }
            }
            //parse camera
            Camera camera = new Camera(ListToVector(config.scene.camera.origin), ListToVector(config.scene.camera.direction), config.scene.camera.rotation, config.scene.camera.width, config.scene.camera.height);

            //parse img config
            int imageWidth = config.imageConfig.size[0];
            int imageHeight = config.imageConfig.size[1];
            string fileOutput = config.imageConfig.outputFile + "." + config.imageConfig.format;

            //parse nodes
            Node root = CreateTree(materials,config.scene.node);

            Scene scene = new Scene(config.scene.ambientCoefficent, camera, materials, root, lightSources);
            return scene;
        }
        internal static Node CreateTree(Dictionary<string, Material> materials, NodeItem node)
        {
            Matrix4d transformationMatrix = new Matrix4d();
            for( int i = 0;  i < 4; i++ )
            {
                for( int j = 0; j < 4; j++ )
                {
                    transformationMatrix[i, j] = node.transformationMatrix[i][j];
                } 
            }

            Matrix4d inverseMatrix = Matrix4d.Invert(transformationMatrix);

            List<ISolid> solids = new List<ISolid>();
            foreach(var s in node.solids)
            {
                if( s.type == "sphere")
                {
                    solids.Add(new Sphere(materials[s.materialName], ListToVector(s.origin), s.size));
                }
                if( s.type == "plane" )
                {
                    solids.Add(new Plane(materials[s.materialName], ListToVector(s.origin), ListToVector(s.normalVector)));
                }
            }
            List<Node> children = new List<Node>();
            if( node.children  != null )
            {
                foreach (var n in node.children)
                {
                    children.Add(CreateTree(materials, n));
                }
            }
            return new Node(transformationMatrix, inverseMatrix, children, solids);
        }
    }
}
