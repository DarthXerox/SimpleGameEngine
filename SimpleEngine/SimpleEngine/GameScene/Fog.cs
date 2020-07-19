using OpenTK;

namespace SimpleEngine.GameScene
{
    /// <summary>
    /// Contains simple parameters for fog calculation
    /// </summary>
    public class Fog
    {
        /// <summary>
        /// Note that for the fog to work correctly,
        /// its color must be the same as the world background color (GL.ClearColor)
        /// </summary>
        public Vector3 Color { get; }
        public float Density { get; }

        public Fog(float density, Vector3 color)
        {
            Density = density;
            Color = color;
        }
    }
}
