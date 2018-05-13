using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Physics;

namespace Game.Sprites
{

    /// <summary>
    /// A fan like sprite that pushes the player in the air and allows them to float in a certain region if balanced correctly
    /// </summary>
    public class Turbine : Sprite
    {
        public float FanSpeed { get; set; }
        public Body AffectZone { get; set; }
        public Turbine(MainGame game, Texture2D tex, Vector2 position, Vector2 size, float speed, int height)
        {
            FanSpeed = speed;
            Game = game;
            Texture = tex;
            Guid = Guid.NewGuid();
            Body = new Body(this)
            {
                Gravity = Vector2.Zero,
                Bounce = Vector2.One,
                Mass = float.MaxValue,
                Enabled = false,
                Shape = Shape.Rectangle,
                Position = position,
                Width = size.X,
                Height = size.Y,
                Static = true,
                Friction = 1.0f,
                Guid = Guid
            };
            AffectZone = new Body(this)
            {
                Gravity = Vector2.Zero,
                Bounce = Vector2.One,
                Mass = float.MaxValue,
                Enabled = true,
                Shape = Shape.Rectangle,
                Position = new Vector2(position.X, position.Y - height),
                Width = size.X,
                Height = height,
                Static = true,
                Friction = 1.0f,
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
            MainGame.SpriteBatch.Draw(Texture, new Rectangle((int)Body.Position.X, (int)Body.Position.Y, (int)Body.Width, (int)Body.Height), Color.White);
        }

        public override void Collisions()
        {
            float d = 0;
            Vector2 s = Vector2.One;
            var collidable = MainGame.Sprites.Where(x => x.SpriteType == SpriteTypes.PlatformerHero || x.SpriteType == SpriteTypes.Box);
            //Uses the AffectZone region
            //Calculates a distance from the source
            //And then applies a fraction of the max velocity based on the distance.
            //So it will have a very small affect at the top of the fan
            foreach (var c in collidable)
            {
                if (World.Intersects(c.Body, AffectZone, ref s, ref d))
                {
                    var distance = Body.Position.Y - c.Body.Position.Y;
                    var strength = distance / AffectZone.Height;
                    c.Body.Velocity.Y -= FanSpeed * strength;
                }
            }
        }
    }
}