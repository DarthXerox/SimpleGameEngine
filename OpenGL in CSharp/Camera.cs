using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using OpenTK;
using OpenTK.Input;

namespace OpenGL_in_CSharp
{
    public class Camera
    {
        public Vector3 Position { private set; get; }
        public Vector3 Target { private set; get; }
        public Vector3 Up { private set; get; }
        public Vector3 Right { private set; get; }

        public Vector3 Front { get { return -1 * Direction; } }

        /// <summary>
        /// This is REVERSED camera direction (its being reverse is useful for calculations
        /// </summary>
        public Vector3 Direction { private set; get; }

        public Camera(Vector3 pos, Vector3 cameraTarget, Vector3 up)
        {
            Position = pos;
            Target = cameraTarget;
            Up = up;
            Direction = Vector3.Normalize(Position - cameraTarget);
            Right = Vector3.Normalize(Vector3.Cross(Up, Direction));
        }


        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Target, Up);
        }

        public void Move()
        {
            var keyState = Keyboard.GetState();
            if  (keyState.IsKeyDown(Key.W))
            {
                Position = new Vector3(Position.X, Position.Y, Position.Z + 0.3f);
            }
            if (keyState.IsKeyDown(Key.S))
            {
                Position = new Vector3(Position.X, Position.Y, Position.Z - 0.3f);
            }

            if (keyState.IsKeyDown(Key.D))
            {
                Position = new Vector3(Position.X + 0.3f, Position.Y, Position.Z );
            }
            if (keyState.IsKeyDown(Key.A))
            {
                Position = new Vector3(Position.X - 0.3f, Position.Y, Position.Z);
            }

            if (keyState.IsKeyDown(Key.KeypadPlus))
            {
                Position = new Vector3(Position.X , Position.Y + 0.3f, Position.Z);
            }
            if (keyState.IsKeyDown(Key.KeypadMinus))
            {
                Position = new Vector3(Position.X, Position.Y - 0.3f, Position.Z);
            }
        }
    }
}
