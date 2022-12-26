using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEditor.Models
{
    public class VertexAndEdges : MainWindow
    {
        public List<Vertex> listOfVertex { get; set; }
        public List<Edges> listOfEdges { get; set; }

        public VertexAndEdges()
        {
            listOfVertex = new List<Vertex>();
            listOfEdges = new List<Edges>();
        }
    }
}
