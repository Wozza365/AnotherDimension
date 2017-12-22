using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Physics;

namespace Game.Sprites.Shapes
{
    /// <summary>
    /// Default Line sprite, does not collide by default, but can be easily implemented to do so
    /// </summary>
    public class Line : Sprite
    {
        public Color Colour { get; set; }
        public Line()
        {
            
        }
        public Line(Vector2 origin, Vector2 direction, Color colour, MainGame game, float friction)
        {
            Body = new Body(this)
            {
                Position = origin,
                Shape = Shape.Line,
                Origin = origin,
                Direction = direction,
                Friction = friction
            };
            Colour = colour;
            Game = game;
        }

        public override void Control()
        {
        }

        public override void Update()
        {
        }

        public override void Draw()
        {
            Vector2 edge = Body.Direction;
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            MainGame.SpriteBatch.Draw(MainGame.WhitePixel, new Microsoft.Xna.Framework.Rectangle((int)Body.Origin.X, (int)Body.Origin.Y, (int)edge.Length(), 1), null, Colour, angle, new Vector2(0, 0), SpriteEffects.None, 0);
        }

        public override void Collisions()
        {
        }
    }
}
