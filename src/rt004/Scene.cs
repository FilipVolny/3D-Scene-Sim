using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static rt004.Program;

namespace rt004
{
    public class Scene
    {
        public double AmbientCoefficient { get; }
        public Camera Camera { get; }
        public Dictionary<string, Material> Materials { get; }
        public List<ISolid> Solids { get; }
        public List<LightSource> LightSources { get; }
        public Scene(double ambientCoefficient, Camera camera)
        {
            AmbientCoefficient = ambientCoefficient;
            Camera = camera;
            Solids = new List<ISolid>();
            LightSources = new List<LightSource>();
            Materials = new Dictionary<string, Material>();
        }
    }
}
