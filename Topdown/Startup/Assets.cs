using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Other;
using Game.Sprites.Shapes;
using TiledSharp;
using Game.AI;
using Game.Misc;
using Game.Sprites;
using Box = Game.Sprites.Box;

namespace Game
{
    public partial class MainGame
    {
        //All of our textures
        public static Texture2D WhitePixel { get; set; }
        public static Texture2D ControlImage { get; set; }
        public static Texture2D SpriteSheet1 { get; set; }
        public static Texture2D Characters { get; set; }
        public static Texture2D TopdownCharacters { get; set; }
        public static Texture2D Background { get; set; }
        public static Texture2D Pistol { get; set; }
        public static Texture2D SMG { get; set; }
        public static Texture2D RPG { get; set; }
        public static Texture2D Rocket { get; set; }
        public static Texture2D Bullet { get; set; }
        public static Texture2D Health { get; set; }
        public static Texture2D Speed { get; set; }
        public static Texture2D Zombie { get; set; }
        public static Texture2D Player { get; set; }
        public static Texture2D BoxTex { get; set; }
        public static Texture2D Turbine { get; set; }
        public static Texture2D Cannon { get; set; }
        public static Texture2D CannonBall { get; set; }

        //The game maps
        public static TiledMap ActiveMap { get; set; }
        public TiledMap TopdownMap { get; set; }
        public TiledMap PlatformMap { get; set; }

        public float TileScreenWidth => Screen.Width / 32;

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            
            LoadMaps();
            AddTexturesToSprites();
            LoadFonts();

            ActiveMap = TopdownMap;
            GameState = GameState.PLAYINGTOPDOWN;
        }

        protected override void UnloadContent()
        {

        }

        private void AddTexturesToSprites()
        {
            TopdownHero.Texture = Player;
        }

        private void LoadTextures()
        {
            //Load in our textures from the files
            ControlImage = Content.Load<Texture2D>("Controls");
            Characters = Content.Load<Texture2D>("spritesheet_players");
            TopdownCharacters = Content.Load<Texture2D>("spritesheet_characters");
            Background = Content.Load<Texture2D>("background0");
            Pistol = Content.Load<Texture2D>("revolver");
            SMG = Content.Load<Texture2D>("uzi");
            RPG = Content.Load<Texture2D>("panzerfaust");
            Rocket = Content.Load<Texture2D>("rocket");
            Bullet = Content.Load<Texture2D>("bulleta");
            Health = Content.Load<Texture2D>("health");
            Speed = Content.Load<Texture2D>("speed");
            Zombie = Content.Load<Texture2D>("zombie");
            Player = Content.Load<Texture2D>("player");
            Turbine = Content.Load<Texture2D>("turbine");
            Cannon = Content.Load<Texture2D>("cannon");
            CannonBall = Content.Load<Texture2D>("bulleta");
            SpriteSheet1 = Content.Load<Texture2D>("tilesheet_complete");
            BoxTex = Content.Load<Texture2D>("box");

            Circle.DefaultTexture = Content.Load<Texture2D>("circle");
        }

        private void LoadFonts()
        {
            Font = Content.Load<SpriteFont>("Font");
        }

        private void LoadMaps()
        {
            LoadTopdown();
        }

        private void LoadTopdown()
        {
            //Load the map imported from Tiled
            TopdownMap = new TiledMap()
            {
                Game = this,
                ScreenTileRatio = 1.6,
                TileSpacing = 10
            };
            TopdownMap.Map = new TmxMap("content/untitled.tmx");
            TopdownMap.Tileset = TopdownMap.Map.Tilesets["spritesheet_tiles"];
            TopdownMap.TilesetTexture = Content.Load<Texture2D>(TopdownMap.Tileset.Name);
            TopdownMap.TileWidth = TopdownMap.Map.Tilesets[0].TileWidth;
            TopdownMap.TileHeight = TopdownMap.Map.Tilesets[0].TileHeight;
            TopdownMap.TilesetTilesWide = TopdownMap.Tileset.Image.Width.Value / (TopdownMap.TileWidth + 9);
            TopdownMap.TilesetTilesHeight = TopdownMap.Tileset.Image.Height.Value / (TopdownMap.TileHeight + 9);
            GenerateTopdownMap(TopdownMap);
            ActiveMap = TopdownMap;
        }

