using OpenTK;

namespace SimpleEngine.GameScene
{
    /// <summary>
    /// Point light cast light in all direction (just as directional light)
    /// but the intensity decreases with increasing distance form the light source
    /// </summary>
    public class PointLight : Light
    {
        public PointLight(Vector3 pos, Vector3 col, Vector3 dir, float c, float l, float q)
            : base(new Vector4(pos, 1.0f), col, dir, c, l, q)
        {
        }
    }
}
