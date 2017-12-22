namespace Game.Sprites
{
    /// <summary>
    /// Weapon types
    /// </summary>
    public enum WeaponTypes
    {
        RPG = 0,
        SMG = 1,
        Pistol = 2
    }

    /// <summary>
    /// Information on the current weapon
    /// </summary>
    public class Weapon
    {
        public WeaponTypes Type { get; set; }
        public int Ammo { get; set; }
        public BulletConfig BulletConfig { get; set; }
    }
}