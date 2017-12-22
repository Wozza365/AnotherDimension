using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.AI;
using Game.Misc;
using Game.Other;
using Game.Physics;
using Game.Sprites;

namespace Game
{
    public partial class MainGame
    {
        public Path Path { get; set; }
        public static Dictionary<WeaponTypes, BulletConfig> BulletConfigs { get; set; } = new Dictionary<WeaponTypes, BulletConfig>();
        public static Dictionary<PowerupType, PowerupConfig> PowerupConfigs { get; set; } = new Dictionary<PowerupType, PowerupConfig>();
        public static bool SwitchMaps { get; set; }
        protected override void Initialize()
        {
            //Set basic settings regardless of current map.
            Screen = new Rectangle(0, 0, 1280, 800);
            ScreenCentre = new Vector2(Screen.Width / 2, Screen.Height / 2);

            _graphics.PreferredBackBufferWidth = Screen.Width;
            _graphics.PreferredBackBufferHeight = Screen.Height;
            _graphics.ApplyChanges();

            //Single white pixel texture
            //really useful for drawing lines or basic objects for debugging
            WhitePixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            WhitePixel.SetData(new[] { Color.White });

            LoadTextures();
            InitialiseTopdown();
            base.Initialize();
        }

        private void InitialiseTopdown()
        {
            GameState = GameState.PLAYINGTOPDOWN;

            //We don't use gravity in this world as its topdown
            World.Gravity = new Vector2(0);
            //Set space friction, so this is how much an object will decelarate by, by default. Prevents permanent sliding
            World.SpaceFriction = 0.95f;
            Sprites = new List<Sprite>();

            //Set up the scene controller with drop rate of powerups and new enemies
            SceneController.Game = this;
            SceneController.DropRate = 200;
            SceneController.EnemyRate = 200;

            //Configs for bullets and powerups
            //Change these numbers to customise the effects of them
            BulletConfig rpg = new BulletConfig(BulletTypes.Rocket, 25, Rocket, new Rectangle(0, 0, 20, 20), 500, 0, new TimeSpan(0, 0, 0, 1));
            BulletConfig smg = new BulletConfig(BulletTypes.Bullet, 40, Bullet, new Rectangle(0, 0, 20, 20), 50, 0, new TimeSpan(0, 0, 0, 0, 50));
            BulletConfig pistol = new BulletConfig(BulletTypes.Bullet, 40, Bullet, new Rectangle(0, 0, 20, 20), 60, 0, new TimeSpan(0, 0, 0, 0, 200));

            PowerupConfig rpgP = new PowerupConfig(PowerupType.RPG, RPG, new Rectangle(8, 8, 32, 32), 20, 1, new TimeSpan(0, 0, 0, 6), 0);
            PowerupConfig smgP = new PowerupConfig(PowerupType.SMG, SMG, new Rectangle(8, 8, 32, 32), 500, 1, new TimeSpan(0, 0, 0, 5), 0);
            PowerupConfig ammoP = new PowerupConfig(PowerupType.Ammo, Pistol, new Rectangle(8, 8, 32, 32), 100, 1, new TimeSpan(0, 0, 0, 8), 0);
            PowerupConfig speedP = new PowerupConfig(PowerupType.Speed, Speed, new Rectangle(8, 8, 32, 32), 0, 0.2f, new TimeSpan(0, 0, 0, 6), 0);
            PowerupConfig healthP = new PowerupConfig(PowerupType.Health, Health, new Rectangle(8, 8, 32, 32), 0, 1, new TimeSpan(0, 0, 0, 5), 50);

            BulletConfigs = new Dictionary<WeaponTypes, BulletConfig>
            {
                { WeaponTypes.RPG, rpg },
                { WeaponTypes.SMG, smg },
                { WeaponTypes.Pistol, pistol }
            };

            PowerupConfigs = new Dictionary<PowerupType, PowerupConfig>
            {
                { PowerupType.RPG, rpgP },
                { PowerupType.SMG, smgP },
                { PowerupType.Ammo, ammoP },
                { PowerupType.Speed, speedP },
                { PowerupType.Health, healthP }
            };

            TopdownHero = new TopdownHero(this, new Vector2(32, 415), new Vector2(32, 32), new Vector2(0.2f, 0.2f), 1)
            {
                Texture = Player,
                TextureRect = Player.Bounds,
                Visible = true
            };
            Sprites.Add(TopdownHero);

            //Add some starter enemies
            Enemy e = new Enemy(this, Zombie, Zombie.Bounds, new Vector2(400, 780), new Vector2(30), new Vector2(0.1f), 1),
                  e2 = new Enemy(this, Zombie, Zombie.Bounds, new Vector2(500, 780), new Vector2(30), new Vector2(0.1f), 1),
                  e3 = new Enemy(this, Zombie, Zombie.Bounds, new Vector2(800, 780), new Vector2(30), new Vector2(0.1f), 1);
            Sprites.AddRange(new List<Enemy>() { e, e2, e3 });

            //Add moving box obstacles
            Box b = new Box(this, BoxTex, new Vector2(30), 16, new Vector2(0.2f), 0.2f);
            Box b2 = new Box(this, BoxTex, new Vector2(550,600), 16, new Vector2(0.2f), 0.2f);
            Box b3 = new Box(this, BoxTex, new Vector2(770, 80), 16, new Vector2(0.2f), 0.2f);
            Sprites.AddRange(new List<Box>() {b, b2, b3 });
            AStar.TileSize = 40;
        }

        private void InitialisePlatformer()
        {
            //Setup gravity, also uses a lower air friction
            //0.99f means about a 50% reduction per second at 60 fps
            World.Gravity = new Vector2(0, 9.8f);
            World.SpaceFriction = 0.99f;
            Sprites = new List<Sprite>();
            GameState = GameState.PLAYINGPLATFORMER;
            AStar.TileSize = 32;

            //Add our hero to map
            PlatformerHero = new PlatformerHero(this, new Vector2(100, 375), new Vector2(32, 32), new Vector2(0.2f, 0.2f), 1)
            {
                Texture = Characters,
                TextureRect = new Rectangle(0, 1088, 128, 192),
                Visible = true
            };
            Sprites.Add(PlatformerHero);
        }
    }
}