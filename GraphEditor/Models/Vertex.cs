using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace GraphEditor.Models
{
    public class Vertex : VertexAndEdges
    {
        [XmlIgnore]
        public List<LineGeometry> connectingLine { get; set; }

        [XmlIgnore]
        public VertexModel vertexModel { get; set; }

        public string textInsideEllipse { get; set; }

        public List<int> counterOfLine { get; set; }

        public List<int> counterOfConnectedVertex { get; set; }

        public int numberOfConnection { get; set; }

        public int counterOfVertex { get; set; }
        public double positionX { get; set; }
        public double positionY { get; set; }

        public Vertex()
        {
            connectingLine = new List<LineGeometry>();

            vertexModel = new VertexModel();

            counterOfConnectedVertex = new List<int>();

            counterOfLine = new List<int>();

            numberOfConnection = 0;

            counterOfVertex = 0;
            positionX = 0;
            positionY = 0;
        }



    }
}
