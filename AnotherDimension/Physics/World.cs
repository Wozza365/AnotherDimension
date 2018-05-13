using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Other;
using Game.Sprites.Shapes;

namespace Game.Physics
{
    /// <summary>
    /// The physics controller of the world
    /// </summary>
    public static class World
    {
        public static MainGame game;
        /// <summary>
        /// The primary gravity value for the world
        /// Each sprite can still have its own gravity affect if set to do so
        /// </summary>
        public static Vector2 Gravity { get; set; }

        /// <summary>
        /// Used for friction on floor in top down or air friction in a Game.
        /// </summary>
        public static float SpaceFriction { get; set; }
        private static Dictionary<Tuple<Guid, Guid>, int> _prevCollisions = new Dictionary<Tuple<Guid, Guid>, int>();

        /// <summary>
        /// Apply the gravitational force once per frame
        /// </summary>
        /// <param name="sprites">The list of sprites to apply to, generally just the full list</param>
        /// <param name="frameTime">Time since last frame, to calculate the correct change required</param>
        public static void ApplyGravity(List<Sprite> sprites, float frameTime)
        {
            foreach (var sprite in sprites)
            {
                var body = sprite.Body;
                if (body.Enabled && !body.Static)
                {
                    body.Velocity.X += body.Gravity.X / frameTime;
                    body.Velocity.Y += body.Gravity.Y / frameTime;
                }
            }
        }

