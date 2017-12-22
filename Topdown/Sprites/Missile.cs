using System;
using System.Collections.Generic;
using System.Linq;
using Game.AI;
using Game.Misc;
using Game.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Sprites
{
    /// <summary>
    /// Missile from a missile launcher
    /// Uses AI to navigate the map but doesn't work as expected so is disabled
    /// </summary>
    public class Missile : AISprite
    {

        public Dictionary<SpriteTypes, int> Targets = new Dictionary<SpriteTypes, int>();
        public bool AtEnemy { get; set; } = false;
        public Vector2 FaceDirection { get; set; } = Vector2.UnitY;

        public Missile(MainGame game, Texture2D tex, Vector2 position, Vector2 size, Vector2 initialVelocity)
        {
            Game = game;
            Texture = tex;
            SpriteType = SpriteTypes.CannonBall;
            var vertices = new List<Vector2>
            {
                position,
                new Vector2(position.X + size.X, position.Y),
                new Vector2(position.X + size.X, position.Y + size.Y),
                new Vector2(position.X, position.Y + size.X)
            };
            
            Body = new Body(this)
            {
                Gravity = World.Gravity,
                Bounce = new Vector2(0.6f),
                Mass = float.MaxValue,
                Enabled = true,
                Shape = Shape.Circle,
                Position = position,
                Radius = size.X / 2,
                Static = false,
                Friction = 0.5f,
                Guid = Guid,
                Velocity = initialVelocity,
                MaxVelocity = initialVelocity,
                Rotation = 1
            };
            Targets.Add(SpriteTypes.PlatformerHero, 100);
        }
        public void CreatePath()
        {
            //Get all of our targets, including the wander nodes
            List<Sprite> spriteTargets = MainGame.Sprites.Where(x => Targets.Keys.Contains(x.SpriteType)).ToList();

            //remove the player if they are far away and create a more friendly Target for each
            //spriteTargets.RemoveAll(x => x.SpriteType == SpriteTypes.PlatformerHero && Vector2.Distance(Body.Position, x.Body.Position) > 500);
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
            //Though we only have one player, this would make adding a second much easier, instead of just picking the first player each time
            if (CurrentPath == null || CurrentPath.Nodes.Count <= 1)
            {
                //Shuffle the list if the hero isnt in it
                if (targets.All(x => x.SpriteType != SpriteTypes.PlatformerHero))
                {
                    //Effective method to shuffle a list
                    Random r = new Random();
                    targets = targets.OrderBy(x => r.Next()).ToList();
                }

                CurrentPath = AStar.GenerateAStarPath(this, targets.First().Sprite);
            }

            if (CurrentPath.Nodes.Count == 0)
            {
                return; //at player/target
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
                    //increase steering factor when at the enemy
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
            CurrentNode.Coordinate = new Vector2((int)(Body.Centre.X / 32), (int)(Body.Centre.Y / 32));
            if ((CurrentPath.Nodes.Count == 1 || CurrentPath.Nodes.Count == 0) && TargetType == SpriteTypes.PlatformerHero)
            {
                AtEnemy = true;
            }
            else if (CurrentPath.Nodes.Count == 1 || CurrentPath.Nodes.Count == 0)
            {
                //for safety we make sure we've got a path, even if its to the same coordinate
                CreatePath();
            }
            else if (CurrentPath.Nodes[0].Coordinate == CurrentNode.Coordinate)
            {
                //remove the node we've reached and find our next node to travel towards
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
            //we've possibly gone off course
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
            MainGame.SpriteBatch.Draw(Texture, new Rectangle((int)(Body.Position.X), (int)(Body.Position.Y), (int)Body.Width, (int)Body.Height), Texture.Bounds, Color.White);
        }

        public override void Collisions()
        {
            Vector2 result = new Vector2();
            float distance = 0;
            for (var i = 0; i < MainGame.Sprites.Count; i++)
            {
                var s = MainGame.Sprites[i];
                if (s.SpriteType == SpriteTypes.Gem)
                    continue;
                if (s.SpriteType != SpriteTypes.PlatformerHero)
                {
                    //explosive damage radius
                    float dist = Vector2.Distance(MainGame.Sprites.First(x => x.SpriteType == SpriteTypes.PlatformerHero).Body.Centre, Body.Centre);
                    if (dist < 50)
                    {
                        ((PlatformerHero)MainGame.Sprites.First(x => x.SpriteType == SpriteTypes.PlatformerHero)).Health -= (50 - (int)dist);
                    }
                    
                    if (!s.Equals(this) && World.Intersects(Body, s.Body, ref result, ref distance))
                    {
                        MainGame.Sprites.Remove(this);
                    }
                }
                else
                {
                    //direct hit, high damage
                    if (!s.Equals(this) && World.Intersects(Body, s.Body, ref result, ref distance))
                    {
                        ((PlatformerHero)MainGame.Sprites.First(x => x.SpriteType == SpriteTypes.PlatformerHero)).Health -= 50;
                        MainGame.Sprites.Remove(this);
                    }
                }
            }
        }
    }
}
