using System;
using OpenTK;
using OpenTK.Input;

namespace OpenGL_in_CSharp
{
    public class Player : Camera
    {
        public Map AssociatedMap { private set; get; }
        public float PlayerHeight { private set; get; } = 10f;

        public SceneObject Arms { private set; get; }

        public Player(Vector3 position, Map map) : base(position)
        {
            AssociatedMap = map;
        }

        public override void Move(MouseState mouse)
        {
            base.UpdateAngles(mouse);

            Front = Vector3.Normalize(new Vector3(
                (float)Math.Cos(MathHelper.DegreesToRadians(Yaw)),
                0,
                (float)Math.Sin(MathHelper.DegreesToRadians(Yaw))
                ));

            base.Move();

            var pos = Position;
            pos.Y = AssociatedMap.GetHeight(pos.X, pos.Z) + PlayerHeight;
            Position = pos;
        }

        public override Matrix4 GetViewMatrix()
        {
            var temp = Vector3.Normalize(new Vector3(
                (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(Yaw)),
                (float)Math.Sin(MathHelper.DegreesToRadians(Pitch)),
                (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(Yaw))
                ));
            return Matrix4.LookAt(Position, Position + temp, Up);

        }
    }
}
