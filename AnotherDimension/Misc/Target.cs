using Game.Sprites;

namespace Game.Misc
{
    
    /// <summary>
    /// A target system generally to be used on top of the AI system rather than with it
    /// It's used for determining which target to calculate towards so that the relatively expensive A-Star algorithm is used as little as possible.
    /// </summary>
    public class Target
    {
        public int Weight { get; set; }
        public float Distance { get; set; }
        public SpriteTypes SpriteType { get; set; }
        public Sprite Sprite { get; set; }
    }
}
