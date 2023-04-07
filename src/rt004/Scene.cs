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
        public double AmbientCoeficient { get; set; }
        public Camera Camera { get; set; }
        public Dictionary<string, Material> Materials { get; set; }
        public List<ISolid> Solids { get; set; }
        public List<LightSource> LightSources { get; set; }
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
