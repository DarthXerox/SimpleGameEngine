using OpenGL_in_CSharp.Utils;

namespace OpenGL_in_CSharp
{
    /// <summary>
    /// Defines an interface for a scene object that can collide with a player
    /// </summary>
    public interface ICollidable
    {
        bool IsColliding(Player player);
        void OnCollisionCheck(object source, CollisionArgs args);
        void ReactToCollision(Player player, ModelTransformations transformations);
    }
}
