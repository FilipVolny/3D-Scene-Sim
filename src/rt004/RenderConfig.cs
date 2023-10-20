using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{


    internal class RenderConfig
    {
        public Scene scene;
        public int imageWidth = 800; //max is the widest monitor
        public int imageHeight = 600; //max is the highest monitor
        public int resolution;
        public string configSaveFile; // just the name
        public string outputFile;   //just the name
        public string outputFileFormat; //.pfm or .jpg, nothing else is supported yet
        public string outputFilePath;
        public int samplePerPixel = 42;
        public bool antiAliasing = true;
        public bool softShadows = true;
        public bool materials = true;
        public bool reflections = true;
        public bool refractions = true;
        public bool glossySurfaces = true;

        public RenderConfig(Scene scene, int width, int height)
        {
            this.scene = scene;
            this.imageWidth = width;
            this.imageHeight = height;
        }
    }
}
