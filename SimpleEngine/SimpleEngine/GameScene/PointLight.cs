using OpenTK;

namespace SimpleEngine.GameScene
{
    public class PointLight : Light
    {
        public PointLight(Vector3 pos, Vector3 col, Vector3 dir, float c, float l, float q)
            : base(new Vector4(pos, 1.0f), col, dir, c, l, q)
        {
        }
    }
}
