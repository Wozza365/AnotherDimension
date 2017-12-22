using System;
using System.Collections.Generic;
using System.Linq;
using Game.AI;
using Game.Misc;
using Game.Physics;
using Game.Sprites.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Sprites
{
    /// <summary>
    /// A patrolling enemy for platforms
    /// Currently broken, doesn't get the correct node.
    /// Searches entire list of wander nodes, need to add a way to add an A and B node to travel to for each surface
    /// </summary>
    public class PlatformEnemy : AISprite
    {
        public Dictionary<SpriteTypes, int> Targets = new Dictionary<SpriteTypes, int>();
        public bool AtEnemy { get; set; } = false;
        public int Health { get; set; }
        public Vector2 FaceDirection { get; set; } = Vector2.UnitY;
        public PlatformEnemy(MainGame game, Texture2D texture, Rectangle texRect, Vector2 position, Vector2 size, Vector2 bounce, float friction, float gravityMultiplier = 1)
        {
            //Standard setup of required properties
            Game = game;
            Guid = new Guid();
            SpriteType = SpriteTypes.Enemy;
            Health = 100;
            Texture = texture;
            TextureRect = texRect;
            
            Body = new Body(this)
            {
                MaxVelocity = new Vector2(4),
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
            Targets.Add(SpriteTypes.PlatformerHero, 100);
            Targets.Add(SpriteTypes.WanderNode, 1);
        }

        public void CreatePath()
        {
            List<WanderNode> wanderTargets = MainGame.PlatformerWanderNodes;
            
            List<Target> targets = wanderTargets.Select(x => new Target()
            {
                Distance = Vector2.Distance(Body.Position, x.Body.Position),
                SpriteType = x.SpriteType,
                Weight = Targets.Where(y => y.Key == x.SpriteType).Select(y => y.Value).First(),
                Sprite = x
            }).ToList();

            //trying to find the only one we can reach, but for some reason we can reach any
            targets = targets.OrderBy(x => (1 / x.Weight) * x.Distance).ToList();
            if (CurrentPath != null)
                CurrentPath.Nodes = new List<Node>();
            if (CurrentPath == null || CurrentPath.Nodes.Count <= 1)
            {
                Random r = new Random();
                targets = targets.OrderBy(x => x.Distance).ToList();
                //this should get the the node on the other side of the same platform, doesn't happen as algorithm is able to create a path to another point
                int i = 1;
                do
                {
                    CurrentPath = AStar.GenerateAStarPath(this, targets[i].Sprite);
                    i++;
                } while (!CurrentPath.Valid || CurrentPath.Nodes.Count == 0);
            }

            TargetType = targets.First().Sprite.SpriteType;
            TargetSprite = targets.First().Sprite;

            CurrentNode = CurrentPath.Nodes[0];
            NextNode = CurrentPath.Nodes[CurrentPath.Nodes.Count > 1 ? 1 : 0];
            TargetNode = CurrentPath.Nodes.Last();
        }

        public override void Control()
        {
            var direction = NextNode.Centre - Body.Centre;
            if (direction.X != 0 || direction.Y != 0)
            {
                direction.Normalize();
            }
            else
            {
                direction = CurrentNode.Centre - Body.Centre;
                direction.Normalize();
            }
            //decelerrates quickly and turns around
            Body.Velocity = Vector2.Normalize(Body.Velocity + direction) * Body.MaxVelocity;
        }

        public override void Update()
        {
            CurrentNode.Coordinate = new Vector2((int)(Body.Centre.X / 40), (int)(Body.Centre.Y / 40));
            if ((CurrentPath.Nodes.Count == 1 || CurrentPath.Nodes.Count == 0))
            {
                //Create new path on arrival
                CreatePath();
            }
            else if (CurrentPath.Nodes[0].Coordinate == CurrentNode.Coordinate)
            {
                //Remove nodes as we reach them
                if ((Body.Centre - CurrentPath.Nodes[0].Centre).Length() < 10 && (Body.Centre - CurrentPath.Nodes[0].Centre).Length() > -10)
                {
                    CurrentPath.Nodes.RemoveAt(0);
                    if (CurrentPath.Nodes.Count >= 1) NextNode = CurrentPath.Nodes[0];
                }
            }
        }

        public override void Draw()
        {
            if (Body.Velocity.Length() > 0)
            {
                FaceDirection = Body.Velocity;
                FaceDirection.Normalize();
            }

            MainGame.SpriteBatch.Draw(Circle.DefaultTexture, new Rectangle((int)(Body.Centre.X - Body.Radius), (int)(Body.Centre.Y - Body.Radius), (int)Body.Width, (int)Body.Width), Color.Yellow);

            MainGame.SpriteBatch.Draw(Texture, new Rectangle((int)(Body.Left + Body.Radius), (int)(Body.Top + Body.Radius), (int)Body.Width, (int)Body.Height), TextureRect, Color.White, (float)Math.Atan2(FaceDirection.X, -FaceDirection.Y) - (float)(Math.PI / 2), new Vector2(TextureRect.Width / 2, TextureRect.Height / 2), SpriteEffects.None, 0);
        }

        public override void Collisions()
        {
        }
    }
}
