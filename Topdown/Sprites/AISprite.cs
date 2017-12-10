using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topdown.AI;

namespace Topdown.Sprites
{
    public abstract class AISprite : Sprite
    {
        public Path CurrentPath { get; set; }
        public Node CurrentNode { get; set; }
        public Node NextNode { get; set; }
        public Node TargetNode { get; set; }
        public SpriteTypes TargetType { get; set; }
        public Sprite TargetSprite { get; set; }
    }
}
