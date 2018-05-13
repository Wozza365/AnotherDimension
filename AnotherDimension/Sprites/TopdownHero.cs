using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game.AI;
using Game.Input;
using Game.Physics;
using Game.Sprites.Shapes;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Game.Sprites
{
    /// <summary>
    /// Hero class for the hero in the topdown dimension
    /// </summary>
    public class TopdownHero : Sprite
    {
        public bool Jumped { get; set; }
        public Vector2 FaceDirection;
        
        public Node CurrentNode { get; set; }
        public Node PreviousNode { get; set; }
        public List<Weapon> CurrentWeapons { get; set; } = new List<Weapon>();
        public int SelectedWeapon;
        public DateTime NextBullet { get; set; }
        public float Health { get; set; } = 100;

        private Rectangle DrawRect => new Rectangle((int)(Body.Centre.X), (int)(Body.Centre.Y), (int)Body.Width, (int)Body.Height);
        
        public TopdownHero(MainGame game, Vector2 position, Vector2 size, Vector2 bounce, float friction, float gravityMultiplier = 1)
        {
            Game = game;
            Guid = new Guid();
            SpriteType = SpriteTypes.TopdownHero;
            FaceDirection = Vector2.UnitX;

            //Default pistol
            CurrentWeapons.Add(new Weapon()
            {
                BulletConfig = MainGame.BulletConfigs[WeaponTypes.Pistol],
                Type = WeaponTypes.Pistol,
                Ammo = int.MaxValue
            });

            Body = new Body(this)
            {
                MaxVelocity = new Vector2(3f, 3f),
                Enabled = true,
                Position = position,
                Radius = size.Y / 2,
                Game = game,
                Gravity = World.Gravity * gravityMultiplier,
                Bounce = bounce,
                Friction = friction,
                Static = false,
                Shape = Shape.Circle,
                Mass = 50
            };
            CurrentNode = new Node()
            {
                Coordinate = new Vector2(position.X / 40, position.Y / 40)
            };
            PreviousNode = new Node()
            {
                Coordinate = new Vector2(position.X / 40, position.Y / 40)
            };
        }

        public override void Control()
        {
            //Movement the character
            if (InputManager.Held(Keys.A))
            {
                Body.Velocity.X--;
            }
            if (InputManager.Held(Keys.D))
            {
                Body.Velocity.X++;
            }
            if (InputManager.Held(Keys.S))
            {
                Body.Velocity.Y++;
            }
            if (InputManager.Held(Keys.W))
            {
                Body.Velocity.Y--;
            }
            //Change the direction that the character is looking at to aim
            if (InputManager.Held(Keys.Up))
            {
                FaceDirection = -Vector2.UnitY;
            }
            if (InputManager.Held(Keys.Down))
            {
                FaceDirection = Vector2.UnitY;
            }
            if (InputManager.Held(Keys.Left))
            {
                FaceDirection = -Vector2.UnitX;
            }
            if (InputManager.Held(Keys.Right))
            {
                FaceDirection = Vector2.UnitX;
            }

            //Fire bullets when held and in intervals
            if (InputManager.Held(Keys.E) && NextBullet <= DateTime.Now && CurrentWeapons[SelectedWeapon].Ammo > 0)
            {
                MainGame.Sprites.Add(new Bullet(Body.Position, new Vector2(16), FaceDirection, CurrentWeapons[SelectedWeapon].BulletConfig));
                NextBullet = DateTime.Now + CurrentWeapons[SelectedWeapon].BulletConfig.FireDelay;
                CurrentWeapons[SelectedWeapon].Ammo--;
            }
            //Weapon changing with number keys
            if (InputManager.Held(Keys.D1))
            {
                SelectedWeapon = 0;
            }
            if (InputManager.Held(Keys.D2) && CurrentWeapons.Count > 1)
            {
                SelectedWeapon = 1;
            }
            if (InputManager.Held(Keys.D3) && CurrentWeapons.Count > 2)
            {
                SelectedWeapon = 2;
            }
        }

        public override void Update()
        {
            PreviousNode.Coordinate = new Vector2(CurrentNode.Coordinate.X, CurrentNode.Coordinate.Y);
            CurrentNode.Coordinate = new Vector2((int)(Body.Position.X / 40), (int)(Body.Position.Y / 40));

            //our enemies may wish to update their path if we are now within range
            if (CurrentNode.Coordinate != PreviousNode.Coordinate)
            {
                foreach (Enemy enemy in MainGame.Sprites.Where(x => x.SpriteType == SpriteTypes.Enemy && Vector2.Distance(Body.Position, x.Body.Position) < 500))
                {
                    enemy.CreatePath();
                }
            }

            Jumped = !(Math.Abs(Body.Velocity.Y) <= 0.4f);
            OnGround = Math.Abs(Body.Velocity.Y) <= 0.4f;
        }

        public override void Draw()
        {
            MainGame.SpriteBatch.Draw(Texture, DrawRect, TextureRect, Color.White, (float)Math.Atan2(FaceDirection.X, -FaceDirection.Y) - (float)Math.PI / 2, new Vector2(TextureRect.Width / 2/* + Body.HalfWidth*/, TextureRect.Height / 2/* + Body.HalfHeight*/), SpriteEffects.None, 0);
            MainGame.SpriteBatch.Draw(Circle.DefaultTexture, new Rectangle((int)(Body.Centre.X - Body.Radius), (int)(Body.Centre.Y - Body.Radius), (int)Body.Width, (int)Body.Width), Color.Yellow);
        }

        public override void Collisions()
        {
            Vector2 result = new Vector2();
            float distance = 0;
            //Loop through each sprite
            for (var i = 0; i < MainGame.Sprites.Count; i++)
            {
                var s = MainGame.Sprites[i];
                if (!s.Equals(this) && World.Intersects(Body, s.Body, ref result, ref distance))
                {
                    //Customize our behaviour based on different sprite types
                    if (s.SpriteType == SpriteTypes.Portal)
                    {
                        MainGame.SwitchMaps = true;
                    }
                    if (s.SpriteType == SpriteTypes.Powerup)
                    {
                        //Apply the powerup to the player
                        var p = (Powerup) s;
                        CurrentWeapons[SelectedWeapon].Ammo += p.PowerupConfig.Ammo;
                        Health += p.PowerupConfig.Health;
                        Body.MaxVelocity.X += p.PowerupConfig.Speed;
                        Body.MaxVelocity.Y += p.PowerupConfig.Speed;
                        //add weapons or more ammo
                        if (p.PowerupConfig.PowerupType == PowerupType.RPG)
                        {
                            if (CurrentWeapons.All(x => x.Type != WeaponTypes.RPG))
                            {
                                CurrentWeapons.Add(new Weapon()
                                {
                                    BulletConfig = MainGame.BulletConfigs[WeaponTypes.RPG],
                                    Type = WeaponTypes.RPG,
                                    Ammo = p.PowerupConfig.Ammo
                                });
                            }
                            else
                            {
                                CurrentWeapons.First(x => x.Type == WeaponTypes.RPG).Ammo = p.PowerupConfig.Ammo;
                            }
                        }
                        
                        if (p.PowerupConfig.PowerupType == PowerupType.SMG)
                        {
                            if (CurrentWeapons.All(x => x.Type != WeaponTypes.SMG))
                            {
                                CurrentWeapons.Add(new Weapon()
                                {
                                    BulletConfig = MainGame.BulletConfigs[WeaponTypes.SMG],
                                    Type = WeaponTypes.SMG,
                                    Ammo = p.PowerupConfig.Ammo
                                });
                            }
                            else
                            {
                                CurrentWeapons.First(x => x.Type == WeaponTypes.SMG).Ammo = p.PowerupConfig.Ammo;
                            }
                        }
                        MainGame.Sprites.Remove(s);
                    }
                    //Take damage from enemy each frame.
                    if (s.SpriteType == SpriteTypes.Enemy)
                    {
                        Health -= 2;
                    }
                }
            }
        }
    }
}