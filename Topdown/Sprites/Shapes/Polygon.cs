using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Physics;

namespace Game.Sprites.Shapes
{
    /// <summary>
    /// Default polygon sprite, customised a little bit, as it was used for map pieces of the top down map
    /// </summary>
    public class Polygon : Sprite
    {
        private Color Colour;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="vertices"></param>
        /// <param name="colour"></param>
        /// <param name="isStatic"></param>
        /// <param name="bounce"></param>
        /// <param name="friction">1 = 100% velocity retained. 0.4 = 40% retainted etc</param>
        /// <param name="gravityMultiplier"></param>
        public Polygon(MainGame game, List<Vector2> vertices, Color colour, bool isStatic, Vector2 bounce, float friction = 1, float gravityMultiplier = 1)
        {
            Guid = Guid.NewGuid();
            Game = game;
            Colour = colour;
            Body = new Body(this)
            {
                Vertices = vertices,
                Gravity = World.Gravity * gravityMultiplier,
                Bounce = bounce,
                Enabled = true,
                Velocity = Vector2.Zero,
                Shape = Shape.Polygon,
                Static = isStatic,
                Friction = friction,
                Position = vertices[0],
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
            for (var i = 0; i < Body.Vertices.Count; i++)
            {
                if (i == Body.Vertices.Count - 1)
                {
                    Vector2 edge = Body.Vertices.Last() - Body.Vertices.First();
                    float angle = (float) Math.Atan2(edge.Y, edge.X);

                    MainGame.SpriteBatch.Draw(MainGame.WhitePixel,new Rectangle((int) Body.Vertices.First().X, (int) Body.Vertices.First().Y, (int) edge.Length(),1), null, Colour, angle, new Vector2(0, 0), SpriteEffects.None, 0);
                }
                else
                {
                    Vector2 edge = Body.Vertices[i + 1] - Body.Vertices[i];
                    float angle = (float)Math.Atan2(edge.Y, edge.X);

                    MainGame.SpriteBatch.Draw(MainGame.WhitePixel, new Rectangle((int)Body.Vertices[i].X, (int)Body.Vertices[i].Y, (int)edge.Length(), 1), null, Colour, angle, new Vector2(0, 0), SpriteEffects.None, 0);
                }
            }
        }

        public override void Collisions()
        {
            Vector2 result = new Vector2();
            float distance = 0;
            foreach (var s in MainGame.Sprites)
            {
                if (s.SpriteType == SpriteTypes.Hero)
                    continue;
                if (s.SpriteType == SpriteTypes.CannonBall)
                    continue;
                if (!s.Equals(this) && World.Intersects(Body, s.Body, ref result, ref distance))
                    World.Separate(Body, s.Body, ref result, ref distance);
            }
        }
    }
}
