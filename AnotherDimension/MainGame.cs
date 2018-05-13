using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Input;
using Game.Misc;
using Game.Other;
using Game.Physics;
using Game.Sprites;

namespace Game
{
    public partial class MainGame : Microsoft.Xna.Framework.Game
    {
        //Generic properties
        GraphicsDeviceManager _graphics;
        public static SpriteBatch SpriteBatch { get; set; }

        public static Rectangle Screen { get; set; }
        public static Vector2 ScreenCentre { get; set; }

        private static GameState _gameState;
        public static GameState GameState
        {
            get => _gameState;
            set
            {
                PreviousGameState = _gameState;
                _gameState = value;
            }
        }

        public static GameState PreviousGameState { get; set; }
        public static List<Sprite> Sprites { get; set; }
        public static SpriteFont Font { get; set; }


        //Game specific properties

        public static TopdownHero TopdownHero { get; set; }
        public static PlatformerHero PlatformerHero { get; set; }
        public static List<WanderNode> WanderNodes { get; set; } = new List<WanderNode>();
        public static List<WanderNode> PlatformerWanderNodes { get; set; } = new List<WanderNode>();

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Core update loop of our game
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            float frameTime = 1.0f / (float)gameTime.ElapsedGameTime.TotalSeconds;
            InputManager.CheckInputs();
            World.ApplyGravity(Sprites, frameTime);
            World.ApplyVelocity(Sprites, frameTime);

            //These can be reordered for slightly different affects, I found to be the best order.
            //Uses for i loops to be able to remove sprites mid loop.
            for (int i = 0; i < Sprites.Count; i++)
            {
                Sprites[i].Control();
            }
            for (int i = 0; i < Sprites.Count; i++)
            {
                Sprites[i].Collisions();
            }
            for (int i = 0; i < Sprites.Count; i++)
            {
                Sprites[i].Update();
            }
            if (SwitchMaps)
            {
                SwapScenes();
                SwitchMaps = false;
            }

            SceneController.UpdateCurrentScene();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            SpriteBatch.Begin();
            //Draw background, map, sprites, ui and then debug information
            SpriteBatch.Draw(Background, Screen, Color.Gray);
            ActiveMap.DrawMap(SpriteBatch);
            Sprites.ForEach(x => x.Draw());
            UserInterface.UpdateInterface();
            Debug.Update(SpriteBatch, Font, WhitePixel);
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}