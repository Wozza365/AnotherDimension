using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Topdown.AI
{
    public static class AStar
    {
        public static int TileSize { get; set; } = 1280/32;
        public static bool AllowDiagonal { get; set; } = false;
        public static float Cost { get; set; } = 1;
        public static float CostDiagonal { get; set; } = 1.414f;
        public static List<Node> MapNodes { get; set; } = new List<Node>();
        
        public static Path GenerateAStarPath(Sprite control, Sprite target)
        {
            var startNode = GetNode(control.Body.Centre);
            var targetNode = GetNode(target.Body.Centre);
            var openList = new List<Node>();
            var closedList = new List<Node>();

            startNode.G = 0;
            startNode.F = startNode.G + (int)Vector2.Distance(startNode.Coordinate, targetNode.Coordinate);
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                var current = openList.OrderBy(x => x.F).First();
                if (current.Coordinate == targetNode.Coordinate)
                {
                    return GetPath(current);
                }
                closedList.Add(current);
                openList.Remove(current);

                foreach (var neighbour in GetNeighbours(current))
                {
                    if (closedList.Count(x => x.Coordinate == neighbour.Coordinate) == 0)
                    {
                        neighbour.F = neighbour.G + (int) Vector2.Distance(neighbour.Coordinate, targetNode.Coordinate);
                        if (openList.Count(x => x.Coordinate == neighbour.Coordinate) == 0)
                        {
                            openList.Add(neighbour);
                        }
                        else
                        {
                            var openNeighbour = openList.First(x => x.Coordinate == neighbour.Coordinate);
                            if (neighbour.G < openNeighbour.G)
                            {
                                openNeighbour.G = neighbour.G;
                                openNeighbour.Parent = neighbour.Parent;
                            }
                        }
                    }
                }
            }
            Path path = new Path();
            path.Valid = false;
            path.Nodes.Reverse();
            return path;
        }

        public static Sprite GetClosestSprite(Sprite control, List<Sprite> targets)
        {
            return targets.OrderBy(x => Vector2.Distance(control.Body.Centre, x.Body.Centre)).First();
        }

        private static Node GetNode(Vector2 centre)
        {
            int x = (int)centre.X / TileSize;
            int y = (int)centre.Y / TileSize;
            return new Node()
            {
                Coordinate = new Vector2(x, y)
            };
        }

        private static List<Node> GetNeighbours(Node node)
        {
            var neighbours = new List<Node>();
            if (MapNodes.Count(x => x.Coordinate == new Vector2(node.Coordinate.X + 1, node.Coordinate.Y)) > 0)
            {
                neighbours.Add(MapNodes.First(x => x.Coordinate == new Vector2(node.Coordinate.X + 1, node.Coordinate.Y)));
            }
            if (MapNodes.Count(x => x.Coordinate == new Vector2(node.Coordinate.X - 1, node.Coordinate.Y)) > 0)
            {
                neighbours.Add(MapNodes.First(x => x.Coordinate == new Vector2(node.Coordinate.X - 1, node.Coordinate.Y)));
            }
            if (MapNodes.Count(x => x.Coordinate == new Vector2(node.Coordinate.X, node.Coordinate.Y + 1)) > 0)
            {
                neighbours.Add(MapNodes.First(x => x.Coordinate == new Vector2(node.Coordinate.X, node.Coordinate.Y + 1)));
            }
            if (MapNodes.Count(x => x.Coordinate == new Vector2(node.Coordinate.X, node.Coordinate.Y - 1)) > 0)
            {
                neighbours.Add(MapNodes.First(x => x.Coordinate == new Vector2(node.Coordinate.X, node.Coordinate.Y - 1)));
            }
            neighbours.ForEach(n => n.G += n.Cost);
            neighbours.ForEach(n => n.Parent = new Node()
            {
                Area = node.Area,
                Coordinate = node.Coordinate,
                Cost = node.Cost,
                F = node.F,
                G = node.G,
                H = node.H,
                Parent = node.Parent
            });
            return neighbours;
        }

        private static Path GetPath(Node node)
        {
            Path path = new Path();
            while (node.Parent != null)
            {
                node = node.Parent;
                path.Nodes.Add(node);
            }
            path.Valid = true;
            return path;
        }
    }
}
