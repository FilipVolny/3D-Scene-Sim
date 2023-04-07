﻿using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    public class LightSource
    {
        public Vector3d Origin { get; }
        public Vector3d Direction { get; }
        public double Intensity { get; }
        public Vector3d Color { get; }
        public LightSource(Vector3d origin, Vector3d direction, Vector3d color, double intensity)
        {
            Origin = origin;
            Direction = direction.Normalized();
            Color = color;
            Intensity = intensity;
        }
    }
}
