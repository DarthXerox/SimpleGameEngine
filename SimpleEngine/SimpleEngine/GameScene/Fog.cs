using OpenTK;

namespace SimpleEngine.GameScene
{
    /// <summary>
    /// Contains simple parametres for fog calculation
    /// </summary>
    public class Fog
    {
        public Vector3 Color { get; }
        public float Density { get; }

        public Fog(float density, Vector3 color)
        {
            Density = density;
            Color = color;
        }
    }
}
