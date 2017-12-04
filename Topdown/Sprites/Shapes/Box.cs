using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topdown;
using Topdown.Physics;

namespace Topdown.Sprites.Shapes
{
    public class Box: Sprite
    {
        private Color Colour { get; set; }
        public Box(TopdownGame game, float x, float y, float w, float h, float m, Color c, bool isStatic, Vector2 bounce, float gravityMultiplier = 1)
        {
            Guid = Guid.NewGuid();
            Game = game;
            Colour = c;
            List<Vector2> indices = new List<Vector2>
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
                Indices = indices,
                Guid = Guid
            };
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

        public override void Control()
        {
            //throw new NotImplementedException();
        }

        public override void Draw()
        {
            for (var i = 0; i < Body.Indices.Count; i++)
            {
                if (i == Body.Indices.Count - 1)
                {
                    Vector2 edge = Body.Indices.Last() - Body.Indices.First();
                    float angle = (float)Math.Atan2(edge.Y, edge.X);

                    TopdownGame.SpriteBatch.Draw(Game.WhitePixel, new Microsoft.Xna.Framework.Rectangle((int)Body.Indices.First().X, (int)Body.Indices.First().Y, (int)edge.Length(), 1), null, Colour, angle, new Vector2(0, 0), SpriteEffects.None, 0);
                }
                else
                {
                    Vector2 edge = Body.Indices[i + 1] - Body.Indices[i];
                    float angle = (float)Math.Atan2(edge.Y, edge.X);

                    TopdownGame.SpriteBatch.Draw(Game.WhitePixel, new Microsoft.Xna.Framework.Rectangle((int)Body.Indices[i].X, (int)Body.Indices[i].Y, (int)edge.Length(), 1), null, Colour, angle, new Vector2(0, 0), SpriteEffects.None, 0);
                }
            }
            TopdownGame.SpriteBatch.Draw(Game.WhitePixel, new Microsoft.Xna.Framework.Rectangle((int)Body.Position.X, (int)Body.Position.Y, (int)Body.Width, (int)Body.Height), Colour);
        }

        public override void Update()
        {
            
        }
    }
}