        private void LoadPlatformer()
        {
            //Load the map imported from Tiled
            PlatformMap = new TiledMap()
            {
                Game = this,
                ScreenTileRatio = 2,
                TileSpacing = 0
            };
            PlatformMap.Map = new TmxMap("content/First.tmx");
            PlatformMap.Tileset = PlatformMap.Map.Tilesets["tilesheet_complete"];
            PlatformMap.TilesetTexture = Content.Load<Texture2D>(PlatformMap.Tileset.Name);
            PlatformMap.TileWidth = PlatformMap.Map.Tilesets[0].TileWidth;
            PlatformMap.TileHeight = PlatformMap.Map.Tilesets[0].TileHeight;
            PlatformMap.TilesetTilesWide = PlatformMap.Tileset.Image.Width.Value / PlatformMap.TileWidth;
            PlatformMap.TilesetTilesHeight = PlatformMap.Tileset.Image.Height.Value / PlatformMap.TileHeight;
            GeneratePlatformerMap(PlatformMap);
            LoadPlatformerObjects();
            ActiveMap = PlatformMap;

            //Must be loaded after map load
            //PlatformEnemy e = new PlatformEnemy(this, Zombie, Zombie.Bounds, new Vector2(864, 544), new Vector2(32), new Vector2(0.1f), 1);
            //e.CreatePath();
            //Sprites.AddRange(new List<PlatformEnemy>() { e/*,e2,e3*/});
            
        }

        private void LoadPlatformerObjects()
        {
            //Load in some sprites to the world that are not included in the imported Tiled map, but could be in future
            Turbine t = new Turbine(this, Turbine, new Vector2(544, 768), new Vector2(32, 32), 1.5f, 100);
            Turbine t2 = new Turbine(this, Turbine, new Vector2(512, 768), new Vector2(32, 32), 1.5f, 100);
            Turbine t3 = new Turbine(this, Turbine, new Vector2(480, 768), new Vector2(32, 32), 1.5f, 100);
            Sprites.AddRange(new List<Turbine>() { t, t2, t3 });
            Cannon c = new Cannon(this, Cannon, new Vector2(350, 160), new Vector2(48, 32));
            Sprites.Add(c);
            //MissileLauncher ml = new MissileLauncher(this, Cannon, new Vector2(571, 35), new Vector2(32, 32));
            //Sprites.Add(ml);
            Lever l = new Lever(this, SpriteSheet1, new Rectangle(576, 640, 64, 64), new Rectangle(704, 640, 64, 64), new Vector2(300, 176), new Vector2(32, 32), c);
            Sprites.Add(l);
            Box b = new Box(this, BoxTex, new Vector2(384, 410), 16, new Vector2(0.2f), 0.2f);
            Box b2 = new Box(this, BoxTex, new Vector2(1120, 736), 16, new Vector2(0.2f), 0.2f);
            Box b3 = new Box(this, BoxTex, new Vector2(1056, 192), 16, new Vector2(0.2f), 0.2f);
            Sprites.Add(b);
            Portal p = new Portal(this, SpriteSheet1, new Rectangle(1216, 384, 64, 64), new Vector2(1216, 64), new Vector2(32, 32));
            Sprites.Add(p);
            //Platform pl = new Platform(this, new Vector2(768, 480), new Vector2(32 * 6, 32), 6, SpriteSheet1, new Vector2(900, 480));
            //Sprites.Add(pl);
        }

        public void SwapScenes()
        {
            //Swapper method between dimensions, could be fleshed out further to add additional maps
            Sprites.Clear();
            if (GameState == GameState.PLAYINGTOPDOWN)
            {
                InitialisePlatformer();
                LoadPlatformer();
            }
            else if (GameState == GameState.PLAYINGPLATFORMER)
            {
                InitialiseTopdown();
                LoadTopdown();
            }
        }

        /// <summary>
        /// Load all of the map objects from the Tiled file into objects
        /// Divide by 1.6 as tiles are 64x64 but screen displays as 40x40
        /// </summary>
        /// <param name="map"></param>
        private void GenerateTopdownMap(TiledMap map)
        {
            TopdownMap.PathTiles = new List<Node>();
            //Areas not walkable on the maps
            foreach (var obj in map.Map.ObjectGroups["Outside"].Objects)
            {
                List<Vector2> points = new List<Vector2>();
                foreach (var point in obj.Points)
                {
                    points.Add(new Vector2(((float)point.X + (float)obj.X) / 1.6f, ((float)point.Y + (float)obj.Y) / 1.6f));
                }
                var block = new MapPiece(this, points, Color.White, true, new Vector2(1f), 0.7f)
                {
                    SpriteType = SpriteTypes.Outside
                };
                Sprites.Add(block);
            }
            //Areas walkable, used for the AI of the game
            foreach (var obj in map.Map.ObjectGroups["Safezones"].Objects)
            {
                var size = 64;
                var tileWide = obj.Width / size;
                var tilesHigh = obj.Height / size;

                for (int i = 0; i < tileWide; i++)
                {
                    for (int j = 0; j < tilesHigh; j++)
                    {
                        TopdownMap.PathTiles.Add(new Node()
                        {
                            Area = new Rectangle((int)((obj.X + (i * size)) / 1.6), (int)((obj.Y + (j * size)) / 1.6), (int)(size / 1.6), (int)(size / 1.6)),
                            Coordinate = new Vector2(i + (float)(obj.X / 64), j + (float)(obj.Y / 64))
                        });
                    }
                }
            }
            //Portals
            foreach (var obj in map.Map.ObjectGroups["Targets"].Objects)
            {
                var portal = new Portal(this, SpriteSheet1, new Rectangle(1216, 384, 64, 64), new Vector2((float)obj.X/1.6f, (float)obj.Y/1.6f), new Vector2((float)(obj.Width/1.6f), (float)(obj.Height/1.6f)));
                Sprites.Add(portal);
            }
            //Wander nodes for the zombie AI
            foreach (var obj in map.Map.ObjectGroups["WanderNodes"].Objects)
            {
                WanderNodes.Add(new WanderNode(this, new Vector2((float)(obj.X/ 1.6), (float)(obj.Y/ 1.6)), new Vector2((float)(obj.Width/1.6), (float) (obj.Height/ 1.6))));
            }

            AStar.MapNodes = TopdownMap.PathTiles;
            //Create our enemy paths now that they have a map to follow
            foreach (Enemy enemy in Sprites.Where(x => x.SpriteType == SpriteTypes.Enemy))
            {
                enemy.CreatePath();
            }
        }

