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
            List<Vector2> indices = new List<Vector2>()
            {
                new Vector2(position.X, position.Y),
                new Vector2(position.X + size.X, position.Y),
                new Vector2(position.X + size.X, position.Y + size.Y),
                new Vector2(position.X, position.Y + size.Y)
            };
            Body = new Body(this)
            {
                MaxVelocity = new Vector2(5f, 5f),
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
    
            //int j2 = 0;
            //while (j2 < CurrentPath.Nodes.Count - 1)
            //{
            //    for (int i = j2 + 2; i < CurrentPath.Nodes.Count; i++)
            //    {
            //        List<Vector2> points = new List<Vector2>();
            //        var start = new Vector2(CurrentPath.Nodes[j2].Coordinate.X * 40 + (40 / 2), CurrentPath.Nodes[j2].Coordinate.Y * 40 + (40 / 2));
            //        var end = new Vector2(CurrentPath.Nodes[i].Coordinate.X * 40 + (40 / 2), CurrentPath.Nodes[i].Coordinate.Y * 40 + (40 / 2));

            //        var direction = start - end;
            //        var angle = Math.Atan2(direction.Y, direction.X);
            //        angle += Math.PI / 2;
            //        points.Add(new Vector2(end.X + ((float)Math.Cos(angle) * 10), end.Y + ((float)Math.Sin(angle) * 10)));
            //        angle += Math.PI;
            //        points.Add(new Vector2(end.X + ((float)Math.Cos(angle) * 10), end.Y + ((float)Math.Sin(angle) * 10)));

            //        direction *= -1;
            //        angle = Math.Atan2(direction.Y, direction.X);
            //        angle += Math.PI / 2;
            //        points.Add(new Vector2(start.X + ((float)Math.Cos(angle) * 10), start.Y + ((float)Math.Sin(angle) * 10)));
            //        angle += Math.PI;
            //        points.Add(new Vector2(start.X + ((float)Math.Cos(angle) * 10), start.Y + ((float)Math.Sin(angle) * 10)));

            //        var poly = new Polygon(Game, points, Color.White, true, Vector2.Zero);
            //        //Sprites.Add(poly);
            //        for (int k = 0; k < points.Count; k++)
            //        {
            //            if (k == 0)
            //            {
            //                Debug.AddLine(points[0], points[points.Count - 1], 100);
            //            }
            //            else
            //            {
            //                Debug.AddLine(points[k], points[k - 1], 100);
            //            }
            //        }

            //        bool intersects = false;
            //        foreach (var outside in TopdownGame.Sprites.Where(x => x.SpriteType == SpriteTypes.Outside))
            //        {
            //            Vector2 r = Vector2.Zero;
            //            float d = 0;
            //            if (World.Intersects(poly.Body, outside.Body, ref r, ref d, true))
            //            {
            //                intersects = true;
            //            }
            //        }
            //        if (intersects)
            //        {
            //            int k = i - j2 - 1;
            //            CurrentPath.Nodes.RemoveRange(j2 + 1, k);
            //            j2++;
            //            //i++;
            //            //for (var k = j + 1; k < i - 1; k++)
            //            //{
            //            //    Path.Nodes.RemoveAt(k);
            //            //}
            //            //Path.Nodes.RemoveAt(i - 1);
            //        }
            //        //else
            //        //{
            //        //    j2++;
            //        //    //i--;
            //        //}
            //    }
            //    //if (Path.Nodes[j].Parent != null)
            //    //{
            //    //    Debug.AddPoint(Path.Nodes[j].Coordinate * TileScreenWidth + new Vector2(TileScreenWidth / 2), Color.Red);
            //    //    Debug.AddPoint(Path.Nodes[j].Parent.Coordinate * TileScreenWidth + new Vector2(TileScreenWidth / 2), Color.Red);
            //    //    Debug.AddLine(Path.Nodes[j].Coordinate * TileScreenWidth + new Vector2(TileScreenWidth / 2), Path.Nodes[j].Parent.Coordinate * TileScreenWidth + new Vector2(TileScreenWidth / 2), 100);
            //    //}
            //    j2++;
            //}

            //for (int i = 0; i < CurrentPath.Nodes.Count; i++)
            //{
            //    bool visible = true;
            //    int position = i + 2;
            //    while (visible)
            //    {
            //        if (position >= CurrentPath.Nodes.Count)
            //        {
            //            visible = false;
            //            break;
            //        }
            //        var start = new Vector2(CurrentPath.Nodes[i].Coordinate.X * 40 + (40 / 2), CurrentPath.Nodes[i].Coordinate.Y * 40 + (40 / 2));
            //        var end = new Vector2(CurrentPath.Nodes[position].Coordinate.X * 40 + (40 / 2), CurrentPath.Nodes[position].Coordinate.Y * 40 + (40 / 2));
                    
            //        List<Vector2> points = new List<Vector2>();
            //        var direction = start - end;
            //        var angle = Math.Atan2(direction.Y, direction.X);
            //        angle += Math.PI / 2;
            //        points.Add(new Vector2(end.X + ((float)Math.Cos(angle) * 5), end.Y + ((float)Math.Sin(angle) * 5)));
            //        angle += Math.PI;
            //        points.Add(new Vector2(end.X + ((float)Math.Cos(angle) * 5), end.Y + ((float)Math.Sin(angle) * 5)));

            //        direction *= -1;
            //        angle = Math.Atan2(direction.Y, direction.X);
            //        angle += Math.PI / 2;
            //        points.Add(new Vector2(start.X + ((float)Math.Cos(angle) * 5), start.Y + ((float)Math.Sin(angle) * 5)));
            //        angle += Math.PI;
            //        points.Add(new Vector2(start.X + ((float)Math.Cos(angle) * 5), start.Y + ((float)Math.Sin(angle) * 5)));

            //        var poly = new Polygon(Game, points, Color.White, true, Vector2.Zero);
            //        //for (int k = 0; k < points.Count; k++)
            //        //{
            //        //    if (k == 0)
            //        //    {
            //        //        Debug.AddLine(points[0], points[points.Count - 1], 100);
            //        //    }
            //        //    else
            //        //    {
            //        //        Debug.AddLine(points[k], points[k - 1], 100);
            //        //    }
            //        //}

            //        foreach (var outside in TopdownGame.Sprites.Where(x => x.SpriteType == SpriteTypes.Outside))
            //        {
            //            Vector2 r = Vector2.Zero;
            //            float d = 0;
            //            if (World.Intersects(outside.Body, poly.Body, ref r, ref d, true))
            //            {
            //                visible = false;
            //                break;
            //            }
            //        }
            //        if (visible) position++;
            //    }
            //    CurrentPath.Nodes.RemoveRange(i + 1, position - 1 - i);
            //}

            //for (int i = 0; i < CurrentPath.Nodes.Count; i++)
            //{
            //    Vector2 start;
            //    Vector2 end;
            //    if (i == 0)
            //    {
            //        start = new Vector2(CurrentPath.Nodes[i].Coordinate.X * 40 + (40 / 2),CurrentPath.Nodes[i].Coordinate.Y * 40 + (40 / 2));
            //        end = new Vector2(CurrentPath.Nodes[CurrentPath.Nodes.Count - 1].Coordinate.X * 40 + (40 / 2),CurrentPath.Nodes[CurrentPath.Nodes.Count - 1].Coordinate.Y * 40 + (40 / 2));
            //    }
            //    else
            //    {
            //        start = new Vector2(CurrentPath.Nodes[i].Coordinate.X * 40 + (40 / 2), CurrentPath.Nodes[i].Coordinate.Y * 40 + (40 / 2));
            //        end = new Vector2(CurrentPath.Nodes[i-1].Coordinate.X * 40 + (40 / 2), CurrentPath.Nodes[i - 1].Coordinate.Y * 40 + (40 / 2));
            //    }
                
            //    List<Vector2> points = new List<Vector2>();
            //    var direction = start - end;
            //    var angle = Math.Atan2(direction.Y, direction.X);
            //    angle += Math.PI / 2;
            //    points.Add(new Vector2(end.X + ((float)Math.Cos(angle) * 10), end.Y + ((float)Math.Sin(angle) * 10)));
            //    angle += Math.PI;
            //    points.Add(new Vector2(end.X + ((float)Math.Cos(angle) * 10), end.Y + ((float)Math.Sin(angle) * 10)));

            //    direction *= -1;
            //    angle = Math.Atan2(direction.Y, direction.X);
            //    angle += Math.PI / 2;
            //    points.Add(new Vector2(start.X + ((float)Math.Cos(angle) * 10), start.Y + ((float)Math.Sin(angle) * 10)));
            //    angle += Math.PI;
            //    points.Add(new Vector2(start.X + ((float)Math.Cos(angle) * 10), start.Y + ((float)Math.Sin(angle) * 10)));

            //    var poly = new Polygon(Game, points, Color.White, true, Vector2.Zero);
            //    for (int k = 0; k < points.Count; k++)
            //    {
            //        if (k == 0)
            //        {
            //            Debug.AddLine(points[0], points[points.Count - 1], 100);
            //        }
            //        else
            //        {
            //            Debug.AddLine(points[k], points[k - 1], 100);
            //        }
            //    }
            //}
            
        }

        public override void Control()
        {
            //var direction = NextNode.Coordinate - CurrentNode.Coordinate;
            //direction.Normalize();
            //direction *= 5;
            //Body.Velocity = direction;


            //Debug.AddLog(OnGround.ToString());
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
            CurrentNode.Coordinate = new Vector2((int)(Body.Position.X/40), (int)(Body.Position.Y/40));

            if (CurrentNode.Coordinate != PreviousNode.Coordinate)
            {
                foreach (Enemy enemy in TopdownGame.Sprites.Where(x => x.SpriteType == SpriteTypes.Enemy && Vector2.Distance(Body.Position, x.Body.Position) < 500))
                {
                    enemy.CreatePath();
                }
            }

            if (CurrentPath.Nodes[1].Coordinate == CurrentNode.Coordinate/* && CurrentPath.Nodes.Count > 1*/)
            {
                CurrentPath.Nodes.RemoveAt(0);
                //CurrentNode = CurrentPath.Nodes[0];
                if (CurrentPath.Nodes.Count > 2) NextNode = CurrentPath.Nodes[1];
            }
            else if (CurrentPath.Nodes.Count > 0)
            {
                
            }
            else if (CurrentPath.Nodes[0].Coordinate != CurrentNode.Coordinate)
            {
                CreatePath();
            }
            
            Jumped = !(Math.Abs(Body.Velocity.Y) <= 0.4f);
            OnGround = Math.Abs(Body.Velocity.Y) <= 0.4f;
        }

        public override void Draw()
        {
            TopdownGame.SpriteBatch.Draw(Texture, DrawRect, TextureRect, Color.White, (float)Math.Atan2(FaceDirection.X, -FaceDirection.Y), new Vector2(TextureRect.Width/2 + Body.HalfWidth, TextureRect.Height/2 + Body.HalfHeight), SpriteEffects.None, 0);
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
                    if (s.SpriteType == SpriteTypes.Portal)
                    {
                        i++;
                    }
                    i = 0;
                    //World.Separate(Body, s.Body, ref result, ref distance);
                }
            }
        }
    }
}