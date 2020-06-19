﻿using System;
using OpenGL_in_CSharp.Utils;
using OpenTK;
using OpenTK.Input;

namespace OpenGL_in_CSharp
{
    public class Player : Camera, ICollidable
    {
        public Map AssociatedMap { private set; get; }
        public float Height { private set; get; } = 5f;

        public SceneObject Arms { private set; get; }
        
        
        public Vector3 LowerCentre{ get => Position - new Vector3(0, Height, 0); }
        public float Radius { private set; get; } = 5f;

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
            pos.Y = AssociatedMap.GetHeight(pos.X, pos.Z) + Height;
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
                args.PointOfCollision.MoveOutOfCollision(this);
            }
        }

        public void MoveOutOfCollision(ICollidable other)
        {
            var temp = Position;
            temp.MoveFromInPlane(other.Position, Radius + other.Radius + 0.01f);
            Position = temp;
        }
    }
}