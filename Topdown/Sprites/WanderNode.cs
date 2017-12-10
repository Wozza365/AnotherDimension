using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Topdown.Physics;

namespace Topdown.Sprites
{
    public class WanderNode : Sprite
    {
        public Vector2 Coordinate { get; set; }

        public WanderNode(TopdownGame game, Vector2 position, Vector2 size)
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