        /// <summary>
        /// Load all of the map objects from the Tiled file into objects
        /// We divide by 2 a lot because the tileset and therefore the map was created at 64x64 per tile so everything needs to be decreased to 32px
        /// </summary>
        /// <param name="map"></param>
        private void GeneratePlatformerMap(TiledMap map)
        {
            PlatformMap.PathTiles = new List<Node>();
            //The main platforms/blocks of the map
            foreach (var obj in map.Map.ObjectGroups["Map"].Objects)
            {
                List<Vector2> points = new List<Vector2>();
                foreach (var point in obj.Points)
                {
                    points.Add(new Vector2(((float)point.X + (float)obj.X) / 2, ((float)point.Y + (float)obj.Y) / 2));
                }
                var block = new Polygon(this, points, Color.White, true, new Vector2(1f), 0.7f);
                Sprites.Add(block);
            }
            //Add all of the ladder sprites
            foreach (var obj in map.Map.ObjectGroups["Ladders"].Objects)
            {
                var ladder = new Ladder(this, new Vector2((float)obj.X / 2, (float)obj.Y / 2), new Vector2((float)obj.Width / 2, (float)obj.Height) / 2, true, new Vector2(1f), 0.7f);
                Sprites.Add(ladder);
            }
            //These are the paths that enemies that crawl across platformers can walk on
            //They are generally just the layer above the platforms
            foreach (var obj in map.Map.ObjectGroups["CrawlerPaths"].Objects)
            {
                var size = 64;
                var tileWide = obj.Width / size;
                var tilesHigh = obj.Height / size;

                for (int i = 0; i < tileWide; i++)
                {
                    for (int j = 0; j < tilesHigh; j++)
                    {
                        PlatformMap.PathTiles.Add(new Node()
                        {
                            Area = new Rectangle((int)((obj.X + (i * size)) / 2), (int)((obj.Y + (j * size)) / 2), (int)(size / 2), (int)(size / 2)),
                            Coordinate = new Vector2(i + (float)(obj.X / 64), j + (float)(obj.Y / 64))
                        });
                    }
                }
            }
            //The targets that the crawlers walk back and forward towards.
            foreach (var obj in map.Map.ObjectGroups["CrawlerTargets"].Objects)
            {
                PlatformerWanderNodes.Add(new WanderNode(this, new Vector2((float)(obj.X / 2), (float)(obj.Y / 2)), new Vector2((float)(obj.Width /2), (float)(obj.Height /2))));
            }

            //Add the gems
            for (int i = 0; i < map.Map.Layers[2].Tiles.Count; i++)
            {
                int gid = map.Map.Layers[2].Tiles[i].Gid;

                if (gid != 0)
                {
                    int tileFrame = gid - 1;
                    int col = tileFrame % map.TilesetTilesWide;
                    int row = (int)Math.Floor((double)tileFrame / (double)map.TilesetTilesWide);

                    float x = (i % map.Map.Width) * map.TileWidth;
                    float y = (float)Math.Floor(i / (double)map.Map.Width) * map.TileHeight;

                    Rectangle tilesetRec = new Rectangle(map.TileWidth * col, map.TileHeight * row, map.TileWidth, map.TileHeight);

                    Gem g = new Gem(this, map.TilesetTexture, tilesetRec, new Vector2(x / 2, y / 2), new Vector2(map.TileWidth / 2, map.TileHeight / 2));
                    Sprites.Add(g);
                    SceneController.GemCount++;
                }
            }
            //Set the active map in the AStar class
            AStar.MapNodes = PlatformMap.PathTiles;

        }
    }
}
