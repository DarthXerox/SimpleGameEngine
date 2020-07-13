using System;
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

    public class PointLight : Light
    {
        public PointLight(Vector3 pos, Vector3 col, Vector3 dir, float c, float l, float q)
            : base(new Vector4(pos, 1.0f), col, dir, c, l, q)
        {
        }
    }

    public class ConeLight : PointLight
    {
        /// <param name="cutOffDeg">Cutoff for full color in degrees</param>
        /// <param name="outerCutOffDeg">Cutoff for fading color (greater than cutOffDeg) in degrees</param>
        public ConeLight(Vector3 pos, Vector3 col, Vector3 dir, float c, float l, float q, float cutOffDeg, float outerCutOffDeg)
            : base(pos, col, dir, c, l, q)
        {
            CutOff = (float)Math.Cos(MathHelper.DegreesToRadians(cutOffDeg));
            OuterCutOff = (float)Math.Cos(MathHelper.DegreesToRadians(outerCutOffDeg));
        }

        public ConeLight(Vector3 pos, Vector3 dir, float c, float l, float q, float cutOffDeg, float outerCutOffDeg)
            : this(pos, new Vector3(1, 1, 1), dir, c, l, q, cutOffDeg, outerCutOffDeg)
        {
        }
    }
}
