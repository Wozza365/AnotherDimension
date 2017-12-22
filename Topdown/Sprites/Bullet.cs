using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Physics;

namespace Game.Sprites
{
    public enum BulletTypes
    {
        Rocket = 1,
        Bullet = 2
    }

    /// <summary>
    /// Bullet config that carries various different stats for the bullet
    /// </summary>
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

    /// <summary>
    /// A simple bullet that damages an enemy
    /// </summary>
    public class Bullet : Sprite
    {
        public Rectangle DrawRectangle { get; set; }
        public BulletConfig BulletConfig { get; set; }
        public Bullet(Vector2 position, Vector2 size, Vector2 direction, BulletConfig config)
        {
            direction.Normalize();
            Texture = config.Texture;
            TextureRect = config.Texture.Bounds;
            DrawRectangle = config.DrawRectangle;
            SpriteType = SpriteTypes.Bullet;
            BulletConfig = config;

            Body = new Body(this)
            {
                MaxVelocity = new Vector2(config.MaxVelocity),
                Velocity = direction * config.MaxVelocity,
                Position = position,
                Width = size.X,
                Height = size.Y,
                Radius = size.X / 2,
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
            if (Body.Velocity.Length() < 0.5f)
            {
                MainGame.Sprites.Remove(this);
            }
        }

        public override void Draw()
        {
            DrawRectangle = new Rectangle((int)Body.Position.X, (int)Body.Position.Y, DrawRectangle.Width, DrawRectangle.Height);
            MainGame.SpriteBatch.Draw(Texture, DrawRectangle, TextureRect, Color.White/* (float)Math.Atan2(Body.Direction.X, -Body.Direction.Y), new Vector2(TextureRect.Width / 2 + Body.HalfWidth, TextureRect.Height / 2 + Body.HalfHeight), SpriteEffects.None, 0*/);

        }

        public override void Collisions()
        {
            Vector2 result = new Vector2();
            float distance = 0;
            foreach (var s in MainGame.Sprites)
            {
                if (s.SpriteType == SpriteTypes.Enemy)
                {
                    if (!s.Equals(this) && World.Intersects(Body, s.Body, ref result, ref distance))
                    {
                        var speed = Body.Velocity.Length();
                        ((Enemy)s).Health -= (int)(BulletConfig.InitialDamage * (speed / Body.MaxVelocity.X));
                        Body.Velocity = Vector2.Zero;
                        //MainGame.Sprites.Remove(this);
                    }
                }
            }
        }
    }
}