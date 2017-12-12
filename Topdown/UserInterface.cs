using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Topdown.Sprites;

namespace Topdown
{
    public static class UserInterface
    {
        public static void UpdateInterface()
        {
            TopdownGame.SpriteBatch.DrawString(TopdownGame.Font, TopdownGame.Hero.Health.ToString(), new Vector2(TopdownGame.Screen.Top + 10, TopdownGame.Screen.Left + 10), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
            for (var i = 0; i < TopdownGame.Hero.CurrentWeapons.Count; i++)
            {
                var weapon = TopdownGame.Hero.CurrentWeapons[i];
                Texture2D tex;
                if (weapon.Type == WeaponTypes.Pistol)
                {
                    tex = TopdownGame.Pistol;
                }
                else if (weapon.Type == WeaponTypes.SMG)
                {
                    tex = TopdownGame.SMG;
                }
                else
                {
                    tex = TopdownGame.RPG;
                }
                TopdownGame.SpriteBatch.Draw(tex, new Rectangle(i * 55 + 5, TopdownGame.Screen.Bottom - 55, 50, 50), tex.Bounds, i == TopdownGame.Hero.SelectedWeapon ? Color.White : Color.Gray);
            }
        }
    }
}