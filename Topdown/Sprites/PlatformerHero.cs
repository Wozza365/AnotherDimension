using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Game.Input;
using Game.Other;
using Game.Physics;
using Game.Sprites.Shapes;
using Game.Misc;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Game.Sprites
{
    /// <summary>
    /// Hero used for the platformer portion of the game
    /// </summary>
    public class PlatformerHero : Sprite
    {

        public bool Jumped { get; set; }
        public bool OnLadder { get; set; }

        private float GravityMultiplier { get; set; }

        public int GemCount { get; set; }
        public int Health { get; set; } = 100;

        private Rectangle DrawRect
        {
            get => new Rectangle((int)(Body.Position.X), (int)(Body.Position.Y), (int)Body.Width, (int)Body.Height);
        }

        public PlatformerHero(MainGame game, Vector2 position, Vector2 size, Vector2 bounce, float friction, float gravityMultiplier = 1)
        {
            Game = game;
            Guid = Guid.NewGuid();
            SpriteType = SpriteTypes.PlatformerHero;
            GravityMultiplier = gravityMultiplier;

            Body = new Body(this)
            {
                MaxVelocity = new Vector2(5f, 50f),
                Enabled = true,
                Position = position,
                Radius = size.Y / 2,
                Game = game,
                Gravity = World.Gravity * gravityMultiplier,
                Bounce = bounce,
                Friction = friction,
                Static = false,
                Shape = Shape.Circle,
                Mass = 50,
                Guid = Guid
            };
        }

        public override void Control()
        {
            Debug.AddLog(OnGround.ToString());
            if (!Jumped)
            {
                if (InputManager.Held(Keys.A))
                {
                    Body.Velocity.X--;
                }
                if (InputManager.Held(Keys.D))
                {
                    Body.Velocity.X++;
                }
            }
            else
            {
                //less agile in mid air
                if (InputManager.Held(Keys.A))
                {
                    Body.Velocity.X -=0.25f;
                }
                if (InputManager.Held(Keys.D))
                {
                    Body.Velocity.X += 0.25f;
                }
            }
            if (InputManager.Pressed(Keys.Space) && !Jumped)
            {
                //Jump
                Body.Velocity.Y = 10;
            }
            if (InputManager.Pressed(Keys.S) && Jumped)
            {
                //A quick drop to the ground
                Body.Velocity.Y = 15;
            }

            //move up and down ladders
            if (InputManager.Held(Keys.W) && OnLadder)
            {
                Body.Velocity.Y = -2;
            }
            if (InputManager.Held(Keys.S) && OnLadder)
            {
                Body.Velocity.Y = 2;
            }

        }

        public override void Update()
        {
            //Base whether we are jumped on Y velocity
            //Not the proper way, but was left in as you can double jump at the apex of the jump
            //Proper way would be to use an additional collision object at the bottom of the hero which will be grounded if colliding or just detect if our body is colliding at the bottom
            Jumped = !(Math.Abs(Body.Velocity.Y) <= 0.4f);
            OnGround = Math.Abs(Body.Velocity.Y) <= 0.4f;
        }

        public override void Draw()
        {
            Rectangle texRect = TextureRect;
            if (OnLadder)
            {
                texRect = new Rectangle(138, 365, 110, 155);
            }

            MainGame.SpriteBatch.Draw(Texture, DrawRect, texRect, Color.White, 0, Vector2.Zero, Body.Velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            MainGame.SpriteBatch.Draw(Circle.DefaultTexture, new Rectangle((int)(Body.Centre.X - Body.Radius), (int)(Body.Centre.Y - Body.Radius), (int)Body.Width, (int)Body.Width), Color.Yellow);
        }

        public override void Collisions()
        {
            Vector2 result = new Vector2();
            float distance = 0;
            bool onLadder = false;

            for (var i = 0; i < MainGame.Sprites.Count; i++)
            {
                var s = MainGame.Sprites[i];
                if (!s.Equals(this) && World.Intersects(Body, s.Body, ref result, ref distance))
                {
                    if (s.SpriteType == SpriteTypes.Ladder)
                    {
                        onLadder = true;
                    }
                    else if (s.SpriteType == SpriteTypes.Gem)
                    {
                        //Collect the gem
                        MainGame.Sprites.Remove(s);
                        GemCount++;
                        SceneController.GemCount--;
                    }
                    else if (s.SpriteType == SpriteTypes.CannonBall)
                    {
                        //Take damage from the cannonball
                        Health -= 50;
                    }
                    else if (s.SpriteType == SpriteTypes.Lever)
                    {
                        //Disable/Enable the lever
                        if (InputManager.Pressed(Keys.E))
                        {
                            ((Lever)s).Activated = !((Lever)s).Activated;
                            ((Lever)s).Cannon.Activated = !((Lever)s).Cannon.Activated;
                        }
                    }
                    else if (s.SpriteType == SpriteTypes.Portal)
                    {
                        //Change maps when the player reaches the portal
                        MainGame.SwitchMaps = true;
                    }
                    else
                    {
                        //Otherwise just standard separate
                        World.Separate(Body, s.Body, ref result, ref distance);
                    }
                }
            }
            if (onLadder)
            {
                //Disable gravity, but decrease our velocity on the ladder
                Body.Velocity *= 0.9f;
                Body.Gravity = Vector2.Zero;
            }
            else
            {
                //Re-enable gravity when we are no longer on the ladder
                Body.Gravity = World.Gravity * GravityMultiplier;
            }
            OnLadder = onLadder;
        }
    }
}
