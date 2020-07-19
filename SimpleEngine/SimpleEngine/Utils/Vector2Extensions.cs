using OpenTK;

namespace SimpleEngine.Utils
{
    public static class Vector2Extensions 
    { 
        /// <summary>
        /// Provides a simple extension for extending a vector to the given length
        /// </summary>
        public static void Resize(ref this Vector2 vec, float newLength)
        {
            vec *= (newLength / vec.Length);
        }
    }

}
