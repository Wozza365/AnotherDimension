using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Game.Physics;
using Game.Sprites.Shapes;

namespace Game.Sprites
{
    /// <summary>
    /// Map piece used for the top down part, is mostly just a simple polygon, but ignores collisions with a few objects.
    /// </summary>
    public class MapPiece : Polygon
    {
        public Color Colour { get; set; }
        public MapPiece(MainGame game, List<Vector2> vertices, Color colour, bool isStatic, Vector2 bounce, float friction = 1, float gravityMultiplier = 1)
        {
            Guid = Guid.NewGuid();
            Game = game;
            Colour = colour;
            Body = new Body(this)
            {
                Vertices = vertices,
                Gravity = World.Gravity * gravityMultiplier,
                Bounce = bounce,
                Enabled = true,
                Velocity = Vector2.Zero,
                Shape = Shape.Polygon,
                Static = isStatic,
                Friction = friction,
                Position = vertices[0],
                Guid = Guid
            };
        }

        public override void Collisions()
        {
            Vector2 result = new Vector2();
            float distance = 0;
            foreach (var s in MainGame.Sprites)
            {
                if (s.Body.Static)
                    continue;
                if (s.SpriteType == SpriteTypes.Bullet)
                    continue;
                if (s.SpriteType == SpriteTypes.Powerup)
                    continue;

                if (!s.Equals(this) && World.Intersects(Body, s.Body, ref result, ref distance))
                    World.Separate(Body, s.Body, ref result, ref distance);
            }
        }
    }
}