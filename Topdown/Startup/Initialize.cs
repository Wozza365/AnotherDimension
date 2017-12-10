using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Topdown.AI;
using Topdown.Other;
using Topdown.Physics;
using Topdown.Sprites;

namespace Topdown
{
    public partial class TopdownGame
    {
        public Path Path { get; set; }
        protected override void Initialize()
        {
            GameState = GameState.LOADING;

            Screen = new Rectangle(0, 0, 1280, 800);
            ScreenCentre = new Vector2(Screen.Width / 2, Screen.Height / 2);

            _graphics.PreferredBackBufferWidth = Screen.Width;
            _graphics.PreferredBackBufferHeight = Screen.Height;
            _graphics.ApplyChanges();

            World.Gravity = new Vector2(0);
            World.SpaceFriction = 0.8f;
            Sprites = new List<Sprite>();
            Hero = new Hero(this, new Vector2(32, 415), new Vector2(32, 32), new Vector2(0.2f, 0.2f), 1)
            {
                TextureRect = new Rectangle(0, 1088, 128, 192),
                Visible = true
            };
            Sprites.Add(Hero);

            Enemy e = new Enemy(this, new Vector2(400, 780), new Vector2(24), new Vector2(0.2f), 1);
            Sprites.Add(e);
            //Hero.CreatePath();

            //Path = AStar.GenerateAStarPath(Hero, Sprites.First(x => x.SpriteType == SpriteTypes.Portal));
            //foreach (var n in Path.Nodes)
            //{
            //    if (n.Parent != null)
            //    {
            //        Debug.AddLine(n.Coordinate, n.Parent.Coordinate, 10);
            //    }
            //}
            
            //Circle c = new Circle(this, new Vector2(500, 550), 25, 50, false, Vector2.One);
            //c = new Circle(this, new Vector2(250, 300), 50, 10, false, new Vector2(1f));
            //c.Body.Velocity.X = 5;
            //c.Body.Velocity.Y = 5;

            //Sprites.Add(c);

            base.Initialize();
        }
    }
}