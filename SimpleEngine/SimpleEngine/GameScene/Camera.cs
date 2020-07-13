using System;
using OpenTK;
using OpenTK.Input;

namespace SimpleEngine.GameScene
{
    public class Camera
    {
        public Vector3 Position { protected set; get; }
        public Vector3 Target { private set; get; }
        public Vector3 Up { private set; get; }
        public Vector3 Front { get; protected set; }
        public bool FirstMove { set; get; } = true;
        public Vector2 LastMousePos { private set; get; }

        public float Yaw { private set; get; } = 45f;
        public float Pitch { private set; get; } = 0;
        public float Speed { protected set; get; } = 1f;
        public float Sensitivity { get; private set; } = 0.1f;


        public Camera(Vector3 pos, Vector3 cameraTarget, Vector3 up)
        {
            Position = pos;
            Target = cameraTarget;
            Up = up;
        }

        public Camera(Vector3 pos)
        {
            Position = pos;
            Up = new Vector3(0, 1, 0);
            Front = new Vector3(0, 0, -1);
        }

        public Vector3 GetRight()
        {
            return Vector3.Normalize(Vector3.Cross(Front, Up));
        }

        public Matrix4 GetViewMatrixAtTarget()
        {
            return Matrix4.LookAt(Position, Target, Up);
        }

        public virtual Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + Front, Up);
        }


        public virtual void Move(MouseState mouse)
        {
            UpdateAngles(mouse);
            Front = Vector3.Normalize(new Vector3(
                (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(Yaw)),
                (float)Math.Sin(MathHelper.DegreesToRadians(Pitch)),
                (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(Yaw))
                ));
            Move();
        }

        /// <summary>
        /// Handles movement of the camera
        /// </summary>
        public virtual void Move()
        {
            var keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Key.W))
            {
                Position += Front * Speed;
            }
            if (keyState.IsKeyDown(Key.S))
            {
                Position -= Front * Speed;
            }

            if (keyState.IsKeyDown(Key.D))
            {
                Position += GetRight() * Speed;
            }
            if (keyState.IsKeyDown(Key.A))
            {
                Position -= GetRight() * Speed;
            }


            if (keyState.IsKeyDown(Key.KeypadPlus))
            {
                Position = new Vector3(Position.X, Position.Y + Speed, Position.Z);
            }
            if (keyState.IsKeyDown(Key.KeypadMinus))
            {
                Position = new Vector3(Position.X, Position.Y - Speed, Position.Z);
            }
        }

        /// <summary>
        /// Based on mouse position difference from last fram recalcualtes new angles - pitch and yaw
        /// </summary>
        /// <param name="mouse"></param>
        public virtual void UpdateAngles(MouseState mouse)
        {
            if (FirstMove)
            {
                LastMousePos = new Vector2(mouse.X, mouse.Y);
                FirstMove = false;
            }
            else
            {
                float deltaX = mouse.X - LastMousePos.X;
                float deltaY = LastMousePos.Y - mouse.Y;
                LastMousePos = new Vector2(mouse.X, mouse.Y);

                Yaw += deltaX * Sensitivity;
                Pitch += deltaY * Sensitivity;

                if (Pitch > 89.0f)
                {
                    Pitch = 89.0f;
                }
                else if (Pitch < -89.0f)
                {
                    Pitch = -89.0f;
                }
            }
        }
    }
}
