using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Physics;

namespace Game.Sprites
{
    /// <summary>
    /// A cannon used on the platformer that fires a cannon every x seconds at a varying velocity
    /// </summary>
    public class Cannon : Sprite
    {
        public bool Activated { get; set; }
        private TimeSpan FireFrequency { get; }
        private DateTime NextCannonBall { get; set; }
        private Random Random { get; } = new Random();
        public Cannon(MainGame game, Texture2D tex, Vector2 position, Vector2 size, int frequency = 5)
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
        }

        public override void Update()
        {
            if (Activated && NextCannonBall <= DateTime.Now)
            {
                int xSpeed = Random.Next(5, 20);
                MainGame.Sprites.Add(new CannonBall(Game, MainGame.CannonBall, new Vector2(Body.Centre.X + 50, Body.Top), new Vector2(20), new Vector2(xSpeed, 8)));
                NextCannonBall = DateTime.Now + FireFrequency;
            }
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