using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Game.Other;
using Game.Sprites;

namespace Game.Misc
{
    /// <summary>
    /// Handles a few different things in the scene
    /// </summary>
    public static class SceneController
    {
        private static Random Random { get; set; } = new Random();
        public static MainGame Game { get; set; }

        /// <summary>
        /// 1 = 1 drop per frame
        /// 100 = 1 drop per 100 frames
        /// </summary>
        public static int DropRate { get; set; }
        public static int EnemyRate { get; set; }

        public static int MaxDrops { get; set; } = 5;
        public static int MaxEnemies { get; set; } = 25;
        public static int GemCount { get; set; } = 0;

        /// <summary>
        /// Call this each frame
        /// </summary>
        public static void UpdateCurrentScene()
        {
            if (MainGame.GameState == GameState.PLAYINGPLATFORMER)
            {
                //Unlock the portal if the player has collected all of the gems in the platformer
                if (GemCount == 0)
                {
                    ((Portal)MainGame.Sprites.First(x => x.SpriteType == SpriteTypes.Portal)).Activated = true;
                    MainGame.Sprites.First(x => x.SpriteType == SpriteTypes.Portal).Body.Enabled = true;
                }
            }
            else if(MainGame.GameState == GameState.PLAYINGTOPDOWN)
            {
                //Drop powerups or new enemies based on a random number between 0 and their rate, we check if its in the middle of 0 and chosen value
                int result = Random.Next(0, DropRate);
                if (result == DropRate / 2 && MainGame.Sprites.Count(x => x.SpriteType == SpriteTypes.Powerup) < MaxDrops)
                {
                    DropPowerup();
                }
                result = Random.Next(EnemyRate);
                if (result == EnemyRate / 2 && MainGame.Sprites.Count(x => x.SpriteType == SpriteTypes.Enemy) < MaxEnemies)
                {
                    AddEnemy();
                }
            }
        }
        
        /// <summary>
        /// Drop a random powerup type at a random coordinate on the map
        /// </summary>
        public static void DropPowerup()
        {
            var coords = MainGame.ActiveMap.PathTiles[Random.Next(MainGame.ActiveMap.PathTiles.Count)].Coordinate;
            PowerupConfig pc = MainGame.PowerupConfigs.ElementAt(Random.Next(MainGame.PowerupConfigs.Count)).Value;
            Powerup p = new Powerup(Game, coords * 40, new Vector2(40, 40), pc);
            MainGame.Sprites.Add(p);
        }

        /// <summary>
        /// Add a new enemy onto the map at a random position
        /// </summary>
        public static void AddEnemy()
        {
            var coords = MainGame.ActiveMap.PathTiles[Random.Next(MainGame.ActiveMap.PathTiles.Count)].Coordinate;
            Enemy e = new Enemy(Game, MainGame.Zombie, MainGame.Zombie.Bounds, new Vector2(coords.X * 40 + 20, coords.Y * 40 + 20), new Vector2(30), new Vector2(0.1f), 1);
            e.CreatePath();
            MainGame.Sprites.Add(e);
        }
    }
}
