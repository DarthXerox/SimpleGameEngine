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
        public Vector3 Front { get; private set; }
        public bool FirstMove { set; get; } = true;
        public Vector2 LastMousePos { private set; get; }

        public float Yaw { private set; get; } = 45f;
        public float Pitch { private set; get; } = 0;
        public float Speed { private set; get; } = 1f;


        /// <summary>
        /// This is REVERSED camera direction (its being reversed is useful for further calculations)
        /// </summary>
        public Vector3 Direction { private set; get; }
        public float Sensitivity { get; private set; } = 0.1f;

        public Camera(Vector3 pos, Vector3 cameraTarget, Vector3 up)
        {
            Position = pos;
            Target = cameraTarget;
            Up = up;
            Direction = Vector3.Normalize(Position - cameraTarget);
            Right = Vector3.Normalize(Vector3.Cross(Up, Direction));
        }

        public Camera()
        {
            Position = Vector3.Zero;
            Up = new Vector3(0,1,0);
            Front = new Vector3(0,0,-1);
        }
        /*
        public static Camera GenerateOmnipotentCamera(Vector3 firstTargetPosition)
        {
            Camera cam = new Camera
            {
                Position = new Vector3(firstTargetPosition.X + 3,
                    firstTargetPosition.Y + 3,
                    firstTargetPosition.Z + 3)
            };
            cam.Up = firstTargetPosition - cam.Position;
            
            return cam;
        }
        */
        public Vector3 GetRight()
        {
            return Vector3.Normalize(Vector3.Cross(Front, Up));
        }

        public Matrix4 GetViewMatrixAtTarget()
        {
            return Matrix4.LookAt(Position, Target, Up);
        }

        public Matrix4 GetViewMAtrix()
        {
            return Matrix4.LookAt(Position, Position + Front, Up);
        }




        public void Move()
        {
            var keyState = Keyboard.GetState();
            if  (keyState.IsKeyDown(Key.W))
            {
                Position += Front * Speed; //new Vector3(Position.X, Position.Y, Position.Z - 0.3f);
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
                Position = new Vector3(Position.X , Position.Y + Speed, Position.Z);
            }
            if (keyState.IsKeyDown(Key.KeypadMinus))
            {
                Position = new Vector3(Position.X, Position.Y - Speed, Position.Z);
            }
            

            /*if (keyState.IsKeyDown(Key.X))
            {
                Target = new Vector3(0,0,0);
            }
            Target = new Vector3(Position.X, Position.Y, Position.Z - 2);
            */
        }


        public void Move(MouseState mouse) 
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
                if (Pitch > 89.0f)
                {
                    Pitch = 89.0f;
                }
                else if (Pitch < -89.0f)
                {
                    Pitch = -89.0f;
                }
                else
                {
                    Pitch += deltaY * Sensitivity;
                }
            }

            Front = Vector3.Normalize(new Vector3(
                (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(Yaw)),
                (float)Math.Sin(MathHelper.DegreesToRadians(Pitch)),
                (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(Yaw))
                ));

            Move();
        }
    }
}
