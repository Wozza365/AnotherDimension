using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Topdown.Physics;

namespace Topdown.Sprites
{
    public enum BulletTypes
    {
        Rocket = 1,
        Bullet = 2
    }

    public struct BulletConfig
    {
        public float MaxVelocity;
        public Texture2D Texture;
        public Rectangle DrawRectangle;
        public float InitialDamage;
        public float Bounce;
    }
    
    public class Bullet : Sprite
    {
        public Rectangle DrawRectangle { get; set; }
        public Bullet(Vector2 position, Vector2 size, Vector2 direction, BulletConfig config)
        {
            direction.Normalize();
            Texture = config.Texture;
            
            Body = new Body(this)
            {
                Velocity = direction * config.MaxVelocity,
                Position = position,
                Width = size.X,
                Height = size.Y,
                Radius = size.X/2,
                Direction = direction,
                Bounce = new Vector2(config.Bounce),
                Shape = Shape.Circle
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