        /// <summary>
        /// Apply velocity of each sprite to its position
        /// </summary>
        /// <param name="sprites">List of sprites to apply to</param>
        /// <param name="frameTime">Time since last frame, to calculate the correct change required</param>
        public static void ApplyVelocity(List<Sprite> sprites, float frameTime)
        {
            foreach (var sprite in sprites)
            {
                var body = sprite.Body;
                if (body.Enabled && !body.Static)
                {
                    //Clamp the sprites maximum velocity in each direction
                    body.Velocity.X = MathHelper.Clamp(body.Velocity.X, -body.MaxVelocity.X, body.MaxVelocity.X);
                    body.Velocity.Y = MathHelper.Clamp(body.Velocity.Y, -body.MaxVelocity.Y, body.MaxVelocity.Y);

                    //Apply the standard space friction, which is either air resistance in a platformer or just to slow down an object going across the floor
                    //Can be reversed by with its own space friction value by setting it to 1/SpaceFriction for 0 friction
                    body.Velocity.X *= SpaceFriction * body.Sprite.SpaceFriction;
                    body.Velocity.Y *= SpaceFriction * body.Sprite.SpaceFriction;

                    //Update position
                    body.Position.X += body.Velocity.X;
                    body.Position.Y += body.Velocity.Y;
                    
                    //Update polygon vertices
                    body.UpdateVertices(body.Velocity.X, body.Velocity.Y);

                    //Rotate (only works with polygons)
                    body.Rotation += body.AngularVelocity;
                    body.AngularVelocity += body.AngularAcceleration;

                    //Clamp position within the screen bounds
                    body.Position.X = MathHelper.Clamp(body.Position.X, MainGame.Screen.X, MainGame.Screen.Width - body.Width);
                    body.Position.Y = MathHelper.Clamp(body.Position.Y, MainGame.Screen.Y, MainGame.Screen.Height - body.Height);

                    //Stops a bouncing affect after we've stopped at the edge of the screen.
                    if (body.Position.X == MainGame.Screen.X + body.HalfWidth || body.Position.X == MainGame.Screen.Width - body.HalfWidth)
                    {
                        body.Velocity.X = 0;
                    }
                    if (body.Position.Y == MainGame.Screen.Y + body.HalfHeight || body.Position.Y == MainGame.Screen.Height - body.HalfHeight)
                    {
                        body.Velocity.Y = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Best  way to test two bodies, if you aren't confident of the shape
        /// </summary>
        /// <param name="body1">First body to test</param>
        /// <param name="body2">Second body</param>
        /// <param name="result">reference to the point of intersection</param>
        /// <param name="distance">depth of the intersection</param>
        /// <param name="fullTest">Used for polygons, conducts a second more thorough test for polygon intersection</param>
        /// <returns></returns>
        public static bool Intersects(Body body1, Body body2, ref Vector2 result, ref float distance, bool fullTest = false)
        {
            if (body1.Equals(body2) || !body1.Collisions || !body2.Collisions)
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
                        return CirclePolygonIntersects(body1, body2, ref result, ref distance);
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
            //A bounding box test around our polygon
            //Very quick and reduces most calculations that are guaranteed to be false
            if ((body1.Right < body2.Left ||
                body1.Left > body2.Right ||
                body1.Top > body2.Bottom ||
                body1.Bottom < body2.Top ||
                body2.Static && body1.Static) && !fullTest)
            {
                return false;
            }
            //Go through each point of each polygon, creating a line between two in the second polygon
            //This is an adaptation of the algorithm used for circle line testing 
            foreach (var indice in body1.Vertices)
            {
                for (var i = 0; i < body2.Vertices.Count; i++)
                {
                    Vector2 AP, AB, A, B;

                    //create the line AB
                    if (i == body2.Vertices.Count - 1)
                    {
                        A = body2.Vertices.Last();
                        B = body2.Vertices.First();
                        AP = body1.Centre - A;
                        AB = B - A;
                    }
                    else
                    {
                        A = body2.Vertices[i];
                        B = body2.Vertices[i + 1];
                        AP = body1.Centre - A;
                        AB = B - A;
                    }

                    float magnitubeAB = AB.LengthSquared();
                    float ABAPproduct = Vector2.Dot(AP, AB);
                    distance = ABAPproduct / magnitubeAB;

                    //check if point is beyond the AB line
                    if (distance < 0)
                    {
                        result = A;
                    }
                    else if (distance > 1)
                    {
                        result = B;
                    }
                    else
                    {
                        //find the closest point along that line
                        result = A + AB * distance;
                    }
                    //check if the distance is within range, can be inaccurate for some tests so IsInPolygon can provide a second more complex test if required, usually if a polygon is concave
                    if (Vector2.Distance(body1.Centre, result) < Vector2.Distance(body1.Centre, indice) && (fullTest || IsInPolygon(body2, indice)))
                    {
                        //SeparatePolygonPolygon(body1, body2, result, Vector2.Distance(body1.Centre, indice));
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Test if a point is inside a polygon
        /// Works with concave polygons
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="indice"></param>
        /// <returns></returns>
        private static bool IsInPolygon(Body body1, Vector2 indice)
        {
            bool isInside = false;
            Vector2 oldPoint = new Vector2(body1.Vertices[body1.Vertices.Count - 1].X, body1.Vertices[body1.Vertices.Count - 1].Y);

            foreach (var polygonIndice in body1.Vertices)
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

        /// <summary>
        /// Test if a circle is colliding with the polygon
        /// Also works with concave polygons
        /// </summary>
        /// <param name="body1">Circle</param>
        /// <param name="body2">Polygon</param>
        /// <param name="result"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        private static bool CirclePolygonIntersects(Body body1, Body body2, ref Vector2 result, ref float distance)
        {
            //A bounding box test around our polygon and circle
            //Very quick and reduces most calculations that are guaranteed to be false
            if ( body1.Centre.X + body1.Radius < body2.Left ||
                 body1.Centre.X - body1.Radius > body2.Right ||
                 body1.Centre.Y - body1.Radius > body2.Bottom ||
                 body1.Centre.Y + body1.Radius < body2.Top ||
                 body2.Static && body1.Static)
            {
                return false;
            }

            for (var i = 0; i < body2.Vertices.Count; i++)
            {
                Vector2 AP, AB, A, B;

                //loop over each line of the polygon to test against the circle
                if (i == body2.Vertices.Count - 1)
                {
                    A = body2.Vertices.Last();
                    B = body2.Vertices.First();
                    AP = body1.Centre - A;
                    AB = B - A;
                }
                else
                {
                    A = body2.Vertices[i];
                    B = body2.Vertices[i + 1];
                    AP = body1.Centre - A;
                    AB = B - A;
                }
                float magnitudeAB = AB.LengthSquared();
                float ABAPproduct = Vector2.Dot(AP, AB);
                distance = ABAPproduct / magnitudeAB;

                //check if beyond or before the line entirely to get the closest point on the line
                if (distance < 0)
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
                //Test if our point is further than the radius
                if (Vector2.Distance(body1.Centre, result) < body1.Radius)
                {
                    //Currently resolves as structure is not currently capable of returning each line where it intersects, which may be multiple
                    var line = new Line(A, B, Color.White, game, body2.Friction);
                    SeparateCircleLine(body1, line.Body, ref result);
                }
            }
            return false;
        }

        /// <summary>
        /// Test if a circle intersects with a line
        /// </summary>
        /// <param name="body1">Circle</param>
        /// <param name="body2">Line</param>
        /// <param name="result"></param>
        /// <returns>intersects result</returns>
        private static bool CircleLineIntersects(Body body1, Body body2, ref Vector2 result)
        {
            Vector2 AP = body1.Centre - body2.Origin; 
            Vector2 AB = body2.End - body2.Origin;

            float magnitudeAB = AB.LengthSquared();
            float ABAPproduct = Vector2.Dot(AP, AB);
            float distance = ABAPproduct / magnitudeAB;

            if (distance < 0)
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
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check collision between circle and AABB rectangle
        /// </summary>
        /// <param name="body1">Circle or rectangle</param>
        /// <param name="body2">Circle or rectangle</param>
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

            //check if closest point is less than radius, leaving squared is more efficient as sqrt is expensive
            return distanceSquared < circle.Radius.Sqrd();
        }

        /// <summary>
        /// Fast AABB test between rectangles
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="body2"></param>
        /// <returns></returns>
        private static bool RectRectIntersects(Body body1, Body body2)
        {
            return !(body1.Right < body2.Left) && !(body1.Left > body2.Right) && !(body1.Top > body2.Bottom) && !(body1.Bottom < body2.Top);
        }

        /// <summary>
        /// Check if distance between the two centres is less than the two radii
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="body2"></param>
        /// <returns></returns>
        private static bool CircleCircleIntersects(Body body1, Body body2)
        {
            return Vector2.Distance(body1.Centre, body2.Centre) < body1.Radius + body2.Radius;
        }

        /// <summary>
        /// Base separate method for any 2 bodies
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="body2"></param>
        /// <param name="result"></param>
        /// <param name="distance"></param>
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

        /// <summary>
        /// Separate two circles
        /// Makes use of the object mass
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="body2"></param>
        private static void SeparateCircleCircle(Body body1, Body body2)
        {
            //distance between the two centres
            double dist = Math.Sqrt((body1.Centre.X - body2.Centre.X).Sqrd() + (body1.Centre.Y - body2.Centre.Y).Sqrd());

            //penetration depth
            double penDepth = (body1.Radius + body2.Radius) - dist;

            //dont bother if the intersection is really small
            if (Math.Abs(penDepth) < 0.2f) return;

            //directions
            Vector2 direction = new Vector2(body1.Centre.X - body2.Centre.X, body1.Centre.Y - body2.Centre.Y),
                    reverseDirection = new Vector2(body2.Centre.X - body1.Centre.X, body2.Centre.Y - body1.Centre.Y);
            direction.Normalize();
            reverseDirection.Normalize();

            //mass ratios
            var m1 = body1.Mass / (body1.Mass + body2.Mass);
            var m2 = body2.Mass / (body1.Mass + body2.Mass);

            //change the positions
            body1.Position.X += direction.X * m1 * (float)penDepth;
            body1.Position.Y += direction.Y * m1 * (float)penDepth;
            body2.Position.X += reverseDirection.X * m2 * (float)penDepth;
            body2.Position.Y += reverseDirection.Y * m2 * (float)penDepth;

            var body1Minbody2 = body1.Mass - body2.Mass;
            var body2Minbody1 = body2.Mass - body1.Mass;

            //change the velocities based on mass and reduces based on the bounce factor as well
            body1.Velocity.X = (body1.Velocity.X * body1Minbody2 + (2 * body2.Mass * body2.Velocity.X)) * body1.Bounce.X / (body1.Mass + body2.Mass);
            body1.Velocity.Y = (body1.Velocity.Y * body1Minbody2 + (2 * body2.Mass * body2.Velocity.Y)) * body1.Bounce.Y / (body1.Mass + body2.Mass);
            body2.Velocity.X = -(body2.Velocity.X * body2Minbody1 + (2 * body1.Mass * body1.Velocity.X)) * body2.Bounce.X / (body2.Mass + body1.Mass);
            body2.Velocity.Y = -(body2.Velocity.Y * body2Minbody1 + (2 * body1.Mass * body1.Velocity.Y)) * body2.Bounce.Y / (body2.Mass + body1.Mass);
        }

        /// <summary>
        /// Separates circle and rectangles
        /// </summary>
        /// <param name="body1">Circle</param>
        /// <param name="body2">Rectangle</param>
        private static void SeparateCircleRect(Body body1, Body body2)
        {
            //closest point of the rectangle to the circle
            Vector2 v = new Vector2(MathHelper.Clamp(body1.Centre.X, body2.Position.X, body2.Right),
                          MathHelper.Clamp(body1.Centre.Y, body2.Position.Y, body2.Bottom));

            Vector2 direction = new Vector2(body1.Centre.X - v.X, body1.Centre.Y - v.Y);

            Debug.AddLine(body1.Centre, v, 0);

            //Resolve velocity and position for each surface of the rectangle
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
            else
            {
                //Closest point is a corner so need to do some more complex work
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

                //var rad = Math.Atan2(rPos.Y - body1.Centre.Y, rPos.X - body1.Centre.Y);
                //if (rad < 0)
                //{
                //    rad = Math.Abs(rad);
                //}
                //else
                //{
                //    rad = 2 * Math.PI - rad;
                //}
                //var deg = rad * (180 / Math.PI);

                //Debug.AddLine(body1.Centre, rPos, 2);
                //Debug.AddLog("Rectangle + Circle Collision: " + deg);
                //Debug.AddPoint(v, Color.Blue);
                //Debug.AddPoint(body2.Centre, Color.Violet);
                //Debug.AddPoint(body1.Centre, Color.Yellow);
            }

        }

        /// <summary>
        /// Separate two rectangles
        ///Two sides of a rectangle will always collid, so we need to check which distance is the shortest and then only resolve the shortest
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="body2"></param>
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

        /// <summary>
        /// Separate a circle and a line by reversing flipping the velocity direction across the normal of the line which it intersects
        /// </summary>
        /// <param name="body1">Circle</param>
        /// <param name="body2">Line</param>
        /// <param name="result"></param>
        private static void SeparateCircleLine(Body body1, Body body2, ref Vector2 result)
        {
            float angle = (float)Math.Atan2(body1.Centre.Y - result.Y, body1.Centre.X - result.X) + (float)Math.PI;
            var x = body1.Centre.X + body1.Radius * Math.Cos(angle);
            var y = body1.Centre.Y + body1.Radius * Math.Sin(angle);
            Debug.AddPoint(new Vector2((float)x, (float)y), Color.Purple);
            Debug.AddPoint(result, Color.Aqua);
            
            //velocity angle
            float angle2 = (float)Math.Atan2(-body1.Velocity.Y, body1.Velocity.X);
            angle2 += (float)Math.PI;
            //intersection angle
            float angle3 = (float)Math.Atan2(-(result.Y - body1.Centre.Y), result.X - body1.Centre.X);
            angle3 += (float)Math.PI;
            
            float angle4 = angle3 - angle2;
            float angle5 = angle3 + angle4;
            Vector2 angleVector3 = new Vector2(
                (float)Math.Cos(angle5),
                -(float)Math.Sin(angle5));

            //reverse the position
            body1.Position.Y -= (float)y - result.Y;
            body1.Position.X -= (float)x - result.X;

            angleVector3.Normalize();

            //set the new velocity from the normal
            var speed = body1.Velocity.Length();
            body1.Velocity.Y = angleVector3.Y * speed;
            body1.Velocity.X = angleVector3.X * speed;


            //use bounce or friction values if we're on or off ground
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

        /// <summary>
        /// Separate two convex or concave polygons
        /// Gets the angle of velocity and then the angle of intersection and then resolve it across the intersection normal
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="body2"></param>
        /// <param name="result"></param>
        /// <param name="distance"></param>
        private static void SeparatePolygonPolygon(Body body1, Body body2, ref Vector2 result, ref float distance)
        {
            Body _active = body1.Static ? body2 : body1;
            Body _static = body1.Static ? body1 : body2;
            
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
            _active.AngularVelocity += (_active.Centre.X * _active.Velocity.Y - _active.Centre.Y * _active.Velocity.X) / _active.Centre.LengthSquared();

            _active.Velocity.Y = angleVector3.Y * speed * _active.Bounce.Y;
            _active.Velocity.X = angleVector3.X * speed * _active.Bounce.X;
        }

        /// <summary>
        /// Resolve velocities for the simple rectangle by just reflecting velocities in a ratio of the mass and reduced (or increased) by the bounce factor
        /// </summary>
        /// <param name="body1"></param>
        /// <param name="body2"></param>
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
                // at some point its better to just stop micro bouncing.
                body2.Velocity.Y = body2.Velocity.Y > 0.5 || body2.Velocity.Y < 0 ? body2.Velocity.Y * body2.Bounce.Y * -1 : 0;
            }
            else if (body2.Static)
            {
                // at some point its better to just stop micro bouncing.
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
