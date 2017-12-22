using System;
using Microsoft.Xna.Framework;
using Game.Physics;
using Game.Sprites.Shapes;

namespace Game.Sprites
{
    /// <summary>
    /// Overrides the collisions method of a standard polygon, as player collides with it
    /// Does not draw as it is drawn as part of the map
    /// </summary>
    public class Ladder : Polygon
    {
        public Ladder(MainGame game, Vector2 position, Vector2 size, bool isStatic, Vector2 bounce, float gravityMultiplier = 1)
        {
            Guid = Guid.NewGuid();
            Game = game;
            SpriteType = SpriteTypes.Ladder;
            Body = new Body(this)
            {
                Gravity = World.Gravity * gravityMultiplier,
                Bounce = bounce,
                Enabled = true,
                Velocity = Vector2.Zero,
                Shape = Shape.Rectangle,
                Static = isStatic,
                Friction = 1.0f,
                Position = position,
                Width = size.X,
                Height = size.Y,
                Guid = Guid
            };
        }

        public override void Collisions()
        {
            
        }
    }
}