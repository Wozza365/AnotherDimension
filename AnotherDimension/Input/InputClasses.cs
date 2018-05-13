using Microsoft.Xna.Framework;

namespace Game.Input
{
    //Simple helper classes for testing input

    public class ControllerState
    {
        public bool DLeft { get; set; }
        public bool DRight { get; set; }
        public bool DDown { get; set; }
        public bool DUp { get; set; }
        public bool Back { get; set; }
        public bool Start { get; set; }
        public bool MiddleButton { get; set; }
        public bool A { get; set; }
        public bool B { get; set; }
        public bool X { get; set; }
        public bool Y { get; set; }
        public bool LB { get; set; }
        public bool RB { get; set; }
        public float LT { get; set; }
        public float RT { get; set; }
        public bool LeftStickClick { get; set; }
        public bool RightStickClick { get; set; }
        public Vector2 LeftStick { get; set; }
        public Vector2 RightStick { get; set; }

    }
    public enum ControllerButtons
    {
        DLeft, DRight, DDown, DUp,
        Back, Start, MiddleButton,
        A, B, X, Y,
        LB, RB, LT, RT,
        LeftStick, RightStick,
        LeftStickClick, RightStickClick
    }
    public enum MouseControl
    {
        LeftClick = 0,
        RightClick = 1,
        MiddleClick = 2,
        ScrollWheel = 3
    }
}
