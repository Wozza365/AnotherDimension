using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;
using Game.AI;

namespace Game.Other
{
    /// <summary>
    /// An imported tilemap from Tiled, has information on how to extract from the sprite sheet and how to draw the map correctly
    /// </summary>
    public class TiledMap
    {
        public MainGame Game { get; set; }
        public TmxMap Map { get; set; }
        public TmxTileset Tileset { get; set; }
        public Texture2D TilesetTexture { get; set; }
        public List<Node> PathTiles { get; set; } = new List<Node>();

        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int TilesetTilesWide { get; set; }
        public int TilesetTilesHeight { get; set; }
        public double ScreenTileRatio { get; set; } = 1.6;
        public int TileSpacing { get; set; } = 10;

        /// <summary>
        /// Draw a map imported from tiled
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used to draw</param>
        public void DrawMap(SpriteBatch spriteBatch)
        {
            for (int j = 0; j < Map.Layers.Count; j++)
            {
                if (Map.Layers[j].Visible)
                {
                    for (int i = 0; i < Map.Layers[j].Tiles.Count; i++)
                    {
                        int gid = Map.Layers[j].Tiles[i].Gid;

                        if (gid != 0)
                        {
                            //Don't declare variabbles outside the loops
                            //Has no affect on C# performance as Loop-Invariant Code Motion is performed at compile time.
                            int tileFrame = gid - 1;
                            int col = tileFrame % TilesetTilesWide;
                            int row = (int)Math.Floor((double)tileFrame / (double)TilesetTilesWide);

                            float x = (i % Map.Width) * TileWidth;
                            float y = (float)Math.Floor(i / (double)Map.Width) * TileHeight;

                            Rectangle tilesetRec = new Rectangle((TileWidth + TileSpacing) * col, (TileHeight + TileSpacing) * row, TileWidth, TileHeight);

                            spriteBatch.Draw(TilesetTexture, new Rectangle((int)(x / ScreenTileRatio), (int)(y / ScreenTileRatio), (int)(TileWidth / ScreenTileRatio), (int)(TileHeight / ScreenTileRatio)), tilesetRec, Color.White);
                        }
                    }
                }
            }
        }
    }
}