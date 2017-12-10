using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Topdown.Physics;

namespace Topdown.Sprites
{
    public class Portal : Sprite
    {
        public Portal(Vector2 position)
        {
            Body = new Body(this)
            {
                Position = position
            };
            SpriteType = SpriteTypes.Portal;
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
