using System;
using System.Linq;
using SimpleEngine.GameScene;
using SimpleEngine.Utils;
using OpenTK;
using SimpleEngine.Collisions;
using System.Collections.Generic;

namespace SimpleEngine.WorldObjects
{
    /// <summary>
    /// Represents a SceneObject which can collide with a player (rn only with a player)
    /// As most of my objects are round I decided not to implement a simple AABB hitbox, but to go with
    /// a cylinder perpendicular to plane xz (its upper and lower circle bases are parallel to xz plane)
    /// </summary>
    public class Collidable : WorldObject, ICollidable
    {
        public float Height { get; }
        public float Radius { get; }
        
        public Collidable(Mesh mesh, ModelTransformations initialTransformations) 
            : this(mesh) 
        {
            ModelTransformations.Add(initialTransformations);
        }

        public Collidable(Mesh mesh, Vector3 initialPosition) 
            : this(mesh, new ModelTransformations() { Position = initialPosition }) { }

        public Collidable(Mesh mesh) : base(mesh) 
        {
            Radius = Math.Max(
                Math.Max(Math.Abs(RawMesh.Model.MinX), Math.Abs(RawMesh.Model.MaxX)),
                Math.Max(Math.Abs(RawMesh.Model.MinZ), Math.Abs(RawMesh.Model.MaxZ))
                );
            Height = Math.Abs(RawMesh.Model.MaxY) + Math.Abs(RawMesh.Model.MinY);
        }

        public Collidable(Mesh mesh, List<ModelTransformations> modelTransformations)
            : this(mesh)
        {
            ModelTransformations = modelTransformations;
        }

        public bool IsColliding(Player player)
        {
            return ModelTransformations.Exists(trans =>
                (player.Position.Y <= trans.Position.Y + Height) && 
                (player.Position.Y >= trans.Position.Y) &&
                (Vector2.Distance(player.Position.Xz, trans.Position.Xz) <= Radius + player.Radius));
        }

        public ModelTransformations GetCollidingPosition(Player player)
        {
            return ModelTransformations.Where(trans =>
                ((player.Position.Y <= trans.Position.Y + Height) ||
                (player.Position.Y >= trans.Position.Y)) &&
                (Vector2.Distance(player.Position.Xz, trans.Position.Xz) <= Radius + player.Radius))
                .FirstOrDefault();
        }

        public virtual void OnCollisionCheck(object source, CollisionArgs args)
        {
            ModelTransformations collidingObject = GetCollidingPosition(args.PointOfCollision);
            if (collidingObject != null)
            {
                ReactToCollision(args.PointOfCollision, collidingObject);
            }
        }

        /// <summary>
        /// Player needs to be moved out of collision (only in xz plane), to do this we
        /// calculate vector from colliding object to player, then extend it length (out of collision)
        /// and add this vector to colliding object's position
        /// </summary>
        public virtual void ReactToCollision(Player player, ModelTransformations transformations)
        {
            var collisionVector = Vector2.Subtract(player.Position.Xz, transformations.Position.Xz);
            collisionVector.Resize(Radius + player.Radius + 0.001f);

            player.SetNewPositionOnMap(transformations.Position.Xz + collisionVector);
        }
    }
}
