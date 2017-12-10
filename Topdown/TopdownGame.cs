using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        public static Hero Hero { get; set; }

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
            Sprites.ForEach(s => s.Control());
            Sprites.ForEach(s => s.Collisions());
            Sprites.ForEach(s => s.Update());
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
            //Map1.PathTiles.ForEach(x => SpriteBatch.Draw(ControlImage, x, Color.White));
            Debug.Update(SpriteBatch, Font, WhitePixel);
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
