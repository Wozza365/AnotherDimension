using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Physics;

namespace Game.Sprites
{
    /// <summary>
    /// Types of powerups
    /// </summary>
    public enum PowerupType
    {
        Ammo = 0,
        SMG = 1,
        RPG = 2,
        Speed = 3,
        Health = 4
    }

    /// <summary>
    /// Configs for powerups stats
    /// Would make it easier to load in from a file
    /// </summary>
    public struct PowerupConfig
    {
        public PowerupType PowerupType { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle DrawRectangle { get; set; }
        public int Ammo { get; set; }
        public float Speed { get; set; }
        public TimeSpan Duration { get; set; }
        public float Health { get; set; }

        public PowerupConfig(PowerupType powerupType, Texture2D texture, Rectangle drawRectangle, int ammo, float speed, TimeSpan duration, float health)
        {
            PowerupType = powerupType;
            Texture = texture;
            DrawRectangle = drawRectangle;
            Ammo = ammo;
            Speed = speed;
            Duration = duration;
            Health = health;
        }
    }
    
    //A powerup on the map
    public class Powerup : Sprite
    {
        public Rectangle DrawRectangle { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public PowerupConfig PowerupConfig { get; set; }
        public Powerup(MainGame game, Vector2 position, Vector2 size, PowerupConfig powerupConfig)
        {
            Game = game;
            PowerupConfig = powerupConfig;
            Guid = new Guid();
            SpriteType = SpriteTypes.Powerup;
            DrawRectangle = powerupConfig.DrawRectangle;
            Duration = powerupConfig.Duration;
            StartTime = DateTime.Now;

            Texture = powerupConfig.Texture;
            Body = new Body(this)
            {
                MaxVelocity = Vector2.Zero,
                Velocity = Vector2.Zero,
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
            //Remove when expired
            if (DateTime.Now > StartTime + Duration)
            {
                MainGame.Sprites.Remove(this);
            }
        }

        public override void Draw()
        {
            DrawRectangle = new Rectangle((int)Body.Position.X, (int)Body.Position.Y, DrawRectangle.Width, DrawRectangle.Height);
            MainGame.SpriteBatch.Draw(Texture, DrawRectangle, Texture.Bounds, Color.White);
        }

        public override void Collisions()
        {
        }
    }
}