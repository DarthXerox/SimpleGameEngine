using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL_in_CSharp.Utils;
using OpenTK;

namespace OpenGL_in_CSharp
{
    public class Collidable : SceneObject, ICollidable
    {
        public Vector3 LowerCentre { get => Position; }

        public float Height { get => Math.Abs(RawMesh.Model.MaxY) + Math.Abs(RawMesh.Model.MinY); }

        public float Radius
        {
            get => Math.Max(
                Math.Max(Math.Abs(RawMesh.Model.MinX), Math.Abs(RawMesh.Model.MaxX)),
                Math.Max(Math.Abs(RawMesh.Model.MinZ), Math.Abs(RawMesh.Model.MaxZ))
                );
        }

        public Collidable(string objFileName, string textureFileName) : base(objFileName, textureFileName) 
        { 
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

        public void MoveOutOfCollision(ICollidable other)
        {
            throw new NotImplementedException();
        }

        public void OnCollisionCheck(object source, CollisionArgs args)
        {
            if (IsColliding(args.PointOfCollision))
            {
                args.PointOfCollision.MoveOutOfCollision(this);
            }
        }
    }
}
