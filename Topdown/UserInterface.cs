using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Topdown
{
    public static class UserInterface
    {
        public static void UpdateInterface()
        {
            TopdownGame.SpriteBatch.DrawString(TopdownGame.Font, TopdownGame.Hero.Health.ToString(), new Vector2(TopdownGame.Screen.Top + 10, TopdownGame.Screen.Left + 10), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
        }
    }
}
