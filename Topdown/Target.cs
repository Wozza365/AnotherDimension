using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topdown.Sprites;

namespace Topdown
{
    public class Target
    {
        public int Weight { get; set; }
        public float Distance { get; set; }
        public SpriteTypes SpriteType { get; set; }
        public Sprite Sprite { get; set; }
    }
}
