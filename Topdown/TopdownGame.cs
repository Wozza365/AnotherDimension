using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Topdown.AI;
using Topdown.Input;
using Topdown.Other;
using Topdown.Physics;
using Topdown.Sprites;

namespace Topdown
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public partial class TopdownGame : Game
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
        public static Hero Hero { get; set; }
        public static List<WanderNode> WanderNodes { get; set; } = new List<WanderNode>();

        public TopdownGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Update(GameTime gameTime)
        {
            float frameTime = 1.0f / (float)gameTime.ElapsedGameTime.TotalSeconds;
            InputManager.CheckInputs();
            World.ApplyGravity(Sprites, frameTime);
            World.ApplyVelocity(Sprites, frameTime);
            for (int i = 0; i < Sprites.Count; i++)
            {
                Sprites[i].Control();
                Sprites[i].Collisions();
                Sprites[i].Update();
            }
            
            World.ClearCollisions();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            SpriteBatch.Begin();
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
