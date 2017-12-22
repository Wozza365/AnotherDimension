using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Physics;

namespace Game.Sprites
{
    /// <summary>
    /// Lever connected to a cannon, player must switch it to turn off the cannon
    /// </summary>
    public class Lever : Sprite
    {
        public bool Activated { get; set; }
        public Cannon Cannon { get; set; }
        private Rectangle TextureRectangle2 { get; set; }
        public Lever(MainGame game, Texture2D tex, Rectangle texRect, Rectangle texRect2, Vector2 position, Vector2 size, Cannon cannon)
        {
            Game = game;
            Texture = tex;
            TextureRect = texRect;
            TextureRectangle2 = texRect2;
            Activated = true;
            Cannon = cannon;
            Guid = Guid.NewGuid();
            SpriteType = SpriteTypes.Lever;

            Body = new Body(this)
            {
                Gravity = Vector2.Zero,
                Bounce = Vector2.One,
                Mass = float.MaxValue,
                Enabled = true,
                Shape = Shape.Circle,
                Position = position,
                Radius = size.X /2,
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
            MainGame.SpriteBatch.Draw(Texture, new Rectangle((int)(Body.Position.X - Body.HalfWidth), (int)(Body.Position.Y - Body.HalfHeight), (int)Body.Width, (int)Body.Height), Activated ? TextureRect : TextureRectangle2, Color.White);
        }

        public override void Collisions()
        {
        }
    }
}
