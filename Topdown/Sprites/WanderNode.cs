using System;
using Microsoft.Xna.Framework;
using Game.Physics;

namespace Game.Sprites
{
    /// <summary>
    /// A simple node on the map
    /// Used for zombies to have a target to casually wander to
    /// </summary>
    public class WanderNode : Sprite
    {
        public Vector2 Coordinate { get; set; }

        public WanderNode(MainGame game, Vector2 position, Vector2 size)
        {
            Game = game;
            Guid = new Guid();
            SpriteType = SpriteTypes.WanderNode;
            

            Body = new Body(this)
            {
                Enabled = false,
                Game = game,
                Guid = Guid,
                Gravity = Vector2.Zero,
                Bounce = Vector2.Zero,
                Velocity = Vector2.Zero,
                Shape = Shape.Circle,
                Static = true,
                Position = position,
                Width = size.X,
                Height = size.Y
            };
        }
        public override void Control()
        {
            
        }

        public override void Update()
        {
            
        }

        public override void Draw()
        {

        }

        public override void Collisions()
        {

        }
    }
}
