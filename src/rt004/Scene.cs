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
        public double AmbientCoeficient { get; }
        public Camera Camera { get; }
        public Dictionary<string, Material> Materials { get; }
        public List<ISolid> Solids { get; }
        public List<LightSource> LightSources { get; }
        public Scene(double ambientCoeficient, Camera camera, List<ISolid> solids, List<LightSource> lightSources)
        {
            AmbientCoeficient = ambientCoeficient;
            Camera = camera;
            Solids = solids;
            LightSources = lightSources;
            Materials = new Dictionary<string, Material>();
        }
    }
}
