using System;
using System.Runtime.CompilerServices;
using OpenTK;

namespace OpenGL_in_CSharp
{
    public class Light
    {
        public Vector4 Position { set; get; }
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

    public class PointLight : Light
    {
        public Vector3 Direction { set; get; }
        public float ConstantAtt { get; }
        public float LinearAtt { get; }
        public float QuadraticAtt { get; }

        public PointLight(Vector4 pos, Vector3 dir, float c, float l, float q) : base(pos)
        {
            Direction = dir;
            ConstantAtt = c;
            LinearAtt = l;
            QuadraticAtt = q;
        }
    }

    public class ConeLight : PointLight
    {
        public float CutOff { get; }

        public float OuterCutOff { get; }

        /// <param name="cutOffDeg">Cutoff for full color in degrees</param>
        /// <param name="outerCutOffDeg">Cutoff for fading color (greater than cutOffDeg) in degrees</param>
        public ConeLight(Vector4 pos, Vector3 dir, float c, float l, float q, float cutOffDeg, float outerCutOffDeg)
            : base(pos, dir, c, l, q)
        {
            CutOff = (float) Math.Cos(MathHelper.DegreesToRadians(cutOffDeg));
            OuterCutOff = (float) Math.Cos(MathHelper.DegreesToRadians(outerCutOffDeg));
        }

        public ConeLight(Vector3 pos, Vector3 dir, float c, float l, float q, float cutOffDeg, float outerCutOffDeg)
            : this(new Vector4(pos, 1), dir, c, l, q, cutOffDeg, outerCutOffDeg) 
        { }
            /*
            public ConeLight(Vector4 pos, Vector3 col, Vector3 direction, float angle) 
                : base(pos, col)
            {
                Direction = direction;
                CutOff = (float) Math.Cos(MathHelper.DegreesToRadians(angle));
            }*/
        }
}
