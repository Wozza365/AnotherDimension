using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.AI;
using Game.Misc;
using Game.Physics;
using Game.Sprites.Shapes;

namespace Game.Sprites
{
    /// <summary>
    /// An enemy for the topdown section
    /// Controlled by an AI
    /// Will target the player if its within range, but if not it will target a random 'wander node' on the map to walk to. This gives a zombie like effect
    /// Uses an indirect state machine, when targeting the player, it will walk and turn much faster, compared to the wander state which is slower
    /// </summary>
    public class Enemy : AISprite
    {
        public Dictionary<SpriteTypes, int> Targets = new Dictionary<SpriteTypes, int>();
        public bool AtEnemy { get; set; } = false;
        public int Health { get; set; }
        public Vector2 FaceDirection { get; set; } = Vector2.UnitY;

        public Enemy(MainGame game, Texture2D texture, Rectangle texRect, Vector2 position, Vector2 size, Vector2 bounce, float friction, float gravityMultiplier = 1)
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
            Targets.Add(SpriteTypes.TopdownHero, 100);
            Targets.Add(SpriteTypes.WanderNode, 1);
        }

        /// <summary>
        /// Generates the path to follow. Call this before allowing the game loop functions to execute
        /// </summary>
        public void CreatePath()
        {
            //Get all of our targets, including the wander nodes
            List<Sprite> spriteTargets = MainGame.Sprites.Where(x => Targets.Keys.Contains(x.SpriteType)).ToList();
            List<WanderNode> wanderTargets = MainGame.WanderNodes;
            wanderTargets.ForEach(x => spriteTargets.Add(x));

            //remove the player if they are far away and create a more friendly Target for each
            spriteTargets.RemoveAll(x => x.SpriteType == SpriteTypes.TopdownHero && Vector2.Distance(Body.Position, x.Body.Position) > 500);
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
                if (targets.All(x => x.SpriteType != SpriteTypes.TopdownHero))
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

            //Sets some of the AISprite properties
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
                var direction = NextNode.Centre - Body.Centre;
                //var direction = NextNode.Coordinate - CurrentNode.Coordinate;
                if (direction.X != 0 || direction.Y != 0)
                {
                    direction.Normalize();
                }
                else
                {
                    direction = CurrentNode.Centre - Body.Centre;
                    direction.Normalize();
                }
                //divide direction by 4 which is essentially our "steering" factor. The smaller the new value, the less affect the steering has
                //This creates a smoother looking path
                Body.Velocity = Vector2.Normalize(Body.Velocity + (direction / 4)) * Body.MaxVelocity;
            }
            else
            {
                var direction = TargetSprite.Body.Centre - Body.Centre;
                if (direction.X != 0 || direction.Y != 0)
                {
                    direction.Normalize();
                    
                    //Slightly higher direction change when close to the player to show its in a more active state
                    Body.Velocity = Vector2.Normalize(Body.Velocity + (direction / 2)) * Body.MaxVelocity;
                }
                else
                {
                    Body.Velocity = Vector2.Zero;
                }
            }
        }

        public override void Update()
        {
            //Dead
            if (Health <= 0)
            {
                MainGame.Sprites.Remove(this);
                return;
            }

            Body.MaxVelocity = TargetType == SpriteTypes.TopdownHero ? new Vector2(1.5f) : Vector2.One;
            
            //If hero has moved out of range then recalculate to a random wander node
            if (TargetType == SpriteTypes.TopdownHero && Vector2.Distance(Body.Centre, TargetSprite.Body.Centre) > 500)
            {
                CreatePath();
                return;
            }
            CurrentNode.Coordinate = new Vector2((int)(Body.Centre.X / 40), (int)(Body.Centre.Y / 40));
            //Reached the enemy
            if ((CurrentPath.Nodes.Count == 1 || CurrentPath.Nodes.Count == 0) && TargetType == SpriteTypes.TopdownHero)
            {
                AtEnemy = true;
            }
            else if (CurrentPath.Nodes.Count == 1 || CurrentPath.Nodes.Count == 0)
            {
                CreatePath();
            }
            else if (CurrentPath.Nodes[0].Coordinate == CurrentNode.Coordinate)
            {
                //uses a radius around the coordinate so that we don't need to directly hit the centre, allows for much smoother movement of corners.
                if ((Body.Centre - CurrentPath.Nodes[0].Centre).Length() < 10 && (Body.Centre - CurrentPath.Nodes[0].Centre).Length() > -10)
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
                //Gone off course so recalculate
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

            MainGame.SpriteBatch.Draw(Circle.DefaultTexture, new Rectangle((int)(Body.Centre.X - Body.Radius), (int)(Body.Centre.Y - Body.Radius), (int)Body.Width, (int)Body.Width), Color.Yellow);

            MainGame.SpriteBatch.Draw(Texture, new Rectangle((int)(Body.Left + Body.Radius), (int)(Body.Top + Body.Radius), (int)Body.Width, (int)Body.Height), TextureRect, Color.White, (float)Math.Atan2(FaceDirection.X, -FaceDirection.Y) - (float)(Math.PI / 2), new Vector2(TextureRect.Width / 2, TextureRect.Height / 2), SpriteEffects.None, 0);
        }

        public override void Collisions()
        {
            
        }
    }
}
