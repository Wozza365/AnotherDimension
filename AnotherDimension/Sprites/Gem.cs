using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Physics;

namespace Game.Sprites
{
    /// <summary>
    /// Gem used for the platformer, inherits the default box shape but overrides the draw and disables collisions on this side (the player will collide with it instead)
    /// </summary>
    public class Gem : Shapes.Box
    {
        public Gem(MainGame game, Texture2D tex, Rectangle texRect, Vector2 position, Vector2 size)
        {
            Game = game;
            Guid = Guid.NewGuid();
            Texture = tex;
            TextureRect = texRect;
            SpriteType = SpriteTypes.Gem;

            Body = new Body(this)
            {
                Gravity = Vector2.Zero,
                Bounce = Vector2.One,
                Mass = float.MaxValue,
                Enabled = true,
                Shape = Shape.Rectangle,
                Position = position,
                Width = size.X,
                Height = size.Y,
                Static = true,
                Friction = 1.0f,
                Guid = Guid
            };
        }

        public override void Collisions()
        {
            
        }

        public override void Draw()
        {
            MainGame.SpriteBatch.Draw(Texture, new Rectangle((int)Body.Position.X, (int)Body.Position.Y, (int)Body.Width, (int)Body.Height), TextureRect, Color.White);
        }
    }
}