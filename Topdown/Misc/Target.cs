using Topdown.Sprites;

namespace Topdown.Misc
{
    public class Target
    {
        public int Weight { get; set; }
        public float Distance { get; set; }
        public SpriteTypes SpriteType { get; set; }
        public Sprite Sprite { get; set; }
    }
}
