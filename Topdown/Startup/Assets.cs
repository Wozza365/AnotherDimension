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
using Topdown.AI;
using Topdown.Sprites;

namespace Topdown
{
    public partial class TopdownGame
    {
        public Texture2D WhitePixel { get; set; }
        public Texture2D ControlImage { get; set; }
        public Texture2D SpriteSheet1 { get; set; }
        public Texture2D Characters { get; set; }
        public Texture2D Background { get; set; }

        public static TiledMap ActiveMap { get; set; }
        public TiledMap Map1 { get; set; }

        public float TileScreenWidth
        {
            get { return Screen.Width / 32; }
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            LoadTextures();
            LoadMaps();
            AddTexturesToSprites();
            LoadFonts();

            ActiveMap = Map1;
            GameState = GameState.PLAYINGTOPDOWN;
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
                var block = new MapPiece(this, points, Color.White, true, new Vector2(1f), 0.7f)
                {
                    SpriteType = SpriteTypes.Outside
                };
                Sprites.Add(block);
            }
            foreach (var obj in map.Map.ObjectGroups["Safezones"].Objects)
            {
                var size = 64;
                var tileWide = obj.Width / size;
                var tilesHigh = obj.Height / size;

                for (int i = 0; i < tileWide; i++)
                {
                    for (int j = 0; j < tilesHigh; j++)
                    {
                        Map1.PathTiles.Add(new Node()
                        {
                            Area = new Rectangle((int)((obj.X + (i * size)) / 1.6), (int)((obj.Y + (j * size)) / 1.6), (int)(size / 1.6), (int)(size / 1.6)),
                            Coordinate = new Vector2(i + (float)(obj.X / 64), j + (float)(obj.Y / 64))
                        });
                    }
                }
            }
            foreach (var obj in map.Map.ObjectGroups["Targets"].Objects)
            {
                var portal = new Portal(new Vector2((float)obj.X/1.6f, (float)obj.Y/1.6f), new Vector2((float)(obj.Width/1.6f), (float)(obj.Height/1.6f)));
                Sprites.Add(portal);
            }
            foreach (var obj in map.Map.ObjectGroups["WanderNodes"].Objects)
            {
                WanderNodes.Add(new WanderNode(this, new Vector2((float)(obj.X/ 1.6), (float)(obj.Y/ 1.6)), new Vector2((float)(obj.Width/1.6), (float) (obj.Height/ 1.6))));
            }

            AStar.MapNodes = Map1.PathTiles;
            Path = AStar.GenerateAStarPath(Hero, Sprites.First(x => x.SpriteType == SpriteTypes.Portal));
            Hero.CreatePath();
            foreach (Enemy enemy in Sprites.Where(x => x.SpriteType == SpriteTypes.Enemy))
            {
                enemy.CreatePath();
            }
            //int j2 = 0;
            //while (j2 < Path.Nodes.Count - 1)
            //{
            //    for (int i = j2 + 2; i < Path.Nodes.Count;i++)
            //    {
            //        List<Vector2> points = new List<Vector2>();
            //        var start = new Vector2(Path.Nodes[j2].Coordinate.X * TileScreenWidth + (TileScreenWidth/2), Path.Nodes[j2].Coordinate.Y * TileScreenWidth + (TileScreenWidth / 2));
            //        var end = new Vector2(Path.Nodes[i].Coordinate.X * TileScreenWidth + (TileScreenWidth / 2), Path.Nodes[i].Coordinate.Y * TileScreenWidth + (TileScreenWidth / 2));

            //        var direction = start - end;
            //        var angle = Math.Atan2(direction.Y, direction.X);
            //        angle += Math.PI / 2;
            //        points.Add(new Vector2(end.X + ((float)Math.Cos(angle) * 10), end.Y + ((float)Math.Sin(angle) * 10)));
            //        angle += Math.PI;
            //        points.Add(new Vector2(end.X + ((float)Math.Cos(angle) * 10), end.Y + ((float)Math.Sin(angle) * 10)));

            //        direction *= -1;
            //        angle = Math.Atan2(direction.Y, direction.X);
            //        angle += Math.PI / 2;
            //        points.Add(new Vector2(start.X + ((float)Math.Cos(angle) * 10), start.Y + ((float)Math.Sin(angle) * 10)));
            //        angle += Math.PI;
            //        points.Add(new Vector2(start.X + ((float)Math.Cos(angle) * 10), start.Y + ((float)Math.Sin(angle) * 10)));

            //        var poly = new Polygon(this, points, Color.White, true, Vector2.Zero);
            //        //Sprites.Add(poly);
            //        for (int k = 0; k < points.Count; k++)
            //        {
            //            if (k == 0)
            //            {
            //                Debug.AddLine(points[0], points[points.Count - 1], 100);
            //            }
            //            else
            //            {
            //                Debug.AddLine(points[k], points[k - 1], 100);
            //            }
            //        }

            //        bool intersects = false;
            //        foreach (var outside in Sprites.Where(x => x.SpriteType == SpriteTypes.Outside))
            //        {
            //            Vector2 r = Vector2.Zero;
            //            float d = 0;
            //            if (World.Intersects(poly.Body, outside.Body, ref r, ref d, true))
            //            {
            //                intersects = true;
            //            }
            //        }
            //        if (intersects)
            //        {
            //            //int k = i - j2 - 1;
            //            //Path.Nodes.RemoveRange(j2 + 1, k);
            //            j2++;
            //            //i++;
            //            //for (var k = j + 1; k < i - 1; k++)
            //            //{
            //            //    Path.Nodes.RemoveAt(k);
            //            //}
            //            //Path.Nodes.RemoveAt(i - 1);
            //        }
            //        else
            //        {
            //            j2++;
            //            //i--;
            //        }
            //    }
            //    //if (Path.Nodes[j].Parent != null)
            //    //{
            //    //    Debug.AddPoint(Path.Nodes[j].Coordinate * TileScreenWidth + new Vector2(TileScreenWidth / 2), Color.Red);
            //    //    Debug.AddPoint(Path.Nodes[j].Parent.Coordinate * TileScreenWidth + new Vector2(TileScreenWidth / 2), Color.Red);
            //    //    Debug.AddLine(Path.Nodes[j].Coordinate * TileScreenWidth + new Vector2(TileScreenWidth / 2), Path.Nodes[j].Parent.Coordinate * TileScreenWidth + new Vector2(TileScreenWidth / 2), 100);
            //    //}
            //    j2++;
            //}
        }
    }
}
