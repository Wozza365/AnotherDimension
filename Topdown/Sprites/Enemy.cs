using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Topdown.AI;
using Topdown.Other;
using Topdown.Physics;
using Topdown.Sprites.Shapes;

namespace Topdown.Sprites
{
    public class Enemy : AISprite
    {
        public static Dictionary<SpriteTypes, int> Targets = new Dictionary<SpriteTypes, int>();
        
        public Enemy(TopdownGame game, Vector2 position, Vector2 size, Vector2 bounce, float friction, float gravityMultiplier = 1)
        {
            Game = game;
            Guid = new Guid();
            SpriteType = SpriteTypes.Enemy;
            Body = new Body(this)
            {
                MaxVelocity = new Vector2(5f, 5f),
                Enabled = true,
                Position = position,
                Radius = size.Y/2,
                Game = game,
                Gravity = World.Gravity * gravityMultiplier,
                Bounce = bounce,
                Friction = friction,
                Static = false,
                Shape = Shape.Circle,
                Mass = 50
            };
            Targets.Add(SpriteTypes.Hero, 100);
            Targets.Add(SpriteTypes.WanderNode, 1);
        }

        public void CreatePath()
        {
            List<Sprite> spriteTargets = TopdownGame.Sprites.Where(x => Targets.Keys.Contains(x.SpriteType)).ToList();
            List<WanderNode> wanderTargets = TopdownGame.WanderNodes;
            wanderTargets.ForEach(x => spriteTargets.Add(x));
            spriteTargets.RemoveAll(x => x.SpriteType == SpriteTypes.Hero && Vector2.Distance(Body.Position, x.Body.Position) > 500);
            List<Target> targets = spriteTargets.Select(x => new Target()
            {
                Distance = Vector2.Distance(Body.Position, x.Body.Position),
                SpriteType = x.SpriteType,
                Weight = Targets.Where(y => y.Key == x.SpriteType).Select(y => y.Value).First(),
                Sprite = x
            }).ToList();
            targets = targets.OrderBy(x => (1 / x.Weight) * x.Distance).ToList();
            if (targets.All(x => x.SpriteType != SpriteTypes.Hero))
            {
                Random r = new Random();
                targets = targets.OrderBy(x => r.Next()).ToList();
            }
            CurrentPath = AStar.GenerateAStarPath(this, targets.First().Sprite);
            TargetType = targets.First().Sprite.SpriteType;

            CurrentNode = CurrentPath.Nodes[0];
            NextNode = CurrentPath.Nodes[1];
            TargetNode = CurrentPath.Nodes.Last();

            for (int i = 0; i < CurrentPath.Nodes.Count; i++)
            {
                Vector2 start;
                Vector2 end;
                if (i == 0)
                {
                    continue;
                    start = new Vector2(CurrentPath.Nodes[i].Coordinate.X * 40 + (40 / 2), CurrentPath.Nodes[i].Coordinate.Y * 40 + (40 / 2));
                    end = new Vector2(CurrentPath.Nodes[CurrentPath.Nodes.Count - 1].Coordinate.X * 40 + (40 / 2), CurrentPath.Nodes[CurrentPath.Nodes.Count - 1].Coordinate.Y * 40 + (40 / 2));
                }
                start = new Vector2(CurrentPath.Nodes[i].Coordinate.X * 40 + (40 / 2), CurrentPath.Nodes[i].Coordinate.Y * 40 + (40 / 2));
                end = new Vector2(CurrentPath.Nodes[i - 1].Coordinate.X * 40 + (40 / 2), CurrentPath.Nodes[i - 1].Coordinate.Y * 40 + (40 / 2));

                List<Vector2> points = new List<Vector2>();
                var direction = start - end;
                var angle = Math.Atan2(direction.Y, direction.X);
                angle += Math.PI / 2;
                points.Add(new Vector2(end.X + ((float)Math.Cos(angle) * 10), end.Y + ((float)Math.Sin(angle) * 10)));
                angle += Math.PI;
                points.Add(new Vector2(end.X + ((float)Math.Cos(angle) * 10), end.Y + ((float)Math.Sin(angle) * 10)));

                direction *= -1;
                angle = Math.Atan2(direction.Y, direction.X);
                angle += Math.PI / 2;
                points.Add(new Vector2(start.X + ((float)Math.Cos(angle) * 10), start.Y + ((float)Math.Sin(angle) * 10)));
                angle += Math.PI;
                points.Add(new Vector2(start.X + ((float)Math.Cos(angle) * 10), start.Y + ((float)Math.Sin(angle) * 10)));

                var poly = new Polygon(Game, points, Color.White, true, Vector2.Zero);
                for (int k = 0; k < points.Count; k++)
                {
                    if (k == 0)
                    {
                        Debug.AddLine(points[0], points[points.Count - 1], 1);
                    }
                    else
                    {
                        Debug.AddLine(points[k], points[k - 1], 1);
                    }
                }
            }
        }
        
