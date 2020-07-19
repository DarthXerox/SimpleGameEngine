using OpenTK;

namespace SimpleEngine.Utils
{
    /// <summary>
    /// Represents all possible transformations of a 3D object in a game
    /// </summary>
    public class Transformations
    {
        public float RotX { set; get; }
        public float RotY { set; get; }
        public float RotZ { set; get; }
        public Vector3 Scaling { set; get; } = new Vector3(1.0f);
        public Vector3 Position { set; get; } = new Vector3(0.0f);

        public Transformations() { }

        public Transformations(float rotX, float rotY, float rotZ, float scalingFactor, Vector3 position)
        {
            RotX = rotX;
            RotY = rotY;
            RotZ = rotZ;
            Scaling *= scalingFactor;
            Position = position;
        }

        public Matrix4 GetModelMatrix()
        {
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
