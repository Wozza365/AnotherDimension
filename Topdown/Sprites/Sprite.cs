using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Game.Physics;
using Game.Sprites;

namespace Game
{
    /// <summary>
    /// Base Sprite class, every sprite should inherit this class
    /// </summary>
    public abstract class Sprite
    {
        public MainGame Game { get; set; }
        public Body Body { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle TextureRect { get; set; }
        public bool Visible { get; set; }
        public Guid Guid { get; set; }
        public bool OnGround { get; set; }
        public SpriteTypes SpriteType { get; set; }

        public List<Action> OnUpdate = new List<Action>();
        public float SpaceFriction { get; set; } = 1;
        

        public abstract void Control();

        public abstract void Update();

        public abstract void Draw();

        /// <summary>
        /// Give every sprite its own collision function to allow custom collisions
        /// Eg something that may go through walls, or needs to not resolve collisions
        /// </summary>
        public abstract void Collisions();

        public virtual void AddToList(ref List<Sprite> list)
        {
            list.Add(this);
        }
    }
}