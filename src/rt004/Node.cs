using OpenTK.Mathematics;
using rt004.Solids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt004
{
    public class Node
    {
        public Matrix4d TransformationMatrix { get; set; }
        public Matrix4d TransformationMatrixInverse { get; }
        public List<Node> Nodes { get; set; }
        public ISolid? Solid { get; set; }
        public Node(Matrix4d transformationMatrix, List<Node> nodes, ISolid? solid)
        {
            this.TransformationMatrix = transformationMatrix;
            this.TransformationMatrixInverse = transformationMatrix.Inverted();
            this.Nodes = nodes;
            this.Solid = solid;
        }
    }
}
