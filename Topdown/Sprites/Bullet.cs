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

    public enum WeaponTypes
    {
        RPG = 0,
        SMG = 1,
        Pistol = 2
    }

    public struct BulletConfig
    {
        public BulletTypes BulletType;
        public float MaxVelocity;
        public Texture2D Texture;
        public Rectangle DrawRectangle;
        public float InitialDamage;
        public float Bounce;
        public TimeSpan FireDelay;

        public BulletConfig(BulletTypes bulletType, float maxVelocity, Texture2D texture, Rectangle drawRectangle, float initialDamage, float bounce, TimeSpan fireDelay)
        {
            BulletType = bulletType;
            MaxVelocity = maxVelocity;
            Texture = texture;
            DrawRectangle = drawRectangle;
            InitialDamage = initialDamage;
            Bounce = bounce;
            FireDelay = fireDelay;
        }
    }
    
    public class Bullet : Sprite
    {
        public Rectangle DrawRectangle { get; set; }
        public Bullet(Vector2 position, Vector2 size, Vector2 direction, BulletConfig config)
        {
            direction.Normalize();
            Texture = config.Texture;
            TextureRect = config.Texture.Bounds;
            DrawRectangle = config.DrawRectangle;
            SpriteType = SpriteTypes.Bullet; 
            
            Body = new Body(this)
            {
                MaxVelocity = new Vector2(config.MaxVelocity),
                Velocity = direction * config.MaxVelocity,
                Position = position,
                Width = size.X,
                Height = size.Y,
                Radius = size.X/2,
                Direction = direction,
                Bounce = new Vector2(config.Bounce),
                Shape = Shape.Circle,
                Static = false,
                Enabled = true
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
            DrawRectangle = new Rectangle((int)Body.Position.X, (int)Body.Position.Y, DrawRectangle.Width, DrawRectangle.Height);
            TopdownGame.SpriteBatch.Draw(Texture, DrawRectangle, TextureRect, Color.White/* (float)Math.Atan2(Body.Direction.X, -Body.Direction.Y), new Vector2(TextureRect.Width / 2 + Body.HalfWidth, TextureRect.Height / 2 + Body.HalfHeight), SpriteEffects.None, 0*/);

        }

        public override void Collisions()
        {
        }
    }
}