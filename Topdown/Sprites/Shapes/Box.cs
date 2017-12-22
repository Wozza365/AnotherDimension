using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Physics;

namespace Game.Sprites.Shapes
{
    /// <summary>
    /// Default AABB rectangle shape with default collision response
    /// Draws just a simple outline of the box on screen.
    /// </summary>
    public class Box: Sprite
    {
        private Color Colour { get; }
        public Box() { }
        public Box(MainGame game, float x, float y, float w, float h, float m, Color c, bool isStatic, Vector2 bounce, float gravityMultiplier = 1)
        {
            Guid = Guid.NewGuid();
            Game = game;
            Colour = c;
            List<Vector2> vertices = new List<Vector2>
            {
                new Vector2(x,y),
                new Vector2(x+w, y),
                new Vector2(x+w, y+h),
                new Vector2(x, y+h)
            };
            Body = new Body(this)
            {
                Gravity = World.Gravity * gravityMultiplier,
                Bounce = bounce,
                Mass = m,
                Enabled = true,
                Shape = Shape.Polygon,
                Position = new Vector2(x, y),
                //Width = w,
                //Height = h,
                Static = isStatic,
                Friction = 1.0f,
                Vertices = vertices,
                Guid = Guid
            };
        }

        public override void Collisions()
        {
            //Test every sprite and resolve them
            Vector2 result = new Vector2();
            float distance = 0;
            foreach (var s in MainGame.Sprites)
            {
                if (!s.Equals(this) && World.Intersects(Body, s.Body, ref result, ref distance))
                    World.Separate(Body, s.Body, ref result, ref distance);
            }
        }

        public override void Control()
        {
            //throw new NotImplementedException();
        }

        //Draw just a rectangle outline
        public override void Draw()
        {
            for (var i = 0; i < Body.Vertices.Count; i++)
            {
                if (i == Body.Vertices.Count - 1)
                {
                    Vector2 edge = Body.Vertices.Last() - Body.Vertices.First();
                    float angle = (float)Math.Atan2(edge.Y, edge.X);

                    MainGame.SpriteBatch.Draw(MainGame.WhitePixel, new Microsoft.Xna.Framework.Rectangle((int)Body.Vertices.First().X, (int)Body.Vertices.First().Y, (int)edge.Length(), 1), null, Colour, angle, new Vector2(0, 0), SpriteEffects.None, 0);
                }
                else
                {
                    Vector2 edge = Body.Vertices[i + 1] - Body.Vertices[i];
                    float angle = (float)Math.Atan2(edge.Y, edge.X);

                    MainGame.SpriteBatch.Draw(MainGame.WhitePixel, new Microsoft.Xna.Framework.Rectangle((int)Body.Vertices[i].X, (int)Body.Vertices[i].Y, (int)edge.Length(), 1), null, Colour, angle, new Vector2(0, 0), SpriteEffects.None, 0);
                }
            }
            MainGame.SpriteBatch.Draw(MainGame.WhitePixel, new Microsoft.Xna.Framework.Rectangle((int)Body.Position.X, (int)Body.Position.Y, (int)Body.Width, (int)Body.Height), Colour);
        }

        public override void Update()
        {
            
        }
    }
}