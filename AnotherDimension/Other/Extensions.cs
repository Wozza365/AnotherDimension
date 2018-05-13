namespace Game.Other
{
    /// <summary>
    /// Any additional extension methods that may be used with built in classes or types for example Sqrd with a float
    /// </summary>
    public static class Extensions
    {
        public static float Sqrd(this float f)
        {
            return f * f;
        }

        public static int Sqrd(this int i)
        {
            return i * i;
        }
    }
}