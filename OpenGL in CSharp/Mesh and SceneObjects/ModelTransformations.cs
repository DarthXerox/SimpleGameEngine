using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenGL_in_CSharp.Utils
{
    public class ModelTransformations
    {
        public float RotX { set; get; } = 0.0f;
        public float RotY { set; get; } = 0.0f;
        public float RotZ { set; get; } = 0.0f;
        public Vector3 Scaling { set; get; } = new Vector3(1.0f);
        public Vector3 Position { set; get; } = new Vector3(0.0f);

        public ModelTransformations() { }

        public ModelTransformations(Vector3 pos)
        {
            Position = pos;
        }

        public ModelTransformations(float rotX, float rotY, float rotZ, float scalingFactor, Vector3 position)
        {
            RotX = rotX;
            RotY = rotY;
            RotZ = rotZ;
            Scaling = Scaling * scalingFactor;
            Position = position;
        }

        public Matrix4 GetModelMatrix()
        {
            //https://www.youtube.com/watch?v=oc8Yl4ZruCA&list=PLRIWtICgwaX0u7Rf9zkZhLoLuZVfUksDP&index=7
            Matrix4 model = Matrix4.Identity;
            model *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(RotX));
            model *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(RotY));
            model *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(RotZ));
            model *= Matrix4.CreateScale(Scaling);
            model *= Matrix4.CreateTranslation(Position);

            return model;
        }
    }
}
