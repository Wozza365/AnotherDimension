using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Topdown.AI
{
    public class Path
    {
        public List<Node> Nodes { get; set; } = new List<Node>();
        public bool Valid { get; set; }
    }
}
