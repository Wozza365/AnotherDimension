using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace Topdown.Other
{
    public class TiledMap
    {
        public TmxMap Map { get; set; }
        public TmxTileset Tileset { get; set; }
        public Texture2D TilesetTexture { get; set; }

        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int TilesetTilesWide { get; set; }
        public int TilesetTilesHeight { get; set; }

        public void DrawMap(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Map.Layers[0].Tiles.Count; i++)
            {
                int gid = Map.Layers[0].Tiles[i].Gid;

                if (gid != 0)
                {
                    int tileFrame = gid - 1;
                    int col = tileFrame % TilesetTilesWide;
                    int row = (int) Math.Floor((double) tileFrame / (double) TilesetTilesWide);

                    float x = (i % Map.Width) * TileWidth;
                    float y = (float) Math.Floor(i / (double) Map.Width) * TileHeight;

                    Rectangle tilesetRec = new Rectangle(TileWidth * col, TileHeight * row, TileWidth, TileHeight);

                    spriteBatch.Draw(TilesetTexture, new Rectangle((int) x / 2, (int) y / 2, TileWidth / 2, TileHeight / 2), tilesetRec, Color.White);
                }
            }
            for (int i = 0; i < Map.Layers[1].Tiles.Count; i++)
            {
                int gid = Map.Layers[1].Tiles[i].Gid;

                if (gid != 0)
                {
                    int tileFrame = gid - 1;
                    int col = tileFrame % TilesetTilesWide;
                    int row = (int)Math.Floor((double)tileFrame / (double)TilesetTilesWide);

                    float x = (i % Map.Width) * TileWidth;
                    float y = (float)Math.Floor(i / (double)Map.Width) * TileHeight;

                    Rectangle tilesetRec = new Rectangle(TileWidth * col, TileHeight * row, TileWidth, TileHeight);

                    spriteBatch.Draw(TilesetTexture, new Rectangle((int)x / 2, (int)y / 2, TileWidth / 2, TileHeight / 2), tilesetRec, Color.White);
                }
            }
            for (int i = 0; i < Map.Layers[2].Tiles.Count; i++)
            {
                int gid = Map.Layers[2].Tiles[i].Gid;

                if (gid != 0)
                {
                    int tileFrame = gid - 1;
                    int col = tileFrame % TilesetTilesWide;
                    int row = (int)Math.Floor((double)tileFrame / (double)TilesetTilesWide);

                    float x = (i % Map.Width) * TileWidth;
                    float y = (float)Math.Floor(i / (double)Map.Width) * TileHeight;

                    Rectangle tilesetRec = new Rectangle(TileWidth * col, TileHeight * row, TileWidth, TileHeight);

                    spriteBatch.Draw(TilesetTexture, new Rectangle((int)x / 2, (int)y / 2, TileWidth / 2, TileHeight / 2), tilesetRec, Color.White);
                }
            }
        }
    }
}
