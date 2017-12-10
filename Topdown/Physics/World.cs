using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Topdown.Other;
using Topdown.Sprites.Shapes;
using Topdown.Other.Extension;
using Topdown;

namespace Topdown.Physics
{
    public static class World
    {
        public static TopdownGame game;
        public static Vector2 Gravity { get; set; }
        private static Dictionary<Tuple<Guid, Guid>, int> _prevCollisions = new Dictionary<Tuple<Guid, Guid>, int>();
        private static List<Tuple<Guid, Guid>> _collisions = new List<Tuple<Guid, Guid>>();

        public static void ClearCollisions()
        {
            _collisions.Clear();
        }

        public static void ApplyGravity(List<Sprite> sprites, float frameTime)
        {
            foreach (var sprite in sprites)
            {
                var body = sprite.Body;
                if (body.Enabled && !body.Static)
                {
                    body.Velocity.X += body.Gravity.X / frameTime;
                    body.Velocity.Y += body.Gravity.Y / frameTime;
                    body.AngularVelocity += body.AngularAcceleration;
                }
            }
        }
        public static void ApplyVelocity(List<Sprite> sprites, float frameTime)
        {
            foreach (var sprite in sprites)
            {
                var body = sprite.Body;
                body.Velocity.X = MathHelper.Clamp(body.Velocity.X, -body.MaxVelocity.X, body.MaxVelocity.X);
                body.Velocity.Y = MathHelper.Clamp(body.Velocity.Y, -body.MaxVelocity.Y, body.MaxVelocity.Y);
                if (body.Enabled && !body.Static)
                {
                    body.Position.X += body.Velocity.X;
                    body.Position.Y += body.Velocity.Y;
                    body.UpdateIndices(body.Velocity.X, body.Velocity.Y);
                    body.Rotation += body.AngularVelocity;
                    body.Position.X = MathHelper.Clamp(body.Position.X, TopdownGame.Screen.X + body.HalfWidth, TopdownGame.Screen.Width - body.HalfWidth);
                    body.Position.Y = MathHelper.Clamp(body.Position.Y, TopdownGame.Screen.Y + body.HalfHeight, TopdownGame.Screen.Height - body.HalfHeight);
                }
            }
        }

        public static bool Intersects(Body body1, Body body2, ref Vector2 result, ref float distance, bool fullTest = false)
        {
            if (body1.Equals(body2))
            {
                return false; //don't want to collide with ourselves
            }

            if (body1.Shape == Shape.Circle)
            {
                switch (body2.Shape)
                {
                    case Shape.Rectangle:
                        return CircleRectIntersects(body1, body2);
                    case Shape.Circle:
                        return CircleCircleIntersects(body1, body2);
                    case Shape.Line:
                        return CircleLineIntersects(body1, body2, ref result);
                    case Shape.Polygon:
                        return CirclePolygonIntersects(body1, body2, ref result, ref distance); //not implemented yet
                }
            }
            else if (body1.Shape == Shape.Rectangle)
            {
                switch (body2.Shape)
                {
                    case Shape.Rectangle:
                        return RectRectIntersects(body1, body2);
                    case Shape.Circle:
                        return CircleRectIntersects(body2, body1);
                    case Shape.Polygon:
                        return CirclePolygonIntersects(body2, body1, ref result, ref distance);
                }
            }
            else if (body1.Shape == Shape.Polygon)
            {
                switch (body2.Shape)
                {
                    case Shape.Polygon:
                        return PolygonPolygonIntersects(body1, body2, ref result, ref distance, fullTest);
                    case Shape.Circle:
                        return CirclePolygonIntersects(body2, body1, ref result, ref distance);
                }
            }

            return false;
        }

