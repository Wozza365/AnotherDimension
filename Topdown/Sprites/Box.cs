using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Topdown.Physics;
using Topdown.Sprites.Shapes;

namespace Topdown.Sprites
{
    public class Box : Polygon
    {
        public Box(TopdownGame game, Texture2D tex, Vector2 position, float radius, Vector2 bounce, float friction, float gravityMultiplier = 1)
        {
            Game = game;
            Guid = new Guid();
            SpriteType = SpriteTypes.Box;
            Texture = tex;
            
            Body = new Body(this)
            {
                MaxVelocity = new Vector2(3),
                Enabled = true,
                Static = false,
                Position = position,
                Radius = radius,
                Game = game,
                Gravity = World.Gravity * gravityMultiplier,
                Bounce = bounce,
                Friction = friction,
                Shape = Shape.Circle,
                Mass = 50
            };
        }

        public override void Draw()
        {
            TopdownGame.SpriteBatch.Draw(Texture, new Rectangle((int)(Body.Centre.X - Body.Radius), (int)(Body.Centre.Y - Body.Radius), (int)Body.Width, (int)Body.Width), Color.White);
            TopdownGame.SpriteBatch.Draw(Circle.DefaultTexture, new Rectangle((int)(Body.Centre.X - Body.Radius), (int)(Body.Centre.Y - Body.Radius), (int)Body.Width, (int)Body.Width), Color.White);
        }
    }
}