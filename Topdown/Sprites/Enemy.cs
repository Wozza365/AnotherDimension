using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Topdown.AI;
using Topdown.Physics;

namespace Topdown.Sprites
{
    public class Enemy : AISprite
    {
        public static Dictionary<SpriteTypes, int> Targets = new Dictionary<SpriteTypes, int>();
        public Enemy(TopdownGame game, Vector2 position, Vector2 size, Vector2 bounce, float friction, float gravityMultiplier = 1)
        {
            Game = game;
            Guid = new Guid();
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
        }

        public void CreatePath()
        {
            Targets.Add(SpriteTypes.Hero, 10);
            List<Sprite> spriteTargets = TopdownGame.Sprites.Where(x => Targets.Keys.Contains(x.SpriteType)).ToList();
            spriteTargets.RemoveAll(x => x.SpriteType == SpriteTypes.Hero && Vector2.Distance(Body.Position, x.Body.Position) > 500);
            List<Target> targets = spriteTargets.Select(x => new Target()
            {
                Distance = Vector2.Distance(Body.Position, x.Body.Position),
                SpriteType = x.SpriteType,
                Weight = 1/Targets.Where(y => y.Key == x.SpriteType).Select(y => y.Value).First(),
                Sprite = x
            }).ToList();
            targets = targets.OrderBy(x => (1 / x.Weight) * x.Distance).ToList();
            CurrentPath = AStar.GenerateAStarPath(this, targets.First().Sprite);
            NextNode = CurrentPath.Nodes[1];
            TargetNode = CurrentPath.Nodes.Last();
        }
        
        public override void Control()
        {
            
        }

        public override void Update()
        {
            
        }

        public override void Draw()
        {
            
        }

        public override void Collisions()
        {
            
        }
    }
}
