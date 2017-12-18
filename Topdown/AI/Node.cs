using System;
using Microsoft.Xna.Framework;

namespace Topdown.AI
{
    public class Node : ICloneable
    {
        public Rectangle Area { get; set; }
        public Vector2 Coordinate { get; set; }
        public int Cost { get; set; } = 10;
        public int F, G, H;
        public Node Parent { get; set; }

        public Vector2 Centre => new Vector2(Coordinate.X * 40 + 20, Coordinate.Y * 40 + 20);

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
