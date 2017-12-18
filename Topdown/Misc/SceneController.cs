using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Topdown.Other;
using Topdown.Sprites;

namespace Topdown.Misc
{
    public static class SceneController
    {
        private static Random Random { get; set; } = new Random();
        public static TopdownGame Game { get; set; }
        
        /// <summary>
        /// 1 = 1 drop per frame
        /// 100 = 1 drop per 100 frames
        /// </summary>
        public static int DropRate { get; set; }
        public static int EnemyRate { get; set; }

        public static int MaxDrops { get; set; } = 5;
        public static int MaxEnemies { get; set; } = 25;
        
        public static void UpdateCurrentScene()
        {
            int result = Random.Next(0, DropRate);
            if (result == DropRate / 2 && TopdownGame.Sprites.Count(x => x.SpriteType == SpriteTypes.Powerup) < MaxDrops)
            {
                DropPowerup();
            }
            result = Random.Next(EnemyRate);
            if (result == EnemyRate / 2 && TopdownGame.Sprites.Count(x => x.SpriteType == SpriteTypes.Enemy) < MaxEnemies)
            {
                AddEnemy();
            }
        }

        public static void DropPowerup()
        {
            var coords = TopdownGame.ActiveMap.PathTiles[Random.Next(TopdownGame.ActiveMap.PathTiles.Count)].Coordinate;
            PowerupConfig pc = TopdownGame.PowerupConfigs.ElementAt(Random.Next(TopdownGame.PowerupConfigs.Count)).Value;
            Powerup p = new Powerup(Game, coords * 40, new Vector2(40, 40), pc);
            TopdownGame.Sprites.Add(p);
        }

        public static void AddEnemy()
        {
            var coords = TopdownGame.ActiveMap.PathTiles[Random.Next(TopdownGame.ActiveMap.PathTiles.Count)].Coordinate;
            Enemy e = new Enemy(Game, TopdownGame.Zombie, TopdownGame.Zombie.Bounds, new Vector2(coords.X * 40 + 20, coords.Y * 40 + 20), new Vector2(30), new Vector2(0.1f), 1);
            e.CreatePath();
            TopdownGame.Sprites.Add(e);
        }
    }
}
