using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Topdown.AI;
using Topdown.Misc;
using Topdown.Other;
using Topdown.Physics;
using Topdown.Sprites.Shapes;

namespace Topdown.Sprites
{
    public class Enemy : AISprite
    {
        public Dictionary<SpriteTypes, int> Targets = new Dictionary<SpriteTypes, int>();
        public bool AtEnemy { get; set; } = false;
        public int Health { get; set; }
        public Vector2 FaceDirection { get; set; } = Vector2.UnitY;

        public Enemy(TopdownGame game, Texture2D texture, Rectangle texRect, Vector2 position, Vector2 size, Vector2 bounce, float friction, float gravityMultiplier = 1)
        {
            //Standard setup of required properties
            Game = game;
            Guid = new Guid();
            SpriteType = SpriteTypes.Enemy;
            Health = 100;
            Texture = texture;
            TextureRect = texRect;

            //Setup the physics body, through some testing ive found circle works best
            //Seems to be industry standard
            Body = new Body(this)
            {
                MaxVelocity = new Vector2(5f, 5f),
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
            Targets.Add(SpriteTypes.Hero, 100);
            Targets.Add(SpriteTypes.WanderNode, 1);
        }

        public void CreatePath()
        {
            //Get all of our targets, including the wander nodes
            List<Sprite> spriteTargets = TopdownGame.Sprites.Where(x => Targets.Keys.Contains(x.SpriteType)).ToList();
            List<WanderNode> wanderTargets = TopdownGame.WanderNodes;
            wanderTargets.ForEach(x => spriteTargets.Add(x));

            //remove the player if they are far away and create a more friendly Target for each
            spriteTargets.RemoveAll(x => x.SpriteType == SpriteTypes.Hero && Vector2.Distance(Body.Position, x.Body.Position) > 500);
            List<Target> targets = spriteTargets.Select(x => new Target()
            {
                Distance = Vector2.Distance(Body.Position, x.Body.Position),
                SpriteType = x.SpriteType,
                Weight = Targets.Where(y => y.Key == x.SpriteType).Select(y => y.Value).First(),
                Sprite = x
            }).ToList();
            
            //this line only really affects when hero is within range to put it top of the list
            targets = targets.OrderBy(x => (1 / x.Weight) * x.Distance).ToList();
            if (CurrentPath != null)
                CurrentPath.Nodes = new List<Node>();
            if (CurrentPath == null || CurrentPath.Nodes.Count <= 1)
            {
                //Shuffle the list if the hero isnt in it
                if (targets.All(x => x.SpriteType != SpriteTypes.Hero))
                {
                    //Effective method to shuffle a list
                    Random r = new Random();
                    targets = targets.OrderBy(x => r.Next()).ToList();
                }

                CurrentPath = AStar.GenerateAStarPath(this, targets.First().Sprite);
            }

            if (CurrentPath.Nodes.Count == 0)
            {
                return; //at player, so sit around
            }

            TargetType = targets.First().Sprite.SpriteType;
            TargetSprite = targets.First().Sprite;

            CurrentNode = CurrentPath.Nodes[0];
            NextNode = CurrentPath.Nodes[CurrentPath.Nodes.Count > 1 ? 1 : 0];
            TargetNode = CurrentPath.Nodes.Last();
        }

        public override void Control()
        {
            if (!AtEnemy)
            {
                //var direction = CurrentNode.Centre - Body.Position;
                var direction = NextNode.Coordinate - CurrentNode.Coordinate;
                if (direction.X != 0 || direction.Y != 0)
                {
                    direction.Normalize();
                }
                else
                {
                    direction = CurrentNode.Centre - Body.Centre;
                    direction.Normalize();
                }
                direction *= Body.MaxVelocity;
                Body.Velocity = direction;
            }
            else
            {
                
                Body.Velocity = Vector2.Zero;
            }

        }

        public override void Update()
        {
            if (Health <= 0)
            {
                TopdownGame.Sprites.Remove(this);
                return;
            }

            Body.MaxVelocity = TargetType == SpriteTypes.Hero ? new Vector2(1.5f) : Vector2.One;
           

            if (TargetType == SpriteTypes.Hero && Vector2.Distance(Body.Centre, TargetSprite.Body.Centre) > 500)
            {
                CreatePath();
                return;
            }
            else if (TargetType == SpriteTypes.Hero && Vector2.Distance(Body.Centre, TargetSprite.Body.Centre) < 500)
            {
                CreatePath();
                return;
            }
            CurrentNode.Coordinate = new Vector2((int)(Body.Centre.X / 40), (int)(Body.Centre.Y / 40));
            if ((CurrentPath.Nodes.Count == 1 || CurrentPath.Nodes.Count == 0) && TargetType == SpriteTypes.Hero)
            {
                AtEnemy = true;
            }
            else if (CurrentPath.Nodes.Count == 1)
            {
                CreatePath();
            }
            else if (CurrentPath.Nodes[0].Coordinate == CurrentNode.Coordinate)
            {
                if ((Body.Centre - CurrentPath.Nodes[0].Centre).Length() < 1 && (Body.Centre - CurrentPath.Nodes[0].Centre).Length() > -1)
                {
                    CurrentPath.Nodes.RemoveAt(0);
                    if (CurrentPath.Nodes.Count >= 1) NextNode = CurrentPath.Nodes[0];
                }
                
            }
            else if (CurrentPath.Nodes.Count > 0)
            {
                AtEnemy = false;
            }
            else if (CurrentPath.Nodes[0].Coordinate != CurrentNode.Coordinate)
            {
                CreatePath();
            }
            else
            {
                AtEnemy = false;
            }
        }

        public override void Draw()
        {
            if (Body.Velocity.Length() > 0)
            {
                FaceDirection = Body.Velocity;
                FaceDirection.Normalize();
            }
            
            TopdownGame.SpriteBatch.Draw(Circle.DefaultTexture, new Rectangle((int)(Body.Centre.X - Body.Radius), (int)(Body.Centre.Y - Body.Radius), (int)Body.Width, (int)Body.Width), Color.Yellow);

            TopdownGame.SpriteBatch.Draw(Texture, new Rectangle((int)(Body.Position.X), (int)(Body.Centre.Y), (int)Body.Width, (int)Body.Height), TextureRect, Color.White, (float)Math.Atan2(FaceDirection.X, -FaceDirection.Y) - (float)(Math.PI/2), new Vector2(TextureRect.Width / 2, TextureRect.Height / 2), SpriteEffects.None, 0);
        }

        public override void Collisions()
        {
        }
    }
}
