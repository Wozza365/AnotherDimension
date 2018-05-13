using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Game.AI
{
    /// <summary>
    /// A star class
    /// Could be modified in future to include multiple algorithms
    /// Some objects might not always need the most direct path that A Star is more likely to deliver so the reduced processing cost would be more beneficial to those AI.
    /// </summary>
    public static class AStar
    {
        public static int TileSize { get; set; } = 1280/32;
        public static bool AllowDiagonal { get; set; } = false;
        public static float Cost { get; set; } = 1;
        public static float CostDiagonal { get; set; } = 1.414f;
        public static List<Node> MapNodes { get; set; } = new List<Node>();
        
        /// <summary>
        /// Generates an A Star path using map nodes
        /// </summary>
        /// <param name="control">Sprite to start targeting from</param>
        /// <param name="target">Sprite it wishes to arrive at</param>
        /// <returns></returns>
        public static Path GenerateAStarPath(Sprite control, Sprite target)
        {
            var startNode = GetNode(control.Body.Centre);
            var targetNode = GetNode(target.Body.Centre);
            var openList = new List<Node>();
            var closedList = new List<Node>();

            startNode.G = 0;
            startNode.F = startNode.G + (int)Vector2.Distance(startNode.Coordinate, targetNode.Coordinate);
            openList.Add(startNode);

            //expand while there is no solution
            while (openList.Count > 0)
            {
                //shortest distance so far
                var current = openList.OrderBy(x => x.F).First();

                if (current.Coordinate == targetNode.Coordinate)
                {
                    return GetPath(current);
                }

                closedList.Add(current);
                openList.Remove(current);

                // go through each of our neighbours
                foreach (var neighbour in GetNeighbours(current).OrderBy(x => Vector2.Distance(targetNode.Coordinate, x.Coordinate)))
                {
                    //if we haven't already traversed via a shorter route, continue
                    if (closedList.Count(x => x.Coordinate == neighbour.Coordinate) == 0)
                    {
                        //calculate the distance traveled to this node and add to the list
                        neighbour.F = neighbour.G + (int) Vector2.Distance(neighbour.Coordinate, targetNode.Coordinate);
                        if (openList.Count(x => x.Coordinate == neighbour.Coordinate) == 0)
                        {
                            openList.Add(neighbour);
                        }
                        else
                        {
                            //if we have a shorter distance to this node, then replace the previous 
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
            return path;
        }
        
        /// <summary>
        /// Gets a node/tile of any specified position on the map
        /// </summary>
        /// <param name="centre">centre of object to test</param>
        /// <returns></returns>
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
            //8-directional neighbours
            // \ | /
            // - . -
            // / | \
            if (MapNodes.Count(x => x.Coordinate == new Vector2(node.Coordinate.X + 1, node.Coordinate.Y)) > 0)
            {
                neighbours.Add((Node)MapNodes.First(x => x.Coordinate == new Vector2(node.Coordinate.X + 1, node.Coordinate.Y)).Clone());
            }
            if (MapNodes.Count(x => x.Coordinate == new Vector2(node.Coordinate.X - 1, node.Coordinate.Y)) > 0)
            {
                neighbours.Add((Node)MapNodes.First(x => x.Coordinate == new Vector2(node.Coordinate.X - 1, node.Coordinate.Y)).Clone());
            }
            if (MapNodes.Count(x => x.Coordinate == new Vector2(node.Coordinate.X, node.Coordinate.Y + 1)) > 0)
            {
                neighbours.Add((Node) MapNodes.First(x => x.Coordinate == new Vector2(node.Coordinate.X, node.Coordinate.Y + 1)).Clone());
            }
            if (MapNodes.Count(x => x.Coordinate == new Vector2(node.Coordinate.X, node.Coordinate.Y - 1)) > 0)
            {
                neighbours.Add((Node)MapNodes.First(x => x.Coordinate == new Vector2(node.Coordinate.X, node.Coordinate.Y - 1)).Clone());
            }

            if (MapNodes.Count(x => x.Coordinate == new Vector2(node.Coordinate.X - 1, node.Coordinate.Y - 1)) > 0)
            {
                neighbours.Add((Node)MapNodes.First(x => x.Coordinate == new Vector2(node.Coordinate.X - 1, node.Coordinate.Y - 1)).Clone());
                neighbours.Last().Cost += 4;
            }
            if (MapNodes.Count(x => x.Coordinate == new Vector2(node.Coordinate.X + 1, node.Coordinate.Y - 1)) > 0)
            {
                neighbours.Add((Node)MapNodes.First(x => x.Coordinate == new Vector2(node.Coordinate.X + 1, node.Coordinate.Y - 1)).Clone());
                neighbours.Last().Cost += 4;
            }
            if (MapNodes.Count(x => x.Coordinate == new Vector2(node.Coordinate.X + 1, node.Coordinate.Y + 1)) > 0)
            {
                neighbours.Add((Node)MapNodes.First(x => x.Coordinate == new Vector2(node.Coordinate.X + 1, node.Coordinate.Y + 1)).Clone());
                neighbours.Last().Cost += 4;
            }
            if (MapNodes.Count(x => x.Coordinate == new Vector2(node.Coordinate.X - 1, node.Coordinate.Y + 1)) > 0)
            {
                neighbours.Add((Node)MapNodes.First(x => x.Coordinate == new Vector2(node.Coordinate.X - 1, node.Coordinate.Y + 1)).Clone());
                neighbours.Last().Cost += 4;
            }
            //add the cost to each and add them to the return list
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

        /// <summary>
        /// Gets the path by going through each pointer to the parent node backwards
        /// </summary>
        /// <param name="node">Start node</param>
        /// <returns></returns>
        private static Path GetPath(Node node)
        {
            Path path = new Path();
            while (node.Parent != null)
            {
                node = node.Parent;
                path.Nodes.Add(node);
            }
            path.Valid = true;
            path.Nodes.Reverse();
            return path;
        }
    }
}
