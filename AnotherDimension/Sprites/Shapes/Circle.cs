using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Other;
using Game.Physics;

namespace Game.Sprites.Shapes
{
    /// <summary>
    /// Default circle sprite, collides with everything and draws a default outline
    /// </summary>
    public class Circle : Sprite
    {
        public static Texture2D DefaultTexture { get; set; }
        public Circle(MainGame game, Vector2 centre, float radius, float mass, bool isStatic, Vector2 bounce, float gravityMultiplier = 1)
        {
            Game = game;
            Body = new Body(this)
            {
                Gravity = World.Gravity * gravityMultiplier,
                Bounce = bounce,
                Mass = mass,
                Enabled = true,
                Shape = Shape.Circle,
                Position = centre,
                Radius = radius,
                Static = isStatic,
                Friction = 1.0f,
                Velocity = new Vector2(0),
                MaxVelocity = new Vector2(5)
            };
        }

        public override void Collisions()
        {
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

        }

        public override void Draw()
        {
            if (Debug.Active)
            {
                MainGame.SpriteBatch.Draw(DefaultTexture, new Rectangle((int)(Body.Centre.X - Body.Radius), (int)(Body.Centre.Y - Body.Radius), (int)Body.Width, (int)Body.Width), Color.Yellow);
            }
        }

        public override void Update()
        {
        }
    }
}
