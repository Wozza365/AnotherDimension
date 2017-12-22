using Game.AI;

namespace Game.Sprites
{
    //An extra layer used for sprites controlled by the AStar algorithm
    //Has a target, path etc
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
