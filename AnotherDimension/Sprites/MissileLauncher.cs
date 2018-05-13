using System;
using System.Collections.Generic;
using Game.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Sprites
{
    /// <summary>
    /// Launches AI controlled missiles, similar to the cannon
    /// </summary>
    public class MissileLauncher : Sprite
    {
        public bool Activated { get; set; }
        private TimeSpan FireFrequency { get; set; }
        private DateTime NextMissile { get; set; }
        private Random Random { get; set; } = new Random();

        public MissileLauncher(MainGame game, Texture2D tex, Vector2 position, Vector2 size, int frequency = 5)
        {
            Game = game;
            Texture = tex;
            Activated = true;
            FireFrequency = new TimeSpan(0, 0, 0, frequency);
            Guid = Guid.NewGuid();
            SpriteType = SpriteTypes.Cannon;
            var vertices = new List<Vector2>
            {
                position,
                new Vector2(position.X + size.X, position.Y),
                new Vector2(position.X + size.X, position.Y + size.Y),
                new Vector2(position.X, position.Y + size.X)
            };

            Body = new Body(this)
            {
                Gravity = Vector2.Zero,
                Bounce = Vector2.One,
                Mass = float.MaxValue,
                Enabled = true,
                Shape = Shape.Polygon,
                Vertices = vertices,
                Position = position,
                Width = size.X,
                Height = size.Y,
                Static = true,
                Friction = 1.0f,
                Guid = Guid
            };
        }

        public override void Control()
        {
            if (Activated && NextMissile <= DateTime.Now)
            {
                int xSpeed = Random.Next(-5, 5);
                Missile m = new Missile(Game, MainGame.CannonBall, new Vector2(Body.Centre.X, Body.Bottom + 10), new Vector2(20), new Vector2(xSpeed, 8));
                m.CreatePath();
                MainGame.Sprites.Add(m);
                NextMissile = DateTime.Now + FireFrequency;
            }
        }

        public override void Update()
        {
        }

        public override void Draw()
        {
            MainGame.SpriteBatch.Draw(Texture, new Rectangle((int)Body.Position.X, (int)Body.Position.Y, (int)Body.Width, (int)Body.Height), Color.White);
        }

        public override void Collisions()
        {
        }
    }
}