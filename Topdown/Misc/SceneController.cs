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
        public static Random Random { get; set; } = new Random();
        public static TopdownGame Game { get; set; }
        
        /// <summary>
        /// 1 = 1 drop per frame
        /// 100 = 1 drop per 100 frames
        /// </summary>
        public static int DropRate { get; set; }
        
        public static void UpdateCurrentScene()
        {
            int result = Random.Next(0, DropRate);
            if (result == DropRate / 2)
            {
                DropPowerup();
            }
        }

        public static void DropPowerup()
        {
            var coords = TopdownGame.ActiveMap.PathTiles[Random.Next(TopdownGame.ActiveMap.PathTiles.Count)].Coordinate;
            PowerupConfig pc = TopdownGame.PowerupConfigs.ElementAt(Random.Next(TopdownGame.PowerupConfigs.Count)).Value;
            Powerup p = new Powerup(Game, coords * 40, new Vector2(40, 40), pc);
            TopdownGame.Sprites.Add(p);
        }
    }
}
