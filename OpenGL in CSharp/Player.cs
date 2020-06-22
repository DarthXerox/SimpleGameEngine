using System;
using OpenGL_in_CSharp.Utils;
using OpenTK;
using OpenTK.Input;

namespace OpenGL_in_CSharp
{
    public class Player : Camera, ICollidable
    {
        public Map AssociatedMap { private set; get; }
        public ConeLight Flashlight { set; get; }
        public float Height { private set; get; } = 5f;
        public Vector3 LowerCentre{ get => Position - new Vector3(0, Height, 0); }
        public float Radius { private set; get; } = 3f;

        public DateTime LastEPressed = new DateTime(2000, 1, 1);

        public Player(Vector3 position, Map map) : base(position)
        {
            AssociatedMap = map;
            Flashlight = new ConeLight(new Vector3(position.X, Height, position.Z), new Vector3(1,0,1), 1f, 0.09f, 0.032f, 12.5f, 17.5f);
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
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Key.E) && (DateTime.Now - LastEPressed).Milliseconds > 200)
            {
                if (Flashlight.Color != Vector3.Zero)
                {
                    Flashlight.Color = new Vector3(0, 0, 0);
                } else
                {
                    Flashlight.Color = new Vector3(1, 1, 1);
                }
                LastEPressed = DateTime.Now;
            }

            var pos = Position;
            pos.Y = AssociatedMap.GetHeight(pos.X, pos.Z) + Height;
            if (pos.X <= 0 + Radius)
            {
                pos.X = Radius + 0.01f;
            } else if (pos.X >= AssociatedMap.MaxX - Radius)
            {
                pos.X = AssociatedMap.MaxX - Radius - 0.01f;
            }

            if (pos.Z <= 0 + Radius)
            {
                pos.Z = Radius + 0.01f;
            }
            else if (pos.Z >= AssociatedMap.MaxZ - Radius)
            {
                pos.Z = AssociatedMap.MaxZ - Radius - 0.01f;
            }
            Position = pos;

            Flashlight.Position = new Vector4(Position, 1.0f);
            Flashlight.Direction = GetEyeFront();
        }

        public Vector3 GetEyeFront()
        {
            return Vector3.Normalize(new Vector3(
                (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(Yaw)),
                (float)Math.Sin(MathHelper.DegreesToRadians(Pitch)),
                (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(Yaw))
                ));
        }

        public override Matrix4 GetViewMatrix()
        {
            /*var temp = Vector3.Normalize(new Vector3(
                (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(Yaw)),
                (float)Math.Sin(MathHelper.DegreesToRadians(Pitch)),
                (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(Yaw))
                ));*/
            return Matrix4.LookAt(Position, Position + GetEyeFront(), Up);

        }

        public bool IsColliding(ICollidable other)
        {
            if (LowerCentre.Y > other.LowerCentre.Y)
            {
                if (LowerCentre.Y <= other.LowerCentre.Y + other.Height)
                {
                    return LowerCentre.IsWithinDistanceInPlane(other.LowerCentre, Radius + other.Radius);
                } 
            }
            else
            {
                if (other.LowerCentre.Y <= LowerCentre.Y + Height)
                {
                    return LowerCentre.IsWithinDistanceInPlane(other.LowerCentre, Radius + other.Radius);
                }
            }

            return false;
        }

        public void OnCollisionCheck(object source, CollisionArgs args)
        {
            if (IsColliding(args.PointOfCollision))
            {
                args.PointOfCollision.ReactToCollision(this);
            }
        }

        public void ReactToCollision(ICollidable other)
        {
            var temp = Position;
            temp.MoveFromInPlane(other.Position, Radius + other.Radius + 0.01f);
            temp.Y = AssociatedMap.GetHeight(temp.X, temp.Z) + Height;
            Position = temp;

        }
    }
}
