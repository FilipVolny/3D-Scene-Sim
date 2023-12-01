using OpenTK.Mathematics;
using rt004.Solids;
using rt004.Textures;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    public class Scene
    {
        public double AmbientCoefficient { get; }
        public Camera Camera { get; }
        public Dictionary<string, IMaterial> Materials { get; }
        public List<ISolid> Solids { get; }
        public List<ILightSource> LightSources { get; }
        public Node Root { get; }
        

        //Non hiearchical model
        public Scene(double ambientCoefficient, Camera camera)
        {
            AmbientCoefficient = ambientCoefficient;
            Camera = camera;
            Solids = new List<ISolid>();
            LightSources = new List<ILightSource>();
            Materials = new Dictionary<string, IMaterial>();
            // only here for the sake of backwards compatibility
            Root = new Node( new Matrix4d(), null, null);


        }
        
        //Hiearchical model (used for parsing, that's why the constructor has all the params, idk, maybe?)
        public Scene(double ambientCoefficient, Camera camera, Dictionary<string, IMaterial> materials, Node root, List<ILightSource> lightSources)
        {
            AmbientCoefficient = ambientCoefficient;
            Camera = camera;
            Materials = materials;
            LightSources = lightSources;
            Root = root;
            Solids = new();
        }
    }
}
