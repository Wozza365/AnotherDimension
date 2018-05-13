using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Physics
{
    /// <summary>
    /// The primary object used for Physics calculations
    /// Line, Circle, AABB Rectangle and Polygon can be implemented through this class
    /// Generally avoid using properties outside of the shapes scope. This will have unknown affects
    /// </summary>
    public class Body
    {
        //Line Specific Properties
        #region line properties
        public Vector2 Origin
        {
            get => Position;
            set => Position = value;
        }
        public Vector2 Direction;
        public Vector2 End => new Vector2(Origin.X + Direction.X, Origin.Y + Direction.Y);
        #endregion

        //Circle Specific Properties
        #region circle properties
        private float _radius { get; set; }
        public float Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                Width = value * 2;
                Height = value * 2;
            }
        }
        public Vector2 Centre
        {
            get
            {
                if (Shape == Shape.Rectangle)
                {
                    return new Vector2(Position.X + HalfWidth, Position.Y + HalfHeight);
                } 
                if (Shape == Shape.Polygon)
                {
                    var x = Vertices.Select(i => i.X).Average();
                    var y = Vertices.Select(i => i.Y).Average();
                    return new Vector2(x, y);
                }
                if (Shape == Shape.Circle)
                {
                    return new Vector2(Position.X + HalfWidth, Position.Y + HalfHeight);
                }
                return Position;
            }
        }

        #endregion

        //Rectangle Specific Properties
        #region rectangle properties
        public float Width { get; set; }
        public float Height { get; set; }
        public Vector2 BottomRight => new Vector2(Position.X + Width, Position.Y + Height);
        public float Right
        {
            get
            {
                if (Shape == Shape.Polygon)
                {
                    return Vertices.OrderByDescending(x => x.X).First().X;
                }
                return Position.X + Width;
            }
        }

        public float Bottom
        {
            get
            {
                if (Shape == Shape.Polygon)
                {
                    return Vertices.OrderByDescending(x => x.Y).First().Y;
                }
                return Position.Y + Height;
            }
        }

        public float Left
        {
            get
            {
                if (Shape == Shape.Polygon)
                {
                    return Vertices.OrderBy(x => x.X).First().X;
                }
                return Position.X;
            }
        }

        public float Top
        {
            get
            {
                if (Shape == Shape.Polygon)
                {
                    return Vertices.OrderBy(x => x.Y).First().Y;
                }
                return Position.Y;
            }
        }
        #endregion

        //Polygon Specific Properties
        public List<Vector2> Vertices = new List<Vector2>();

        //Physics Related Properties
        #region physics properties
        public Vector2 Position;
        public Vector2 Gravity;
        public Vector2 Velocity;
        public Vector2 Bounce;
        public Vector2 Acceleration;
        public Vector2 MaxVelocity;
        public bool Static { get; set; }
        public float Friction { get; set; }
        public float Mass { get; set; }
        #endregion

        //Rotation Properties (Experimental)
        #region rotation properties
        public float AngularVelocity { get; set; }
        public float AngularAcceleration { get; set; }
        public float Rotation { get; set; } = 0;
        #endregion

        // General Properties
        #region general properties
        public MainGame Game { get; set; }
        public Sprite Sprite { get; set; }
        public Guid Guid { get; set; }
        public bool Enabled { get; set; }
        public Shape Shape { get; set; }
        public bool Collisions { get; set; } = true;
        #endregion

        /// <summary>
        /// Acts as half width for rectangle and radius for circle
        /// </summary>
        public float HalfWidth => Shape == Shape.Rectangle || Shape == Shape.Polygon ? Width / 2 : Radius;

        /// <summary>
        /// Half height for rectangles
        /// </summary>
        public float HalfHeight => Shape == Shape.Rectangle || Shape == Shape.Polygon ? Height / 2 : Radius;

        /// <summary>
        /// Only necessary values are set here 
        /// Use object setting to set the values you require for the chosen shape
        /// </summary>
        /// <param name="s"></param>
        public Body(Sprite s)
        {
            Sprite = s;
            Game = s.Game;
            Gravity = World.Gravity;
        }

        /// <summary>
        /// Can be called foreach shape but is only for updating polygons
        /// </summary>
        /// <param name="deltaX">change in X</param>
        /// <param name="deltaY">change in Y</param>
        public void UpdateVertices(float deltaX, float deltaY)
        {
            if (Shape == Shape.Polygon)
            {
                for (var i = 0; i < Vertices.Count; i++)
                {
                    var x = Vertices[i].X + deltaX;
                    var y = Vertices[i].Y + deltaY;
                    //Vertices[i] = new Vector2(x, y);
                    float s = (float)Math.Sin(AngularVelocity);
                    float c = (float)Math.Cos(AngularVelocity);

                    // translate point back to origin:
                    x -= Centre.X;
                    y -= Centre.Y;

                    // rotate point
                    float xnew = x * c - y * s;
                    float ynew = x * s + y * c;

                    // translate point back:
                    x = xnew + Centre.X;
                    y = ynew + Centre.Y;
                    Vertices[i] = new Vector2(x, y);
                }
            }
        }
    }
}
