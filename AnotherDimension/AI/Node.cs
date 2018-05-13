using System;
using Microsoft.Xna.Framework;

namespace Game.AI
{
    /// <summary>
    /// Node used for AI system
    /// </summary>
    public class Node : ICloneable
    {
        /// <summary>
        /// Total area of the node
        /// </summary>
        public Rectangle Area { get; set; }
        /// <summary>
        /// Simple co-ordinate of the node
        /// </summary>
        public Vector2 Coordinate { get; set; }
        /// <summary>
        /// Cost to traverse to the node, by default set to 10
        /// </summary>
        public int Cost { get; set; } = 10;
        public int F, G, H;

        /// <summary>
        /// Pointer to the parent node
        /// Used to loop back to the beginning of the list
        /// </summary>
        public Node Parent { get; set; }

        /// <summary>
        /// Centre of the Area
        /// </summary>
        public Vector2 Centre => new Vector2(Coordinate.X * 40 + 20, Coordinate.Y * 40 + 20);

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
