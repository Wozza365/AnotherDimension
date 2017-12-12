using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Topdown.Physics;

namespace Topdown.Sprites
{
    public enum PowerupType
    {
        Ammo = 0,
        SMG = 1,
        RPG = 2,
        Speed = 3
    }

    public struct PowerupConfig
    {
        public PowerupType PowerupType;
        public Texture2D Texture;
        public Rectangle DrawRectangle;
        public float Ammo;
        public float Speed;

        public PowerupConfig(PowerupType powerupType, Texture2D texture, Rectangle drawRectangle, float ammo, float speed)
        {
            PowerupType = powerupType;
            Texture = texture;
            DrawRectangle = drawRectangle;
            Ammo = ammo;
            Speed = speed;
        }
    }
    
    public class Powerup : Sprite
    {
        public Rectangle DrawRectangle { get; set; }

        public PowerupConfig PowerupConfig { get; set; }
        public Powerup(TopdownGame game, Vector2 position, Vector2 size, PowerupConfig powerupConfig)
        {
            Game = game;
            PowerupConfig = powerupConfig;
            Guid = new Guid();
            SpriteType = SpriteTypes.Powerup;
            DrawRectangle = powerupConfig.DrawRectangle;

            Texture = game.WhitePixel;
            Body = new Body(this)
            {
                MaxVelocity = new Vector2(0),
                Enabled = true,
                Position = position,
                Radius = size.Y / 2,
                Game = game,
                Gravity = World.Gravity,
                Bounce = Vector2.Zero,
                Friction = 0,
                Static = true,
                Shape = Shape.Circle,
                Mass = 0
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
            TopdownGame.SpriteBatch.Draw(Texture, DrawRectangle, TextureRect, Color.White);
        }

        public override void Collisions()
        {
        }
    }
}