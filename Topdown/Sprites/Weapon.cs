using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topdown.Sprites
{
    public enum WeaponTypes
    {
        RPG = 0,
        SMG = 1,
        Pistol = 2
    }
    
    public class Weapon
    {
        public WeaponTypes Type { get; set; }
        public int Ammo { get; set; }
        public BulletConfig BulletConfig { get; set; }
    }
}