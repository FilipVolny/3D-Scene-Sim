using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    public class Scene
    {
        public double AmbientCoefficient { get; }
        public Camera Camera { get; }
        public Dictionary<string, Material> Materials { get; }
        public List<ISolid> Solids { get; }
        public List<ILightSource> LightSources { get; }
        public Scene(double ambientCoefficient, Camera camera)
        {
            AmbientCoefficient = ambientCoefficient;
            Camera = camera;
            Solids = new List<ISolid>();
            LightSources = new List<ILightSource>();
            Materials = new Dictionary<string, Material>();
        }
        public Scene(string configFilePath)
        {

        }
    }
}
