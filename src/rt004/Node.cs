using OpenTK.Mathematics;
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
        public Matrix4d InvertedMatrix { get; set; }
        public List<Node> Nodes { get; set; }
        public List<ISolid> Solids { get; set; }
        public Node(Matrix4d transformationMatrix, Matrix4d invertedMatrix,  List<Node> nodes, List<ISolid> solids)
        {
            this.TransformationMatrix = transformationMatrix;
            this.InvertedMatrix = invertedMatrix;
            this.Nodes = nodes;
            this.Solids = solids;
        }
    }
}
