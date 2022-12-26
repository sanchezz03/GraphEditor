using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace GraphEditor.Models
{
    public class Edges : VertexAndEdges
    {
        [XmlIgnore]
        public List<Path> listOfPath { get; set; }
        [XmlIgnore]
        public int numberOfLines { get; set; }
        public int counterOfLine { get; set; }
        public Edges()
        {
            listOfPath = new List<Path>();
            counterOfLine = 0;
            numberOfLines = 0;
        }


        //public Path GetPathForDelete(int _counterOfLines)
        //{
        //    return listOfPath[numberOfLines - 1];
        //}

    }
}
