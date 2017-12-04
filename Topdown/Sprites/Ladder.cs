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
    public class Ladder : Polygon
    {
        public Ladder(TopdownGame game, List<Vector2> indices, Color colour, bool isStatic, Vector2 bounce, float gravityMultiplier = 1)
        {
            Guid = Guid.NewGuid();
            Game = game;
            Body = new Body(this)
            {
                Indices = indices,
                Gravity = World.Gravity * gravityMultiplier,
                Bounce = bounce,
                Enabled = true,
                Velocity = Vector2.Zero,
                Shape = Shape.Polygon,
                Static = isStatic,
                Friction = 1.0f,
                Position = indices[0],
                Guid = Guid
            };
        }

        public override void Draw()
        {
            
        }

        public override void Collisions()
        {
            
        }
    }
}
