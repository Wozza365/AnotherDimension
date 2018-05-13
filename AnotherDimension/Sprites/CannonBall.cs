using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Physics;

namespace Game.Sprites
{
    //A projectile cannonball, sent by the cannon and damages the player if hits
    public class CannonBall : Sprite
    {
        public CannonBall(MainGame game, Texture2D tex, Vector2 position, Vector2 size, Vector2 initialVelocity)
        {
            Game = game;
            Texture = tex;
            SpriteType = SpriteTypes.CannonBall;
            var vertices = new List<Vector2>
            {
                position,
                new Vector2(position.X + size.X, position.Y),
                new Vector2(position.X + size.X, position.Y + size.Y),
                new Vector2(position.X, position.Y + size.X)
            };
            
            Body = new Body(this)
            {
                Gravity = World.Gravity,
                Bounce = new Vector2(0.6f),
                Mass = float.MaxValue,
                Enabled = true,
                Shape = Shape.Circle,
                Position = position,
                Radius = size.X / 2,
                Static = false,
                Friction = 0.5f,
                Guid = Guid,
                Velocity = initialVelocity,
                MaxVelocity = initialVelocity,
                Rotation = 1
            };
        }

        public override void Control()
        {
        }

        public override void Update()
        {
            //Remove when its no longer moving
            if (Math.Abs(Body.Velocity.X) < 0.1f && Math.Abs(Body.Velocity.Y) < 0.1f)
            {
                MainGame.Sprites.Remove(this);
            }
        }

        public override void Draw()
        {
            MainGame.SpriteBatch.Draw(Texture, new Rectangle((int)(Body.Position.X), (int)(Body.Position.Y), (int)Body.Width, (int)Body.Height), Texture.Bounds, Color.White);
        }

        public override void Collisions()
        {
            Vector2 result = new Vector2();
            float distance = 0;
            for (var i = 0; i < MainGame.Sprites.Count; i++)
            {
                var s = MainGame.Sprites[i];
                //ignore some things like gems on screen and ladders
                if (s.SpriteType == SpriteTypes.CannonBall || s.SpriteType == SpriteTypes.Gem || s.SpriteType == SpriteTypes.Ladder || s.SpriteType == SpriteTypes.Cannon)
                    continue;
                if (!s.Equals(this) && World.Intersects(Body, s.Body, ref result, ref distance))
                {
                    World.Separate(Body, s.Body, ref result, ref distance);
                }
            }
        }
    }
}