        public override void Control()
        {
            var direction = NextNode.Coordinate - CurrentNode.Coordinate;
            direction.Normalize();
            direction *= 2;
            Body.Velocity = direction;

        }

        public override void Update()
        {
            if (TargetType == SpriteTypes.Hero && Vector2.Distance(Body.Position, TargetSprite.Body.Position) > 500)
            {
                CreatePath();
            }
            CurrentNode.Coordinate = new Vector2((int)(Body.Position.X / 40), (int)(Body.Position.Y / 40));
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
        }

        public override void Draw()
        {
            for (int i = 0; i < CurrentPath.Nodes.Count; i++)
            {
                Vector2 start;
                Vector2 end;
                if (i == 0)
                {
                    continue;
                    start = new Vector2(CurrentPath.Nodes[i].Coordinate.X * 40 + (40 / 2), CurrentPath.Nodes[i].Coordinate.Y * 40 + (40 / 2));
                    end = new Vector2(CurrentPath.Nodes[CurrentPath.Nodes.Count - 1].Coordinate.X * 40 + (40 / 2), CurrentPath.Nodes[CurrentPath.Nodes.Count - 1].Coordinate.Y * 40 + (40 / 2));
                }
                else
                {
                    start = new Vector2(CurrentPath.Nodes[i].Coordinate.X * 40 + (40 / 2), CurrentPath.Nodes[i].Coordinate.Y * 40 + (40 / 2));
                    end = new Vector2(CurrentPath.Nodes[i - 1].Coordinate.X * 40 + (40 / 2), CurrentPath.Nodes[i - 1].Coordinate.Y * 40 + (40 / 2));
                }

                List<Vector2> points = new List<Vector2>();
                var direction = start - end;
                var angle = Math.Atan2(direction.Y, direction.X);
                angle += Math.PI / 2;
                points.Add(new Vector2(end.X + ((float)Math.Cos(angle) * 10), end.Y + ((float)Math.Sin(angle) * 10)));
                angle += Math.PI;
                points.Add(new Vector2(end.X + ((float)Math.Cos(angle) * 10), end.Y + ((float)Math.Sin(angle) * 10)));

                direction *= -1;
                angle = Math.Atan2(direction.Y, direction.X);
                angle += Math.PI / 2;
                points.Add(new Vector2(start.X + ((float)Math.Cos(angle) * 10), start.Y + ((float)Math.Sin(angle) * 10)));
                angle += Math.PI;
                points.Add(new Vector2(start.X + ((float)Math.Cos(angle) * 10), start.Y + ((float)Math.Sin(angle) * 10)));

                var poly = new Polygon(Game, points, Color.White, true, Vector2.Zero);
                for (int k = 0; k < points.Count; k++)
                {
                    if (k == 0)
                    {
                        Debug.AddLine(points[0], points[points.Count - 1], 0);
                    }
                    else
                    {
                        Debug.AddLine(points[k], points[k - 1], 0);
                    }
                }
            }
            TopdownGame.SpriteBatch.Draw(Circle.DefaultTexture, new Rectangle((int)(Body.Centre.X - Body.Radius), (int)(Body.Centre.Y - Body.Radius), (int)Body.Width, (int)Body.Width), Color.Yellow);
        }

        public override void Collisions()
        {
            
        }
    }
}
