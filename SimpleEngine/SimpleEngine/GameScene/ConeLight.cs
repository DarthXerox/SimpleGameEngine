using System;
using OpenTK;

namespace SimpleEngine.GameScene
{
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
