using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Physics;

namespace Game.Sprites
{
    /// <summary>
    /// Moving platform
    /// </summary>
    public class Platform : Sprite
    {
        public Vector2 MinimumPosition;
        public Vector2 MaximumPosition;
        public int Width { get; set; }
        public Platform(MainGame game, Vector2 position, Vector2 size, int width, Texture2D tex, Vector2 max)
        {
            Game = game;
            Texture = tex;
            Guid = Guid.NewGuid();
            SpriteType = SpriteTypes.Platform;
            MinimumPosition = position;
            MaximumPosition = max;
            Width = width;

            Vector2 vel = (max - position);
            vel.Normalize();

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
                Position = position,
                Width = size.X,
                Height = size.Y,
                Vertices = vertices,
                Static = false,
                Friction = 0.8f,
                Guid = Guid,
                Velocity = vel,
                MaxVelocity = new Vector2(1)
            };
        }
        public override void Control()
        {
            
        }

        public override void Update()
        {
            if (Body.Position.X < MinimumPosition.X)
            {
                Body.Velocity.X = 1;
            }
            if (Body.Position.X > MaximumPosition.X)
            {
                Body.Velocity.X = -1;
            }
            if (Body.Position.Y < MinimumPosition.Y)
            {
                Body.Velocity.Y = 1;
            }
            if (Body.Position.Y > MaximumPosition.Y)
            {
                Body.Velocity.Y = -1;
            }
        }

        public override void Draw()
        {
            var vertices = new List<Vector2>
            {
                Body.Position,
                new Vector2(Body.Position.X + Body.Width, Body.Position.Y),
                new Vector2(Body.Position.X + Body.Width, Body.Position.Y + Body.Height),
                new Vector2(Body.Position.X, Body.Position.Y + Body.Height)
            };
            for (var i = 0; i < vertices.Count; i++)
            {
                if (i == vertices.Count - 1)
                {
                    Vector2 edge = vertices.Last() - vertices.First();
                    float angle = (float)Math.Atan2(edge.Y, edge.X);

                    MainGame.SpriteBatch.Draw(MainGame.WhitePixel, new Rectangle((int)vertices.First().X, (int)vertices.First().Y, (int)edge.Length(), 1), null, Color.White, angle, new Vector2(0, 0), SpriteEffects.None, 0);
                }
                else
                {
                    Vector2 edge = vertices[i + 1] - vertices[i];
                    float angle = (float)Math.Atan2(edge.Y, edge.X);

                    MainGame.SpriteBatch.Draw(MainGame.WhitePixel, new Rectangle((int)vertices[i].X, (int)vertices[i].Y, (int)edge.Length(), 1), null, Color.White, angle, new Vector2(0, 0), SpriteEffects.None, 0);
                }
            }
            //MainGame.SpriteBatch.Draw(MainGame.WhitePixel, new Microsoft.Xna.Framework.Rectangle((int)Body.Position.X, (int)Body.Position.Y, (int)Body.Width, (int)Body.Height), Colour);
            //Draw platform with different tile from edges.
            MainGame.SpriteBatch.Draw(MainGame.SpriteSheet1, new Rectangle((int) Body.Position.X, (int) Body.Position.Y, 32, 32), new Rectangle(448, 256, 64, 64), Color.White);
            for (int i = 1; i < Width - 1; i++)
            {
                MainGame.SpriteBatch.Draw(MainGame.SpriteSheet1, new Rectangle((int)Body.Position.X + (i*32), (int)Body.Position.Y, 32, 32), new Rectangle(128, 192, 64, 64), Color.White);
            }
            MainGame.SpriteBatch.Draw(MainGame.SpriteSheet1, new Rectangle((int)Body.Position.X + ((Width-1) * 32), (int)Body.Position.Y, 32, 32), new Rectangle(512, 256, 64, 64), Color.White);
        }

        public override void Collisions()
        {
            
        }
    }
}