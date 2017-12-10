using System.Collections.Generic;

namespace Topdown.AI
{
    public class Path
    {
        public List<Node> Nodes { get; set; } = new List<Node>();
        public bool Valid { get; set; }
    }
}
