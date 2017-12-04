using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Topdown;
using Topdown.Other;
using Topdown.Physics;
using Topdown.Sprites.Shapes;
using TiledSharp;

namespace Topdown
{
    public partial class TopdownGame
    {
        public Texture2D WhitePixel { get; set; }
        public Texture2D ControlImage { get; set; }
        public Texture2D SpriteSheet1 { get; set; }
        public Texture2D Characters { get; set; }
        public Texture2D Background { get; set; }
        
        public TiledMap ActiveMap { get; set; }
        public TiledMap Map1 { get; set; }
        
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            
            LoadTextures();
            LoadMaps();
            AddTexturesToSprites();
            LoadFonts();

            ActiveMap = Map1;
            GameState = GameState.PLAYING;
        }

        protected override void UnloadContent()
        {
            
        }

        private void AddTexturesToSprites()
        {
            Hero.Texture = Characters;
        }

        private void LoadTextures()
        {
            ControlImage = Content.Load<Texture2D>("Controls");
            Characters = Content.Load<Texture2D>("spritesheet_players");
            Background = Content.Load<Texture2D>("background0");
            Circle.DefaultTexture = Content.Load<Texture2D>("circle");
            
            WhitePixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            WhitePixel.SetData(new[] { Color.White });
        }

        private void LoadFonts()
        {
            Font = Content.Load<SpriteFont>("Font");
        }

        private void LoadMaps()
        {
            Map1 = new TiledMap();
            Map1.Map = new TmxMap("content/untitled.tmx");
            Map1.Tileset = Map1.Map.Tilesets["spritesheet_tiles"];
            Map1.TilesetTexture = Content.Load<Texture2D>(Map1.Tileset.Name);
            Map1.TileWidth = Map1.Map.Tilesets[0].TileWidth;
            Map1.TileHeight = Map1.Map.Tilesets[0].TileHeight;
            Map1.TilesetTilesWide = Map1.Tileset.Image.Width.Value / (Map1.TileWidth + 9);
            Map1.TilesetTilesHeight = Map1.Tileset.Image.Height.Value / (Map1.TileHeight + 9);
            GenerateMap(Map1);
        }

        private void GenerateMap(TiledMap map)
        {
            foreach (var obj in map.Map.ObjectGroups["Outside"].Objects)
            {
                List<Vector2> points = new List<Vector2>();
                foreach (var point in obj.Points)
                {
                    points.Add(new Vector2(((float)point.X + (float)obj.X) / 1.6f, ((float)point.Y + (float)obj.Y) / 1.6f));
                }
                var block = new Polygon(this, points, Color.White, true, new Vector2(1f), 0.7f)
                {
                };
                Sprites.Add(block);
            }
        }
    }
}
