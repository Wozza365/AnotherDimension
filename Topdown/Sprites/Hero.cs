using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Topdown.AI;
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
        public Vector2 FaceDirection;

        public Path CurrentPath { get; set; }
        public Node CurrentNode { get; set; }
        public Node PreviousNode { get; set; }
        public Node NextNode { get; set; }
        public Node TargetNode { get; set; }
        public List<Weapon> CurrentWeapons { get; set; } = new List<Weapon>();
        public int SelectedWeapon;
        public DateTime NextBullet { get; set; }
        public float Health { get; set; } = 100;

        private Rectangle DrawRect => new Rectangle((int)(Body.Position.X), (int)(Body.Position.Y), (int)Body.Width, (int)Body.Height);

        public int TilePositionX => (int)Body.Centre.X / Game.Map1.TileWidth;
        public int TilePositionY => (int)Body.Centre.Y / Game.Map1.TileHeight;

        public Hero(TopdownGame game, Vector2 position, Vector2 size, Vector2 bounce, float friction, float gravityMultiplier = 1)
        {
            Game = game;
            Guid = new Guid();
            SpriteType = SpriteTypes.Hero;
            FaceDirection = Vector2.UnitX;
            CurrentWeapons.Add(new Weapon()
            {
                BulletConfig = TopdownGame.BulletConfigs[WeaponTypes.Pistol],
                Type = WeaponTypes.Pistol,
                Ammo = int.MaxValue
            });
            //CurrentWeapons.Add(new Weapon()
            //{
            //    BulletConfig = TopdownGame.BulletConfigs[WeaponTypes.SMG],
            //    Type = WeaponTypes.SMG,
            //    Ammo = int.MaxValue
            //});
            //CurrentWeapons.Add(new Weapon()
            //{
            //    BulletConfig = TopdownGame.BulletConfigs[WeaponTypes.RPG],
            //    Type = WeaponTypes.RPG,
            //    Ammo = int.MaxValue
            //});
            List<Vector2> indices = new List<Vector2>()
            {
                new Vector2(position.X, position.Y),
                new Vector2(position.X + size.X, position.Y),
                new Vector2(position.X + size.X, position.Y + size.Y),
                new Vector2(position.X, position.Y + size.Y)
            };
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
        }

        public void CreatePath()
        {
            CurrentPath = AStar.GenerateAStarPath(this, TopdownGame.Sprites.First(x => x.SpriteType == SpriteTypes.Portal));
            PreviousNode = new Node()
            {
                Coordinate = CurrentPath.Nodes[0].Coordinate,
                Area = CurrentPath.Nodes[0].Area,
                Cost = CurrentPath.Nodes[0].Cost,
                F = CurrentPath.Nodes[0].F,
                G = CurrentPath.Nodes[0].G,
                H = CurrentPath.Nodes[0].H
            };
            CurrentNode = new Node()
            {
                Coordinate = CurrentPath.Nodes[0].Coordinate,
                Area = CurrentPath.Nodes[0].Area,
                Cost = CurrentPath.Nodes[0].Cost,
                F = CurrentPath.Nodes[0].F,
                G = CurrentPath.Nodes[0].G,
                H = CurrentPath.Nodes[0].H
            };
            NextNode = CurrentPath.Nodes[1];
            TargetNode = CurrentPath.Nodes.Last();
        }

        public override void Control()
        {
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
            if (InputManager.Held(Keys.E) && NextBullet <= DateTime.Now && CurrentWeapons[SelectedWeapon].Ammo > 0)
            {
                TopdownGame.Sprites.Add(new Bullet(Body.Position, new Vector2(16), FaceDirection, CurrentWeapons[SelectedWeapon].BulletConfig));
                NextBullet = DateTime.Now + CurrentWeapons[SelectedWeapon].BulletConfig.FireDelay;
                CurrentWeapons[SelectedWeapon].Ammo--;
            }
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
                foreach (Enemy enemy in TopdownGame.Sprites.Where(x => x.SpriteType == SpriteTypes.Enemy && Vector2.Distance(Body.Position, x.Body.Position) < 500))
                {
                    enemy.CreatePath();
                }
            }

            //if (CurrentPath.Nodes[1].Coordinate == CurrentNode.Coordinate/* && CurrentPath.Nodes.Count > 1*/)
            //{
            //    CurrentPath.Nodes.RemoveAt(0);
            //    //CurrentNode = CurrentPath.Nodes[0];
            //    if (CurrentPath.Nodes.Count > 2) NextNode = CurrentPath.Nodes[1];
            //}
            //else if (CurrentPath.Nodes.Count > 0)
            //{

            //}
            //else if (CurrentPath.Nodes[0].Coordinate != CurrentNode.Coordinate)
            //{
            //    CreatePath();
            //}

            Jumped = !(Math.Abs(Body.Velocity.Y) <= 0.4f);
            OnGround = Math.Abs(Body.Velocity.Y) <= 0.4f;
        }

        public override void Draw()
        {
            TopdownGame.SpriteBatch.Draw(Texture, DrawRect, TextureRect, Color.White, (float)Math.Atan2(FaceDirection.X, -FaceDirection.Y), new Vector2(TextureRect.Width / 2 + Body.HalfWidth, TextureRect.Height / 2 + Body.HalfHeight), SpriteEffects.None, 0);
            TopdownGame.SpriteBatch.Draw(Circle.DefaultTexture, new Rectangle((int)(Body.Centre.X - Body.Radius), (int)(Body.Centre.Y - Body.Radius), (int)Body.Width, (int)Body.Width), Color.Yellow);
        }

        public override void Collisions()
        {
            Vector2 result = new Vector2();
            float distance = 0;
            //Loop through each sprite
            for (var i = 0; i < TopdownGame.Sprites.Count; i++)
            {
                var s = TopdownGame.Sprites[i];
                if (!s.Equals(this) && World.Intersects(Body, s.Body, ref result, ref distance))
                {
                    //Customize our behaviour based on different sprite types
                    if (s.SpriteType == SpriteTypes.Portal)
                    {
                        //TODO add functionality for portal
                    }
                    if (s.SpriteType == SpriteTypes.Powerup)
                    {
                        var p = (Powerup) s;
                        CurrentWeapons[SelectedWeapon].Ammo += p.PowerupConfig.Ammo;
                        Health += p.PowerupConfig.Health;
                        Body.MaxVelocity.X += p.PowerupConfig.Speed;
                        Body.MaxVelocity.Y += p.PowerupConfig.Speed;
                        if (p.PowerupConfig.PowerupType == PowerupType.RPG)
                        {
                            if (CurrentWeapons.All(x => x.Type != WeaponTypes.RPG))
                            {
                                CurrentWeapons.Add(new Weapon()
                                {
                                    BulletConfig = TopdownGame.BulletConfigs[WeaponTypes.RPG],
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
                                    BulletConfig = TopdownGame.BulletConfigs[WeaponTypes.SMG],
                                    Type = WeaponTypes.SMG,
                                    Ammo = p.PowerupConfig.Ammo
                                });
                            }
                            else
                            {
                                CurrentWeapons.First(x => x.Type == WeaponTypes.SMG).Ammo = p.PowerupConfig.Ammo;
                            }
                        }
                        TopdownGame.Sprites.Remove(s);
                    }
                    if (s.SpriteType == SpriteTypes.Enemy)
                    {
                        Health -= 2;
                    }
                }
            }
        }
    }
}