        private static bool PolygonPolygonIntersects(Body body1, Body body2, ref Vector2 result, ref float distance, bool fullTest = false)
        {
            if ((body1.Right < body2.Left ||
                body1.Left > body2.Right ||
                body1.Top > body2.Bottom ||
                body1.Bottom < body2.Top ||
                body2.Static && body1.Static) && !fullTest)
            {
                return false;
            }
            foreach (var indice in body1.Indices)
            {
                for (var i = 0; i < body2.Indices.Count; i++)
                {
                    Vector2 AP, AB, A, B;

                    if (i == body2.Indices.Count - 1)
                    {
                        A = body2.Indices.Last();
                        B = body2.Indices.First();
                        AP = body1.Centre - A; //Vector from A to P   
                        AB = B - A; //Vector from A to B  
                    }
                    else
                    {
                        A = body2.Indices[i];
                        B = body2.Indices[i + 1];
                        AP = body1.Centre - A; //Vector from A to P   
                        AB = B - A; //Vector from A to B  
                    }
                    float magnitudeAB = AB.LengthSquared(); //Magnitude of AB vector (it's length squared)     
                    float ABAPproduct = Vector2.Dot(AP, AB); //The DOT product of a_to_p and a_to_b     
                    distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

                    if (distance < 0) //Check if P projection is over vectorAB     
                    {
                        result = A;
                    }
                    else if (distance > 1)
                    {
                        result = B;
                    }
                    else
                    {
                        result = A + AB * distance;
                    }
                    if (Vector2.Distance(body1.Centre, result) < Vector2.Distance(body1.Centre, indice) && (fullTest || IsInPolygon(body2, indice)))
                    {
                        //SeparatePolygonPolygon(body1, body2, result, Vector2.Distance(body1.Centre, indice));
                        return true;
                    }
                    else
                    {
                        _prevCollisions[new Tuple<Guid, Guid>(body1.Guid, body2.Guid)] = 0;
                    }
                }
            }
            return false;
        }

        private static bool IsInPolygon(Body body1, Vector2 indice)
        {
            bool isInside = false;
            Vector2 oldPoint = new Vector2(body1.Indices[body1.Indices.Count - 1].X, body1.Indices[body1.Indices.Count - 1].Y);

            foreach (var polygonIndice in body1.Indices)
            {
                Vector2 newPoint = new Vector2(polygonIndice.X, polygonIndice.Y);

                Vector2 p1, p2;
                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if (newPoint.X < indice.X == indice.X <= oldPoint.X && (indice.Y - p1.Y) * (p2.X - p1.X) < (p2.Y - p1.Y) * (indice.X - p1.X))
                {
                    isInside = !isInside;
                }
                oldPoint = newPoint;
            }
            return isInside;
        }

        private static bool CirclePolygonIntersects(Body body1, Body body2, ref Vector2 result, ref float distance)
        {
            if (_collisions.Contains(new Tuple<Guid, Guid>(body1.Guid, body2.Guid)) ||
                _collisions.Contains(new Tuple<Guid, Guid>(body2.Guid, body1.Guid)))
                return false;
            for (var i = 0; i < body2.Indices.Count; i++)
            {
                Vector2 AP, AB, A, B;

                if (i == body2.Indices.Count - 1)
                {
                    A = body2.Indices.Last();
                    B = body2.Indices.First();
                    AP = body1.Centre - A; //Vector from A to P   
                    AB = B - A; //Vector from A to B  
                }
                else
                {
                    A = body2.Indices[i];
                    B = body2.Indices[i + 1];
                    AP = body1.Centre - A; //Vector from A to P   
                    AB = B - A; //Vector from A to B  
                }
                float magnitudeAB = AB.LengthSquared(); //Magnitude of AB vector (it's length squared)     
                float ABAPproduct = Vector2.Dot(AP, AB); //The DOT product of a_to_p and a_to_b     
                distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

                if (distance < 0) //Check if P projection is over vectorAB     
                {
                    result = A;
                }
                else if (distance > 1)
                {
                    result = B;
                }
                else
                {
                    result = A + AB * distance;
                }
                //Debug.AddLine(body1.Centre, result, 0);

                if (Vector2.Distance(body1.Centre, result) < body1.Radius)
                {
                    var line = new Line(A, B, Color.White, game, body2.Friction);
                    SeparateCircleLine(body1, line.Body, ref result);
                    _collisions.Add(new Tuple<Guid, Guid>(body1.Guid, body2.Guid));
                    //return true;
                }

            }
            return false;
        }

