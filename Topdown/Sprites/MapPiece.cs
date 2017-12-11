using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Topdown.Physics;
using Topdown.Sprites.Shapes;

namespace Topdown.Sprites
{
    public class MapPiece : Polygon
    {
        public Color Colour { get; set; }
        public MapPiece(TopdownGame game, List<Vector2> indices, Color colour, bool isStatic, Vector2 bounce, float friction = 1, float gravityMultiplier = 1)
        {
            Guid = Guid.NewGuid();
            Game = game;
            Colour = colour;
            Body = new Body(this)
            {
                Indices = indices,
                Gravity = World.Gravity * gravityMultiplier,
                Bounce = bounce,
                Enabled = true,
                Velocity = Vector2.Zero,
                Shape = Shape.Polygon,
                Static = isStatic,
                Friction = friction,
                Position = indices[0],
                Guid = Guid
            };
        }

        public override void Collisions()
        {
            Vector2 result = new Vector2();
            float distance = 0;
            foreach (var s in TopdownGame.Sprites)
            {
                if (s.SpriteType == SpriteTypes.Bullet)
                    continue;

                if (!s.Equals(this) && World.Intersects(Body, s.Body, ref result, ref distance))
                    World.Separate(Body, s.Body, ref result, ref distance);
            }
        }
    }
}
