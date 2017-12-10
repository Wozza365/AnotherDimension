using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Topdown.AI
{
    public class Node
    {
        public Rectangle Area { get; set; }
        public Vector2 Coordinate { get; set; }
        public int Cost { get; set; } = 10;
        public int F, G, H;
        public Node Parent { get; set; }
    }
}
