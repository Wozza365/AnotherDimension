using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Topdown.AI;
using Topdown.Misc;
using Topdown.Other;
using Topdown.Physics;
using Topdown.Sprites;

namespace Topdown
{
    public partial class TopdownGame
    {
        public Path Path { get; set; }
        public static Dictionary<WeaponTypes, BulletConfig> BulletConfigs { get; set; } = new Dictionary<WeaponTypes, BulletConfig>();
        public static Dictionary<PowerupType, PowerupConfig> PowerupConfigs { get; set; } = new Dictionary<PowerupType, PowerupConfig>();
        protected override void Initialize()
        {
            GameState = GameState.LOADING;

            Screen = new Rectangle(0, 0, 1280, 800);
            ScreenCentre = new Vector2(Screen.Width / 2, Screen.Height / 2);

            _graphics.PreferredBackBufferWidth = Screen.Width;
            _graphics.PreferredBackBufferHeight = Screen.Height;
            _graphics.ApplyChanges();

            World.Gravity = new Vector2(0);
            World.SpaceFriction = 0.95f;
            Sprites = new List<Sprite>();
            SceneController.Game = this;
            SceneController.DropRate = 100;

            WhitePixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            WhitePixel.SetData(new[] { Color.White });

            BulletConfig rpg = new BulletConfig(BulletTypes.Rocket, 50f, WhitePixel, new Rectangle(0, 0, 10, 10), 500, 0, new TimeSpan(0, 0, 0, 2));
            BulletConfig smg = new BulletConfig(BulletTypes.Bullet, 500, WhitePixel, new Rectangle(0, 0, 10, 10), 50, 0, new TimeSpan(0, 0, 0, 0, 200));
            BulletConfig pistol = new BulletConfig(BulletTypes.Bullet, 100, WhitePixel, new Rectangle(0, 0, 20, 20), 60, 0, new TimeSpan(0, 0, 0, 0, 400));

            PowerupConfig rpgP = new PowerupConfig(PowerupType.RPG, WhitePixel, new Rectangle(0, 0, 40, 40), 20, 1);
            PowerupConfig smgP = new PowerupConfig(PowerupType.SMG, WhitePixel, new Rectangle(0, 0, 40, 40), 500, 1);
            PowerupConfig ammoP = new PowerupConfig(PowerupType.Ammo, WhitePixel, new Rectangle(0, 0, 40, 40), 100, 1);
            PowerupConfig speedP = new PowerupConfig(PowerupType.Speed, WhitePixel, new Rectangle(0, 0, 40, 40), 0, 2);

            BulletConfigs.Add(WeaponTypes.RPG, rpg);
            BulletConfigs.Add(WeaponTypes.SMG, smg);
            BulletConfigs.Add(WeaponTypes.Pistol, pistol);

            PowerupConfigs.Add(PowerupType.RPG, rpgP);
            PowerupConfigs.Add(PowerupType.SMG, smgP);
            PowerupConfigs.Add(PowerupType.Ammo, ammoP);
            PowerupConfigs.Add(PowerupType.Speed, speedP);

            Hero = new Hero(this, new Vector2(32, 415), new Vector2(32, 32), new Vector2(0.2f, 0.2f), 1)
            {
                TextureRect = new Rectangle(0, 1088, 128, 192),
                Visible = true
            };
            Sprites.Add(Hero);

            Enemy e = new Enemy(this, new Vector2(400, 780), new Vector2(40), new Vector2(0.2f), 1);
            Sprites.Add(e);

            base.Initialize();
        }
    }
}