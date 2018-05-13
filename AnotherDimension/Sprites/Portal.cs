using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Physics;

namespace Game.Sprites
{
    /// <summary>
    /// In-game object for teleporting between dimensions
    /// </summary>
    public class Portal : Sprite
    {
        public bool Activated { get; set; }
        public Portal(MainGame game, Texture2D tex, Rectangle texRect, Vector2 position, Vector2 size)
        {
            Game = game;
            Texture = tex;
            TextureRect = texRect;
            Activated = false;
            Guid = Guid.NewGuid();
            SpriteType = SpriteTypes.Portal;

            Body = new Body(this)
            {
                Gravity = Vector2.Zero,
                Bounce = Vector2.One,
                Mass = float.MaxValue,
                Enabled = false,
                Shape = Shape.Rectangle,
                Position = position,
                Width = size.X,
                Height = size.Y,
                Static = true,
                Friction = 0.8f,
                Guid = Guid
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
            if (Activated)
            {
                MainGame.SpriteBatch.Draw(Texture, new Rectangle((int)(Body.Position.X), (int)(Body.Position.Y), (int)Body.Width, (int)Body.Height), TextureRect, Color.White);
            }
        }

        public override void Collisions()
        {

        }
    }
}