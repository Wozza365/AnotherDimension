using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Topdown.Input
{
    public static class InputManager
    {
        public static ControllerState[] CurrentControllerState = new ControllerState[4];
        public static ControllerState[] PreviousControllerState = new ControllerState[4];

        public static KeyboardState CurrentKeyboardState;
        public static KeyboardState PreviousKeyboardState;
        public static MouseState CurrentMouseState;
        public static MouseState PreviousMouseState;

        public static object Game { get; set; }

        public static void CheckInputs()
        {
            PreviousKeyboardState = CurrentKeyboardState;
            PreviousMouseState = CurrentMouseState;
            CurrentKeyboardState = Keyboard.GetState();
            CurrentMouseState = Mouse.GetState();

            PreviousControllerState[0] = CurrentControllerState[0];
            PreviousControllerState[1] = CurrentControllerState[1];
            PreviousControllerState[2] = CurrentControllerState[2];
            PreviousControllerState[3] = CurrentControllerState[3];
            CurrentControllerState[0] = GetControllerState(1);
            CurrentControllerState[1] = GetControllerState(2);
            CurrentControllerState[2] = GetControllerState(3);
            CurrentControllerState[3] = GetControllerState(4);
        }


        public static bool Pressed(Keys key, MouseControl button, ControllerButtons cb, PlayerIndex pi = PlayerIndex.One)
        {
            return Pressed(key) || Pressed(button) || Pressed(cb, pi);
        }
        public static bool Held(Keys key, MouseControl button, ControllerButtons cb, PlayerIndex pi = PlayerIndex.One)
        {
            return Held(key) || Held(button) || Held(cb, pi);
        }
        public static bool Released(Keys key, MouseControl button, ControllerButtons cb, PlayerIndex pi = PlayerIndex.One)
        {
            return Released(key) || Released(button) || Released(cb, pi);
        }

        public static bool Pressed(Keys key, ControllerButtons cb, PlayerIndex pi = PlayerIndex.One)
        {
            return Pressed(key) || Pressed(cb, pi);
        }
        public static bool Held(Keys key, ControllerButtons cb, PlayerIndex pi = PlayerIndex.One)
        {
            return Held(key) || Held(cb, pi);
        }
        public static bool Released(Keys key, ControllerButtons cb, PlayerIndex pi = PlayerIndex.One)
        {
            return Released(key) || Released(cb, pi);
        }


        public static bool Pressed(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key) && !PreviousKeyboardState.IsKeyDown(key);
        }
        public static bool Pressed(MouseControl button)
        {
            switch (button)
            {
                case MouseControl.LeftClick:
                    return CurrentMouseState.LeftButton == ButtonState.Pressed &&
                           PreviousMouseState.LeftButton != ButtonState.Pressed;
                case MouseControl.RightClick:
                    return CurrentMouseState.RightButton == ButtonState.Pressed &&
                           PreviousMouseState.RightButton != ButtonState.Pressed;
                case MouseControl.MiddleClick:
                    return CurrentMouseState.MiddleButton == ButtonState.Pressed &&
                           PreviousMouseState.MiddleButton != ButtonState.Pressed;
            }
            return false;
        }
        public static bool Pressed(ControllerButtons cb, PlayerIndex pi)
        {
            bool current = (bool)typeof(ControllerState).GetProperty(cb.ToString()).GetValue(CurrentControllerState[(int)pi]);
            bool previous = (bool)typeof(ControllerState).GetProperty(cb.ToString()).GetValue(PreviousControllerState[(int)pi]);

            return current && !previous;
        }

        public static bool Held(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key) && PreviousKeyboardState.IsKeyDown(key);
        }
        public static bool Held(MouseControl button)
        {
            switch (button)
            {
                case MouseControl.LeftClick:
                    return CurrentMouseState.LeftButton == ButtonState.Pressed &&
                           PreviousMouseState.LeftButton == ButtonState.Pressed;
                case MouseControl.RightClick:
                    return CurrentMouseState.RightButton == ButtonState.Pressed &&
                           PreviousMouseState.RightButton == ButtonState.Pressed;
                case MouseControl.MiddleClick:
                    return CurrentMouseState.MiddleButton == ButtonState.Pressed &&
                           PreviousMouseState.MiddleButton == ButtonState.Pressed;
            }
            return false;
        }
        public static bool Held(ControllerButtons cb, PlayerIndex pi)
        {
            bool current = (bool)typeof(ControllerState).GetProperty(cb.ToString()).GetValue(CurrentControllerState[(int)pi], null);
            bool previous = (bool)typeof(ControllerState).GetProperty(cb.ToString()).GetValue(PreviousControllerState[(int)pi], null);

            return current && previous;
        }

        public static bool Released(Keys key)
        {
            return !CurrentKeyboardState.IsKeyDown(key);
        }
        public static bool Released(MouseControl button)
        {
            switch (button)
            {
                case MouseControl.LeftClick:
                    return CurrentMouseState.LeftButton == ButtonState.Released;
                case MouseControl.RightClick:
                    return CurrentMouseState.RightButton == ButtonState.Released;
                case MouseControl.MiddleClick:
                    return CurrentMouseState.MiddleButton == ButtonState.Released;
            }
            return false;
        }
        public static bool Released(ControllerButtons cb, PlayerIndex pi)
        {
            bool current = (bool)typeof(ControllerState).GetProperty(cb.ToString()).GetValue(CurrentControllerState[(int)pi], null);
            return current;
        }

        public static Vector2 MousePosition()
        {
            return new Vector2(CurrentMouseState.X, CurrentMouseState.Y);
        }
        public static Vector2 ThumbstickPosition(ControllerButtons cb, int index)
        {
            if (index > 3 || index < 0)
                throw new Exception();
            switch (cb)
            {
                case ControllerButtons.LeftStick:
                    return CurrentControllerState[index].LeftStick;
                case ControllerButtons.RightStick:
                    return CurrentControllerState[index].RightStick;
                default:
                    throw new Exception();
            }
        }
        public static bool ControllerActive(int index)
        {
            return GamePad.GetState((PlayerIndex)index).IsConnected;
        }

        public static ControllerState GetControllerState(int p)
        {
            ControllerState c = new ControllerState
            {
                A = GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed,
                B = GamePad.GetState((PlayerIndex)p).Buttons.B == ButtonState.Pressed,
                X = GamePad.GetState((PlayerIndex)p).Buttons.X == ButtonState.Pressed,
                Y = GamePad.GetState((PlayerIndex)p).Buttons.Y == ButtonState.Pressed,

                LB = GamePad.GetState((PlayerIndex)p).Buttons.LeftShoulder == ButtonState.Pressed,
                RB = GamePad.GetState((PlayerIndex)p).Buttons.RightShoulder == ButtonState.Pressed,

                Back = GamePad.GetState((PlayerIndex)p).Buttons.Back == ButtonState.Pressed,
                Start = GamePad.GetState((PlayerIndex)p).Buttons.Start == ButtonState.Pressed,
                MiddleButton = GamePad.GetState((PlayerIndex)p).Buttons.BigButton == ButtonState.Pressed,

                DLeft = GamePad.GetState((PlayerIndex)p).DPad.Left == ButtonState.Pressed,
                DRight = GamePad.GetState((PlayerIndex)p).DPad.Right == ButtonState.Pressed,
                DUp = GamePad.GetState((PlayerIndex)p).DPad.Up == ButtonState.Pressed,
                DDown = GamePad.GetState((PlayerIndex)p).DPad.Down == ButtonState.Pressed,

                LT = GamePad.GetState((PlayerIndex)p).Triggers.Left,
                RT = GamePad.GetState((PlayerIndex)p).Triggers.Right,

                LeftStick = GamePad.GetState((PlayerIndex)p).ThumbSticks.Left,
                RightStick = GamePad.GetState((PlayerIndex)p).ThumbSticks.Right
            };

            return c;
        }
    }
}
