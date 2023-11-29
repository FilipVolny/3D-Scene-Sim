using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Util;
using OpenTK.Mathematics;
using System.Diagnostics.Tracing;
using rt004.Solids;
using rt004.Textures;

namespace rt004
{
    // !!!!!!
    //      TODO parser cant parse textures
    //      must be able to save to a JSON as well
    // !!!!!!

    record ConfigItem(SceneItem scene, ImageItem imageConfig);
    record SceneItem(double ambientCoefficient, CameraItem camera, List<MaterialItem> materials, List<NodeItem> nodes, List<LightSourceItem> lightSources);
    record CameraItem(List<double> origin, List<double> direction, double rotation, double width, double height );
    record MaterialItem(string name, string type, List<double> color, double diffusionCoefficient, double specularCoefficient, double glossiness, double transparency, double refractiveIndex);
    record LightSourceItem(string type, List<double> direction, List<double> origin, List<double> color, double intensity, double size);
    record ImageItem(string outputFile, string format, List<int> size);
    record SolidItem(string type, string materialName, List<double> origin, double size, List<double> normalVector);
    record NodeItem(List<List<double>> transformationMatrix, List<NodeItem> children, SolidItem solid);
    //-------------------------------------

    internal static class JsonParser
    {
        internal static Vector3d ListToVector(List<double> dbl)
        {
            return new Vector3d(dbl[0], dbl[1], dbl[2]);
        }
        internal static Scene ParseJsonConfig(string fileName)
        {
            using FileStream json = File.OpenRead(fileName);
            ConfigItem configJson = JsonSerializer.Deserialize<ConfigItem>(json)!;

            //parse materials
            Dictionary<string, IMaterial> materials = new Dictionary<string, IMaterial>
            {
                { "default", new Material(new Vector3d(0.9, 0.9, 0.9), 1, 0, 0, 0, 1) }
            };

            foreach (var matJson in configJson.scene.materials)
            {
                if(matJson.type == "material")
                {
                    materials.Add(matJson.name, new Material(ListToVector(matJson.color), matJson.diffusionCoefficient, matJson.specularCoefficient, matJson.glossiness, matJson.transparency, matJson.refractiveIndex));
                }
                else if(matJson.type == "noise")
                {
                    materials.Add(matJson.name, new PerlinNoise(ListToVector(matJson.color), matJson.diffusionCoefficient, matJson.specularCoefficient, matJson.glossiness, matJson.transparency, matJson.refractiveIndex));
                }
                else if(matJson.type == "texture")
                {
                    materials.Add(matJson.name, new Texture(ListToVector(matJson.color), matJson.diffusionCoefficient, matJson.specularCoefficient, matJson.glossiness, matJson.transparency, matJson.refractiveIndex));
                }

            }
            //parse lights
            List<ILightSource> lightSources = new List<ILightSource>();
            foreach(var lgtJson in configJson.scene.lightSources)
            {
                if(lgtJson.type == "directional")
                {
                    lightSources.Add(new DirectionalLightSource(ListToVector(lgtJson.direction), ListToVector(lgtJson.color), lgtJson.intensity));
                }
                else if(lgtJson.type == "point")
                {
                    lightSources.Add(new PointLightSource(ListToVector(lgtJson.origin), ListToVector(lgtJson.color), lgtJson.intensity));
                }
                else if(lgtJson.type == "spherical")
                {
                    lightSources.Add(new SphericalLightSource(ListToVector(lgtJson.origin), ListToVector(lgtJson.color), lgtJson.intensity, lgtJson.size));
                }
            }
            //parse camera
            Camera camera = new Camera(ListToVector(configJson.scene.camera.origin), ListToVector(configJson.scene.camera.direction), configJson.scene.camera.rotation, configJson.scene.camera.width, configJson.scene.camera.height);

            //parse img config --- nothing is done with this at the moment
            int imageWidth = configJson.imageConfig.size[0];
            int imageHeight = configJson.imageConfig.size[1];
            string fileOutput = configJson.imageConfig.outputFile + "." + configJson.imageConfig.format;

            //parse nodes
            Node root = new Node(Matrix4d.Identity, new List<Node>(), null);
            foreach(var node in configJson.scene.nodes)
            {
                root.Nodes.Add(CreateTree(materials,node, null));
            }

            Scene scene = new Scene(configJson.scene.ambientCoefficient, camera, materials, root, lightSources);
            return scene;
        }
        public static Node CreateTree(Dictionary<string, IMaterial> materials, NodeItem nodeJson, Node? parent)
        {
            Matrix4d transformationMatrix = new Matrix4d();
            for( int i = 0;  i < 4; i++ )
            {
                for( int j = 0; j < 4; j++ )
                {
                    transformationMatrix[i, j] = nodeJson.transformationMatrix[i][j];
                } 
            }

            ISolid? solid = null; //for the sake of testing transformation order
            if(nodeJson.solid is not null)
            {
                if (nodeJson.solid.type == "sphere")
                {
                    solid = (new Sphere(materials[nodeJson.solid.materialName], ListToVector(nodeJson.solid.origin), nodeJson.solid.size));
                }
                else if (nodeJson.solid.type == "plane")
                {
                    solid = (new Plane(materials[nodeJson.solid.materialName], ListToVector(nodeJson.solid.origin), ListToVector(nodeJson.solid.normalVector)));
                }
                else if (nodeJson.solid.type == "cube")
                {
                    solid = (new Box(materials[nodeJson.solid.materialName], ListToVector(nodeJson.solid.origin), nodeJson.solid.size));
                }
            }
            Node currentNode = new Node(transformationMatrix, new List<Node>(), solid);
            List<Node> children = new List<Node>();
            if( nodeJson.children != null )
            {
                foreach (var n in nodeJson.children)
                {
                    children.Add(CreateTree(materials, n, currentNode));
                }
            }
            currentNode.Nodes = children;
            return currentNode;
        }
    }
}
