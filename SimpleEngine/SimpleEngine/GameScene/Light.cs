using OpenTK;

namespace SimpleEngine.GameScene
{
    /// <summary>
    /// Represents all parametres a light can have
    /// Even directional light has them (but it has different Position.W parameter)
    /// </summary>
    public class Light
    {
        public Vector4 Position { set; get; }
        public Vector3 Color { set; get; } = new Vector3(1.0f);
        public Vector3 Direction { set; get; } = new Vector3(1.0f);
        public float ConstantAtt { protected set; get; } = 1.0f;
        public float LinearAtt { protected set; get; } = 0.0f;
        public float QuadraticAtt { protected set; get; } = 0.0f;
        public float CutOff { protected set; get; } = -1.0f;
        public float OuterCutOff { protected set; get; } = -1.0f;


        /// Constructs basic diretional light
        public Light(Vector3 pos)
        {
            Position = new Vector4(pos, 0.0f);
        }

        protected Light(Vector4 pos, Vector3 col, Vector3 dir, float c, float l, float q)
        {
            Position = pos;
            Color = col;
            Direction = dir;
            ConstantAtt = c;
            LinearAtt = l;
            QuadraticAtt = q;
        }
    }
}
