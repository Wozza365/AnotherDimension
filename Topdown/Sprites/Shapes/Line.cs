using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Topdown.Physics;

namespace Topdown.Sprites.Shapes
{
    public class Line : Sprite
    {
        public Color Colour { get; set; }
        public Line()
        {
            
        }
        public Line(Vector2 origin, Vector2 direction, Color colour, TopdownGame game, float friction)
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
            //throw new NotImplementedException();
        }

        public override void Update()
        {
            //throw new NotImplementedException();
        }

        public override void Draw()
        {
            Vector2 edge = Body.Direction;
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            TopdownGame.SpriteBatch.Draw(Game.WhitePixel, new Microsoft.Xna.Framework.Rectangle((int)Body.Origin.X, (int)Body.Origin.Y, (int)edge.Length(), 1), null, Colour, angle, new Vector2(0, 0), SpriteEffects.None, 0);
        }

        public override void Collisions()
        {
            //throw new NotImplementedException();
        }
    }
}
