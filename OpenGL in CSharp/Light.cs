using OpenTK;

namespace OpenGL_in_CSharp
{
    public class Light
    {
        public Vector4 Position { get; }
        public Vector3 Color { get; }

        public Light(Vector4 pos, Vector3 col)
        {
            Position = pos;
            Color = col;
        }

        public Light(Vector4 pos)
        {
            Position = pos;
            Color = new Vector3(1.0f);
        }

        public Light(Vector3 pos, bool isDirectional)
        {
            float w = isDirectional ? 0.0f : 1.0f;
            Position = new Vector4(pos, w);
            Color = new Vector3(1.0f);
        }
    }
}
