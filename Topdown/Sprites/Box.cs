using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Physics;
using Game.Sprites.Shapes;

namespace Game.Sprites
{
    /// <summary>
    /// In-game box, separate to the default Box shape
    /// Uses circle for best compatibility across all different shapes
    /// AABB against polygon collisions is not supported
    /// </summary>
    public class Box : Polygon
    {
        public Box(MainGame game, Texture2D tex, Vector2 position, float radius, Vector2 bounce, float friction, float gravityMultiplier = 1)
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

        public override void Collisions()
        {
            Vector2 result = new Vector2();
            float distance = 0;
            foreach (var s in MainGame.Sprites)
            {
                if (s.SpriteType == SpriteTypes.PlatformerHero)
                    continue;
                if (s.SpriteType == SpriteTypes.CannonBall)
                    continue;
                if (s.SpriteType == SpriteTypes.Gem)
                    continue;
                if (!s.Equals(this) && World.Intersects(Body, s.Body, ref result, ref distance))
                    World.Separate(Body, s.Body, ref result, ref distance);
            }
        }

        public override void Draw()
        {
            MainGame.SpriteBatch.Draw(Texture, new Rectangle((int)(Body.Centre.X - Body.Radius), (int)(Body.Centre.Y - Body.Radius), (int)Body.Width, (int)Body.Width), Color.White);
            //MainGame.SpriteBatch.Draw(Circle.DefaultTexture, new Rectangle((int)(Body.Centre.X - Body.Radius), (int)(Body.Centre.Y - Body.Radius), (int)Body.Width, (int)Body.Width), Color.White);
        }
    }
}