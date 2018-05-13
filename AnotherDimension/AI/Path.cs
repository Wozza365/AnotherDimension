using System.Collections.Generic;

namespace Game.AI
{
    //Object returned from the A star algorithm
    public class Path
    {
        /// <summary>
        /// List of nodes to reach the destination
        /// </summary>
        public List<Node> Nodes { get; set; } = new List<Node>();
        /// <summary>
        /// Is the path valid, so has it actually found a path to the target
        /// </summary>
        public bool Valid { get; set; }
    }
}
