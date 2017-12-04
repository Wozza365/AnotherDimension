using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Topdown.Input;
using Topdown.Other;
using Topdown.Physics;
using Topdown.Sprites.Shapes;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Topdown.Sprites
{
    public class Hero : Sprite
    {

        public bool Jumped { get; set; }

        private Rectangle DrawRect
        {
            get => new Rectangle((int)(Body.Position.X - Body.Width/2), (int)(Body.Position.Y - Body.Height/2), (int)Body.Width, (int)Body.Height);
            set { }
        }

        public Hero(TopdownGame game, Vector2 position, Vector2 size, Vector2 bounce, float friction, float gravityMultiplier = 1)
        {
            Game = game;
            Guid = new Guid();
            List<Vector2> indices = new List<Vector2>()
            {
                new Vector2(position.X, position.Y),
                new Vector2(position.X + size.X, position.Y),
                new Vector2(position.X + size.X, position.Y + size.Y),
                new Vector2(position.X, position.Y + size.Y)
            };
            Body = new Body(this)
            {
                MaxVelocity = new Vector2(5f, 50f),
                Enabled = true,
                Position = position,//new Vector2(position.X + (size.X/2), position.Y + (size.Y/2)),
                Radius = size.Y/2,
                Game = game,
                Gravity = World.Gravity * gravityMultiplier,
                Bounce = bounce,
                Friction = friction,
                Static = false,
                Shape = Shape.Circle,
                //Indices = indices,
                Mass = 50
            };
        }

        public override void Control()
        {
            Debug.AddLog(OnGround.ToString());
            if (InputManager.Held(Keys.A))
            {
                Body.Velocity.X--;
            }
            if (InputManager.Held(Keys.D))
            {
                Body.Velocity.X++;
            }
            if (InputManager.Pressed(Keys.Space) && !Jumped)
            {
                Body.Velocity.Y = 10;
            }
            if (InputManager.Pressed(Keys.S) && Jumped)
            {
                Body.Velocity.Y = 15;
            }
        }

        public override void Update()
        {
            Jumped = !(Math.Abs(Body.Velocity.Y) <= 0.4f);
            OnGround = Math.Abs(Body.Velocity.Y) <= 0.4f;
        }

        public override void Draw()
        {
            TopdownGame.SpriteBatch.Draw(Texture, DrawRect, TextureRect, Color.White);
            TopdownGame.SpriteBatch.Draw(Circle.DefaultTexture, new Rectangle((int)(Body.Centre.X - Body.Radius), (int)(Body.Centre.Y - Body.Radius), (int)Body.Width, (int)Body.Width), Color.Yellow);
        }

        public override void Collisions()
        {
            int i = 0;
            Vector2 result = new Vector2();
            float distance = 0;
            foreach (var s in TopdownGame.Sprites)
            {
                if (!s.Equals(this) && World.Intersects(Body, s.Body, ref result, ref distance))
                {
                    i = 0;
                    //World.Separate(Body, s.Body, ref result, ref distance);
                }
            }
        }
    }
}
