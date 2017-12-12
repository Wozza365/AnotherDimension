using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Topdown.Physics;

namespace Topdown.Sprites.Shapes
{
    public class Polygon : Sprite
    {
        private Color Colour;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="indices"></param>
        /// <param name="colour"></param>
        /// <param name="isStatic"></param>
        /// <param name="bounce"></param>
        /// <param name="friction">1 = 100% velocity retained. 0.4 = 40% retainted etc</param>
        /// <param name="gravityMultiplier"></param>
        public Polygon(TopdownGame game, List<Vector2> indices, Color colour, bool isStatic, Vector2 bounce, float friction = 1, float gravityMultiplier = 1)
        {
            Guid = Guid.NewGuid();
            Game = game;
            Colour = colour;
            Body = new Body(this)
            {
                Indices = indices,
                Gravity = World.Gravity * gravityMultiplier,
                Bounce = bounce,
                Enabled = true,
                Velocity = Vector2.Zero,
                Shape = Shape.Polygon,
                Static = isStatic,
                Friction = friction,
                Position = indices[0],
                Guid = Guid
            };
        }

        public Polygon()
        {
        }

        public override void Control()
        {
        }
            
        public override void Update()
        {
        }

        public override void Draw()
        {
            for (var i = 0; i < Body.Indices.Count; i++)
            {
                if (i == Body.Indices.Count - 1)
                {
                    Vector2 edge = Body.Indices.Last() - Body.Indices.First();
                    float angle = (float) Math.Atan2(edge.Y, edge.X);

                    TopdownGame.SpriteBatch.Draw(Game.WhitePixel,new Microsoft.Xna.Framework.Rectangle((int) Body.Indices.First().X, (int) Body.Indices.First().Y, (int) edge.Length(),1), null, Colour, angle, new Vector2(0, 0), SpriteEffects.None, 0);
                }
                else
                {
                    Vector2 edge = Body.Indices[i + 1] - Body.Indices[i];
                    float angle = (float)Math.Atan2(edge.Y, edge.X);

                    TopdownGame.SpriteBatch.Draw(Game.WhitePixel, new Microsoft.Xna.Framework.Rectangle((int)Body.Indices[i].X, (int)Body.Indices[i].Y, (int)edge.Length(), 1), null, Colour, angle, new Vector2(0, 0), SpriteEffects.None, 0);
                }
            }
        }

        public override void Collisions()
        {
            Vector2 result = new Vector2();
            float distance = 0;
            foreach (var s in TopdownGame.Sprites)
            {
                if (!s.Equals(this) && World.Intersects(Body, s.Body, ref result, ref distance))
                    World.Separate(Body, s.Body, ref result, ref distance);
            }
        }
    }
}