        private static bool CircleLineIntersects(Body body1, Body body2, ref Vector2 result)
        {
            //Vector2 result;
            Vector2 AP = body1.Centre - body2.Origin;       //Vector from A to P   
            Vector2 AB = body2.End - body2.Origin;       //Vector from A to B  

            float magnitudeAB = AB.LengthSquared();     //Magnitude of AB vector (it's length squared)     
            float ABAPproduct = Vector2.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
            float distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

            if (distance < 0)     //Check if P projection is over vectorAB     
            {
                result = body2.Origin;
            }
            else if (distance > 1)
            {
                result = body2.End;
            }
            else
            {
                result = body2.Origin + AB * distance;
            }
            Debug.AddLine(body1.Centre, result, 0);

            if (Vector2.Distance(body1.Centre, result) < body1.Radius)
            {
                //SeparateCircleLine(body1, body2, ref result);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check collision between circle and rectangle
        /// </summary>
        /// <param name="body1">Circle</param>
        /// <param name="body2">Rectangle</param>
        /// <returns>True if intersects</returns>
        private static bool CircleRectIntersects(Body body1, Body body2)
        {
            var circle = body1.Shape == Shape.Circle ? body1 : body2;
            var rect = body1.Shape == Shape.Circle ? body2 : body1;

            //find the closest point of the rectangle to the circle
            Vector2 closest = new Vector2(MathHelper.Clamp(circle.Centre.X, rect.Left, rect.Right),
                                          MathHelper.Clamp(circle.Centre.Y, rect.Top, rect.Bottom));

            Vector2 direction = Vector2.Subtract(circle.Centre, closest);
            float distanceSquared = direction.LengthSquared();

            return distanceSquared < circle.Radius * circle.Radius;
        }

        private static bool RectRectIntersects(Body body1, Body body2)
        {
            return !(body1.Right < body2.Left) && !(body1.Left > body2.Right) && !(body1.Top > body2.Bottom) && !(body1.Bottom < body2.Top);
        }

        private static bool CircleCircleIntersects(Body body1, Body body2)
        {
            return Vector2.Distance(body1.Centre, body2.Centre) > body1.Radius + body2.Radius;
        }

        public static void Separate(Body body1, Body body2, ref Vector2 result, ref float distance)
        {
            if (body1.Enabled && body2.Enabled)
            {
                if (body1.Shape == Shape.Circle)
                {
                    if (body2.Shape == Shape.Circle)
                    {
                        SeparateCircleCircle(body1, body2);
                    }
                    else if (body2.Shape == Shape.Rectangle)
                    {
                        SeparateCircleRect(body1, body2);
                    }
                    else if (body2.Shape == Shape.Line)
                    {
                        SeparateCircleLine(body1, body2, ref result);
                    }
                    else if (body2.Shape == Shape.Polygon)
                    {
                        SeparateCircleLine(body1, body2, ref result);
                    }
                }
                else if (body1.Shape == Shape.Rectangle)
                {
                    if (body2.Shape == Shape.Circle)
                    {
                        SeparateCircleRect(body2, body1);
                    }
                    else if (body2.Shape == Shape.Rectangle)
                    {
                        SeparateRectRect(body1, body2);
                    }
                }
                else if (body1.Shape == Shape.Polygon)
                {
                    if (body2.Shape == Shape.Circle)
                    {
                        SeparateCircleLine(body2, body1, ref result);
                    }
                    else if (body2.Shape == Shape.Polygon)
                    {
                        SeparatePolygonPolygon(body1, body2, ref result, ref distance);
                    }
                }
            }
        }

        private static void SeparateCircleCircle(Body body1, Body body2)
        {
            double dist = Math.Sqrt((body1.Centre.X - body2.Centre.X) * (body1.Centre.X - body2.Centre.X) +
                        (body1.Centre.Y - body2.Centre.Y) * (body1.Centre.Y - body2.Centre.Y));

            double penDepth = (body1.Radius + body2.Radius) - dist;
            Vector2 direction = new Vector2(body1.Centre.X - body2.Centre.X, body1.Centre.Y - body2.Centre.Y),
                    reverseDirection = new Vector2(body2.Centre.X - body1.Centre.X, body2.Centre.Y - body1.Centre.Y);
            direction.Normalize();
            reverseDirection.Normalize();
            var m1 = body1.Mass / (body1.Mass + body2.Mass);
            var m2 = body2.Mass / (body1.Mass + body2.Mass);
            body1.Position.X += direction.X * m1 * (float)penDepth;
            body1.Position.Y += direction.Y * m1 * (float)penDepth;
            body2.Position.X += reverseDirection.X * m2 * (float)penDepth;
            body2.Position.Y += reverseDirection.Y * m2 * (float)penDepth;

            var body1Minbody2 = body1.Mass - body2.Mass;
            var body2Minbody1 = body2.Mass - body1.Mass;
            body1.Velocity.X = (body1.Velocity.X * body1Minbody2 + (2 * body2.Mass * body2.Velocity.X)) / (body1.Mass + body2.Mass);
            body1.Velocity.Y = (body1.Velocity.Y * body1Minbody2 + (2 * body2.Mass * body2.Velocity.Y)) / (body1.Mass + body2.Mass);
            body2.Velocity.X = -(body2.Velocity.X * body2Minbody1 + (2 * body1.Mass * body1.Velocity.X)) / (body2.Mass + body1.Mass);
            body2.Velocity.Y = -(body2.Velocity.Y * body2Minbody1 + (2 * body1.Mass * body1.Velocity.Y)) / (body2.Mass + body1.Mass);
        }

        private static void SeparateCircleRect(Body body1, Body body2)
        {
            Vector2 v = new Vector2(MathHelper.Clamp(body1.Centre.X, body2.Position.X, body2.Right),
                          MathHelper.Clamp(body1.Centre.Y, body2.Position.Y, body2.Bottom));

            Vector2 direction = new Vector2(body1.Centre.X - v.X, body1.Centre.Y - v.Y);

            float distanceSquared = direction.LengthSquared();
            float distance = Vector2.Distance(body2.Centre, body1.Centre);
            Debug.AddLine(body1.Centre, v, 0);

            if (body1.Centre.Y < body2.Position.Y && body1.Centre.X > body2.Position.X && body1.Centre.X < body2.Position.X + body2.Width)
            {
                ResolveVelocityYCircleRect(ref body1, ref body2);
                body1.Position.Y = body2.Position.Y - body1.Radius - 1;
            }
            else if (body1.Centre.Y > body2.Position.Y + body2.Height && body1.Centre.X > body2.Position.X && body1.Centre.X < body2.Position.X + body2.Width)
            {
                ResolveVelocityYCircleRect(ref body1, ref body2);
                body1.Position.Y = body2.Position.Y + body2.Height + body1.Radius + 1;
            }
            else if (body1.Centre.X < body2.Position.X && body1.Centre.Y > body2.Position.Y && body1.Centre.Y < body2.Position.Y + body2.Height)
            {
                ResolveVelocityXCircleRect(ref body1, ref body2);
                body1.Position.X = body2.Position.X - body1.Radius - 1;
            }
            else if (body1.Centre.X > body2.Position.X && body1.Centre.Y > body2.Position.Y && body1.Centre.Y < body2.Position.Y + body2.Height)
            {
                ResolveVelocityXCircleRect(ref body1, ref body2);
                body1.Position.X = body2.Position.X + body2.Width + body1.Radius + 1;
            }
            //if (r.Mass == float.PositiveInfinity)
            else
            {
                Vector2 rPos = new Vector2(0);

                if (Vector2.Distance(body1.Centre, new Vector2(body2.Position.X, body2.Position.Y)) < body1.Radius)
                {
                    rPos = new Vector2(body2.Position.X, body2.Position.Y);
                }
                else if (Vector2.Distance(body1.Centre, new Vector2(body2.Right, body2.Position.Y)) < body1.Radius)
                {
                    rPos = new Vector2(body2.Right, body2.Position.Y);
                }
                else if (Vector2.Distance(body1.Centre, new Vector2(body2.Position.X, body2.Bottom)) < body1.Radius)
                {
                    rPos = new Vector2(body2.Position.X, body2.Bottom);
                }
                else if (Vector2.Distance(body1.Centre, new Vector2(body2.Right, body2.Bottom)) < body1.Radius)
                {
                    rPos = new Vector2(body2.Right, body2.Bottom);
                }

                body1.Velocity.X *= -1;
                body1.Velocity.Y *= -1;
                body1.Position.X += (direction.Length() - body1.Radius);
                body1.Position.Y += (direction.Length() - body1.Radius);

                var rad = Math.Atan2(rPos.Y - body1.Centre.Y, rPos.X - body1.Centre.Y);
                if (rad < 0)
                {
                    rad = Math.Abs(rad);
                }
                else
                {
                    rad = 2 * Math.PI - rad;
                }
                var deg = rad * (180 / Math.PI);

                Debug.AddLine(body1.Centre, rPos, 2);
                Debug.AddLog("Rectangle + Circle Collision: " + deg);
                Debug.AddPoint(v, Color.Blue);
                Debug.AddPoint(body2.Centre, Color.Violet);
                Debug.AddPoint(body1.Centre, Color.Yellow);
            }

        }

        private static void SeparateRectRect(Body body1, Body body2)
        {
            //tr
            if (body1.Right >= body2.Position.X && body1.Right <= body2.Right && body1.Position.Y <= body2.Bottom && body1.Position.Y >= body2.Position.Y && !body1.Equals(body2))
            {
                if (body1.Right - body2.Position.X < body2.Bottom - body1.Position.Y)
                {
                    ResolveVelocityXRectRect(ref body1, ref body2);
                    ResolveVelocityYRectRect(ref body1, ref body2);
                    if (float.IsPositiveInfinity(body1.Mass))
                    {
                        body2.Position.X = body1.Position.X - body2.Width - 1;
                    }
                    else if (float.IsPositiveInfinity(body2.Mass))
                    {
                        body1.Position.X = body2.Position.X - body1.Width - 1;
                    }
                    else
                    {
                        float a = body1.Mass;
                        float b = body2.Mass;
                        float c = a + b;
                        a = a / c;
                        b = b / c;
                        float depth = body1.Position.X - body2.Right;
                        body1.Position.X -= (depth * a) - 1;
                        body2.Position.X += (depth * b) + 1;
                    }
                }
                else
                {
                    ResolveVelocityXRectRect(ref body1, ref body2);
                    ResolveVelocityYRectRect(ref body1, ref body2);
                    if (float.IsPositiveInfinity(body1.Mass))
                    {
                        body2.Position.Y = body1.Position.Y - body2.Height - 1;
                    }
                    else if (float.IsPositiveInfinity(body2.Mass))
                    {
                        body1.Position.Y = body2.Position.Y - body1.Height - 1;
                    }
                    else
                    {
                        float a = body1.Mass;
                        float b = body2.Mass;
                        float c = a + b;
                        a = a / c;
                        b = b / c;
                        float depth = body2.Position.Y - body1.Bottom;
                        body1.Position.Y += (depth * a) + 1;
                        body2.Position.Y -= (depth * b) - 1;
                    }

                    //body1.Y = body2.Y + body2.Height + 1;
                }
            }
            //bl
            else if (body1.Position.X <= body2.Right && body1.Position.X >= body2.Position.X && body1.Bottom >= body2.Position.Y && body1.Bottom <= body2.Bottom && !body1.Equals(body2))
            {
                if (body2.Right - body1.Position.X < body1.Bottom - body2.Position.Y)
                {
                    ResolveVelocityXRectRect(ref body1, ref body2);
                    ResolveVelocityYRectRect(ref body1, ref body2);
                    if (float.IsPositiveInfinity(body1.Mass))
                    {
                        body2.Position.X = body1.Position.X - body2.Width - 1;
                    }
                    else if (float.IsPositiveInfinity(body2.Mass))
                    {
                        body1.Position.X = body2.Right + 1;
                    }
                    else
                    {
                        float a = body1.Mass;
                        float b = body2.Mass;
                        float c = a + b;
                        a = a / c;
                        b = b / c;
                        float depth = body2.Right - body1.Position.X;
                        body1.Position.X += (depth * a) + 1;
                        body2.Position.X -= (depth * b) - 1;
                    }

                    //body1.X = body2.X + body1.Width + 1;
                }
                else
                {
                    ResolveVelocityXRectRect(ref body1, ref body2);
                    ResolveVelocityYRectRect(ref body1, ref body2);
                    if (float.IsPositiveInfinity(body1.Mass))
                    {
                        body2.Position.Y = body1.Position.Y - body2.Height - 1;
                    }
                    else if (float.IsPositiveInfinity(body2.Mass))
                    {
                        body1.Position.Y = body2.Position.Y - body1.Height - 1;
                    }
                    else
                    {
                        float a = body1.Mass;
                        float b = body2.Mass;
                        float c = a + b;
                        a = a / c;
                        b = b / c;
                        float depth = body2.Position.Y - body1.Bottom;
                        body1.Position.Y -= (depth * a) - 1;
                        body2.Position.Y += (depth * b) + 1;
                    }
                    //body1.Y = body2.Y - body1.Height - 1;
                    //onGround = true;
                }
            }
            //tl
            else if (body1.Position.X <= body2.Right && body1.Position.X >= body2.Position.X && body1.Position.Y <= body2.Bottom && body1.Position.Y >= body2.Position.Y && !body1.Equals(body2))
            {
                if (body2.Right - body1.Position.X < body2.Bottom - body1.Position.Y)
                {
                    ResolveVelocityXRectRect(ref body1, ref body2);
                    ResolveVelocityYRectRect(ref body1, ref body2);
                    if (float.IsPositiveInfinity(body1.Mass))
                    {
                        body2.Position.X = body1.Position.X + body2.Width + 1;
                    }
                    else if (float.IsPositiveInfinity(body2.Mass))
                    {
                        body1.Position.X = body2.Position.X + body1.Width + 1;
                    }
                    else
                    {
                        float a = body1.Mass;
                        float b = body2.Mass;
                        float c = a + b;
                        a = a / c;
                        b = b / c;
                        float depth = body2.Right - body1.Position.X;
                        body1.Position.X += (depth * a) + 1;
                        body2.Position.X -= (depth * b) - 1;
                    }
                    //body1.X = body2.X + body1.Width + 1;
                }
                else
                {
                    ResolveVelocityXRectRect(ref body1, ref body2);
                    ResolveVelocityYRectRect(ref body1, ref body2);
                    if (float.IsPositiveInfinity(body1.Mass))
                    {
                        body2.Position.Y = body1.Position.Y - body2.Height - 1;
                    }
                    else if (float.IsPositiveInfinity(body2.Mass))
                    {
                        body1.Position.Y = body2.Position.Y - body1.Height - 1;
                    }
                    else
                    {
                        float a = body1.Mass;
                        float b = body2.Mass;
                        float c = a + b;
                        a = a / c;
                        b = b / c;
                        float depth = body2.Position.Y - body1.Bottom;
                        body1.Position.Y += (depth * a) + 1;
                        body2.Position.Y -= (depth * b) - 1;
                    }
                }
            }
            //br
            else if (body1.Right >= body2.Position.X && body1.Right <= body2.Right && body1.Bottom >= body2.Position.Y && body1.Bottom <= body2.Bottom)
            {
                if (body1.Right - body2.Position.X < body1.Bottom - body2.Position.Y)
                {
                    ResolveVelocityXRectRect(ref body1, ref body2);
                    ResolveVelocityYRectRect(ref body1, ref body2);
                    if (float.IsPositiveInfinity(body1.Mass))
                    {
                        body2.Position.X = body1.Position.X - body2.Width - 1;
                    }
                    else if (float.IsPositiveInfinity(body2.Mass))
                    {
                        body1.Position.X = body2.Position.X - body1.Width - 1;
                    }
                    else
                    {
                        float a = body1.Mass;
                        float b = body2.Mass;
                        float c = a + b;
                        a = a / c;
                        b = b / c;
                        float depth = body1.Right - body2.Position.X;
                        body1.Position.X -= (depth * a) - 1;
                        body2.Position.X += (depth * b) + 1;
                    }
                }
                else
                {
                    ResolveVelocityXRectRect(ref body1, ref body2);
                    ResolveVelocityYRectRect(ref body1, ref body2);
                    if (float.IsPositiveInfinity(body1.Mass))
                    {
                        body2.Position.Y = body1.Position.Y - body2.Height - 1;
                    }
                    else if (float.IsPositiveInfinity(body2.Mass))
                    {
                        body1.Position.Y = body2.Position.Y - body1.Height - 1;
                    }
                    else
                    {
                        float a = body1.Mass;
                        float b = body2.Mass;
                        float c = a + b;
                        a = a / c;
                        b = b / c;
                        float depth = body2.Position.Y - body1.Bottom;
                        body1.Position.Y -= (depth * a) - 1;
                        body2.Position.Y += (depth * b) + 1;
                    }
                    //body1.Y = body2.Y - body1.Height - 1;
                    //onGround = true;
                }
            }
        }

        private static void SeparateCircleLine(Body body1, Body body2, ref Vector2 result)
        {
            float angle = (float)Math.Atan2(body1.Centre.Y - result.Y, body1.Centre.X - result.X) + (float)Math.PI;
            var x = body1.Centre.X + body1.Radius * Math.Cos(angle);
            var y = body1.Centre.Y + body1.Radius * Math.Sin(angle);
            Debug.AddPoint(new Vector2((float)x, (float)y), Color.Purple);
            Debug.AddPoint(result, Color.Aqua);

            //float velocity = (float)Math.Atan2(-body1.Velocity.Y, body1.Velocity.X);
            //float difference = (float)Math.Atan2(-(result.Y - body1.Centre.Y), result.X - body1.Centre.X);

            //float angle5 = difference + difference - velocity;
            //Vector2 angleVector3 = new Vector2((float)Math.Cos(angle5), -(float)Math.Sin(angle5));

            float angle2 = (float)Math.Atan2(-body1.Velocity.Y, body1.Velocity.X);
            angle2 += (float)Math.PI;
            float angle3 = (float)Math.Atan2(-(result.Y - body1.Centre.Y), result.X - body1.Centre.X);
            angle3 += (float)Math.PI;


            float angle4 = angle3 - angle2;
            float angle5 = angle3 + angle4;
            Vector2 angleVector3 = new Vector2(
                (float)Math.Cos(angle5),
                -(float)Math.Sin(angle5));

            body1.Position.Y -= (float)y - result.Y;
            body1.Position.X -= (float)x - result.X;

            angleVector3.Normalize();

            var speed = body1.Velocity.Length();
            body1.Velocity.Y = angleVector3.Y * speed;
            body1.Velocity.X = angleVector3.X * speed;


            if (!body1.Sprite.OnGround)
            {
                body1.Velocity.X *= body1.Bounce.X;
                body1.Velocity.Y *= body1.Bounce.Y;
            }
            else
            {
                body1.Velocity.X *= body2.Friction;
                body1.Velocity.Y *= body2.Friction;
            }
        }

        private static void SeparatePolygonPolygon(Body body1, Body body2, ref Vector2 result, ref float distance)
        {
            Body _active = body1.Static ? body2 : body1;
            Body _static = body1.Static ? body1 : body2;

            int _collisionsCount = 0;
            _prevCollisions.TryGetValue(new Tuple<Guid, Guid>(_active.Guid, _static.Guid), out _collisionsCount);
            if (_collisionsCount == 0)
                _prevCollisions.TryGetValue(new Tuple<Guid, Guid>(_static.Guid, _active.Guid), out _collisionsCount);

            float angle = (float)Math.Atan2(_active.Centre.Y - result.Y, _active.Centre.X - result.X) + (float)Math.PI;
            var x = _active.Centre.X + distance * Math.Cos(angle);
            var y = _active.Centre.Y + distance * Math.Sin(angle);

            float angle2 = (float)Math.Atan2(-_active.Velocity.Y, _active.Velocity.X);
            float angle3 = (float)Math.Atan2(-(result.Y - _active.Centre.Y), result.X - _active.Centre.X);

            float angle4 = angle3 - angle2;
            float angle5 = angle3 + angle4;
            Vector2 angleVector3 = new Vector2((float)Math.Cos(angle5), -(float)Math.Sin(angle5));

            _active.Position.Y -= (float)y - result.Y;
            _active.Position.X -= (float)x - result.X;

            angleVector3.Normalize();

            var speed = _active.Velocity.Length();
            _active.AngularVelocity += (_active.Centre.X * _active.Velocity.Y - _active.Centre.Y * _active.Velocity.X) / (_active.Centre.X.Sqrd() + _active.Centre.Y.Sqrd());

            _active.Velocity.Y = angleVector3.Y * speed * _active.Bounce.Y * (_collisionsCount + 1);// * (negY ? -1 : 1);
            _active.Velocity.X = angleVector3.X * speed * _active.Bounce.X * (_collisionsCount + 1);//* (negX ? -1 : 1);
            _prevCollisions[new Tuple<Guid, Guid>(_active.Guid, _static.Guid)] = _collisionsCount + 1;

        }

        private static void ResolveVelocityXCircleRect(ref Body body1, ref Body body2)
        {
            if (body1.Static)
            {
                body2.Velocity.X = body2.Velocity.X * body2.Bounce.X * -1;
            }
            else if (body2.Static)
            {
                body1.Velocity.X = body1.Velocity.X * body1.Bounce.X * -1;
            }
            else
            {
                var mv1 = body1.Mass * body1.Velocity.X;
                var mv2 = body2.Mass * body2.Velocity.X;

                var v = mv1 - mv2 / body1.Mass + body2.Mass;
                body1.Velocity.X = body2.Velocity.X = v;
            }
        }
        private static void ResolveVelocityYCircleRect(ref Body body1, ref Body body2)
        {
            if (body1.Static)
            {
                body2.Velocity.Y = body2.Velocity.Y > 0.5 || body2.Velocity.Y < 0 ? body2.Velocity.Y * body2.Bounce.Y * -1 : 0;
            }
            else if (body2.Static)
            {
                // at some point its better to just stop bouncing.
                body1.Velocity.Y = body1.Velocity.Y > 0.5 || body1.Velocity.Y < 0 ? body1.Velocity.Y * body1.Bounce.Y * -1 : 0;
            }
            else
            {
                var mv1 = body1.Mass * body1.Velocity.Y;
                var mv2 = body2.Mass * body2.Velocity.Y;

                var v = (mv2 - mv1) / (body1.Mass + body2.Mass);
                body1.Velocity.Y = body2.Velocity.Y = v;
                Debug.AddLog(body1.Velocity.Y.ToString());
            }
        }
        private static void ResolveVelocityXRectRect(ref Body body1, ref Body body2)
        {
            if (body1.Static)
            {
                body2.Velocity.X = body2.Velocity.X * body2.Bounce.X * -1;
            }
            else if (body2.Static)
            {
                body1.Velocity.X = body1.Velocity.X * body1.Bounce.X * -1;
            }
            else
            {
                var mv1 = body1.Mass * body1.Velocity.X;
                var mv2 = body2.Mass * body2.Velocity.X;

                var v = mv1 - mv2 / body1.Mass + body2.Mass;
                body1.Velocity.X = body2.Velocity.X = v;
            }
        }
        private static void ResolveVelocityYRectRect(ref Body body1, ref Body body2)
        {
            if (body1.Static)
            {
                body2.Velocity.Y = body2.Velocity.Y > 0.5 || body2.Velocity.Y < 0 ? body2.Velocity.Y * body2.Bounce.Y * -1 : 0;
            }
            else if (body2.Static)
            {
                body1.Velocity.Y = body1.Velocity.Y > 0.5 || body1.Velocity.Y < 0 ? body1.Velocity.Y * body1.Bounce.Y * -1 : 0;
            }
            else
            {
                var mv1 = body1.Mass * body1.Velocity.Y;
                var mv2 = body2.Mass * body2.Velocity.Y;

                var v = mv1 - mv2 / body1.Mass + body2.Mass;
                body1.Velocity.Y = body2.Velocity.Y = v;
                Debug.AddLog(body1.Velocity.Y.ToString());
            }
        }
    }
}
