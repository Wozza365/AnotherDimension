using Game.Misc;
using Game.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Sprites;

namespace Game
{
    public static class UserInterface
    {
        public static void UpdateInterface()
        {
            //Basic UI information 
            if (MainGame.GameState == GameState.PLAYINGPLATFORMER)
            {
                MainGame.SpriteBatch.DrawString(MainGame.Font, "Remaining Gems: " + SceneController.GemCount, new Vector2(MainGame.Screen.Left + 10, MainGame.Screen.Top + 10), Color.White, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);

                MainGame.SpriteBatch.DrawString(MainGame.Font, "Health: " + MainGame.PlatformerHero.Health, new Vector2(MainGame.Screen.Left + 10, MainGame.Screen.Top + 40), Color.White, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
            }
            else if (MainGame.GameState == GameState.PLAYINGTOPDOWN)
            {
                MainGame.SpriteBatch.DrawString(MainGame.Font, "Health: " + MainGame.TopdownHero.Health, new Vector2(MainGame.Screen.Left + 10, MainGame.Screen.Top + 10), Color.White, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);

                MainGame.SpriteBatch.DrawString(MainGame.Font, "Ammo: " + MainGame.TopdownHero.CurrentWeapons[MainGame.TopdownHero.SelectedWeapon].Ammo, new Vector2(MainGame.Screen.Left + 10, MainGame.Screen.Top + 40), Color.White, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
            }
            
            //Draw our weapon inventory at the bottom of the screen, an icon for each weapon we possess
            for (var i = 0; i < MainGame.TopdownHero.CurrentWeapons.Count; i++)
            {
                var weapon = MainGame.TopdownHero.CurrentWeapons[i];
                Texture2D tex;
                if (weapon.Type == WeaponTypes.Pistol)
                {
                    tex = MainGame.Pistol;
                }
                else if (weapon.Type == WeaponTypes.SMG)
                {
                    tex = MainGame.SMG;
                }
                else
                {
                    tex = MainGame.RPG;
                }
                MainGame.SpriteBatch.Draw(tex, new Rectangle(i * 55 + 5, MainGame.Screen.Bottom - 55, 50, 50), tex.Bounds, i == MainGame.TopdownHero.SelectedWeapon ? Color.White : Color.Gray);
            }
        }
    }
}