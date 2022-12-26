using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace GraphEditor.Models
{
    public class VertexModel : Vertex
    {
        public TextBlock textInsideVertex { get; set; }
        public VertexModel()
        {
            textInsideVertex = new TextBlock();
        }
    }
}
