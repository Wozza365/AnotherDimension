﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Topdown.Sprites.Shapes;

namespace Topdown.Other
{
    public static class Debug
    {
        private static List<Tuple<string, DateTime>> _textList = new List<Tuple<string, DateTime>>();
        private static List<Dot> _dotsList = new List<Dot>();
        private static List<DebugLine> _lineList = new List<DebugLine>();

        public static bool Active { get; set; } = true;

        public static void AddLog(string s)
        {
            _textList.Add(new Tuple<string, DateTime>(s, DateTime.Now));
        }

        public static void Update(SpriteBatch sb, SpriteFont sf, Texture2D whitePixel)
        {
            if (Active)
            {
                for (int i = 0; i < _textList.Count; i++)
                {
                    if (_textList.ElementAt(i).Item2 < DateTime.Now.AddSeconds(-0.5))
                    {
                        _textList.RemoveAt(i);
                    }
                    else
                    {
                        sb.DrawString(sf, _textList.ElementAt(i).Item1, new Vector2(10, 10 + i * 15), Color.White);
                    }
                }
                for (int i = 0; i < _dotsList.Count; i++)
                {
                    if (_dotsList.ElementAt(i).StartTime < DateTime.Now.AddSeconds(-2))
                    {
                        _dotsList.RemoveAt(i);
                    }
                    else
                    {
                        //sb.Draw(whitePixel, dots[i].Position, dots[i].Colour);
                        sb.Draw(whitePixel, new Rectangle((int) _dotsList[i].Position.X - 1, (int) _dotsList[i].Position.Y - 1, 3, 3), _dotsList[i].Colour);
                    }
                }

                for (int i = 0; i < _lineList.Count; i++)
                {
                    if (_lineList.ElementAt(i).StartTime < DateTime.Now.AddSeconds(-_lineList[i].Delay - 0.01))
                    {
                        _lineList.RemoveAt(i);
                    }
                    else
                    {
                        Vector2 edge = _lineList[i].Point - _lineList[i].Point2;
                        float angle = (float) Math.Atan2(edge.Y, edge.X) + (float) Math.PI;

                        sb.Draw(whitePixel, new Rectangle((int) _lineList[i].Point.X, (int) _lineList[i].Point.Y, (int) edge.Length(), 1), null, Color.White, angle, new Vector2(0, 0), SpriteEffects.None, 0);

                        //sb.Draw(whitePixel, new Rectangle((int)lines[i].Point2.X, (int)lines[i].Point2.Y, (int)edge.Length(), 1), null, Color.Blue, angle + (float)(Math.PI / 2), new Vector2(0, 0), SpriteEffects.None, 0);

                        //sb.Draw(whitePixel, new Rectangle((int)lines[i].Point2.X, (int)lines[i].Point2.Y, (int)edge.Length(), 1), null, Color.Blue, angle - (float)(Math.PI / 2), new Vector2(0, 0), SpriteEffects.None, 0);

                    }
                }
            }
        }

        public static void AddPoint(Vector2 v, Color c)
        {
            _dotsList.Add(new Dot
            {
                Position = v,
                Colour = c,
                StartTime = DateTime.Now
            });
        }

        public static void AddLine(Vector2 point, Vector2 point2, int delay)
        {
            _lineList.Add(new DebugLine()
            {
                Point = point,
                Point2 = point2,
                Delay = delay,
                StartTime = DateTime.Now
            });
        }
    }

    public class Dot
    {
        public Vector2 Position { get; set; }
        public Color Colour { get; set; }
        public DateTime StartTime { get; set; }
    }
    public class DebugLine : Line
    {
        public DateTime StartTime { get; set; }
        public Vector2 Scale { get; set; }
        public float Angle { get; set; }
        public Vector2 Point { get; set; }
        public Vector2 Point2 { get; set; }
        public int Delay { get; set; }
    }
}
