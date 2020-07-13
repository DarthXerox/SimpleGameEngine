using SimpleEngine.GameScene;

namespace SimpleEngine.Collisions
{
    /// <summary>
    /// Collision detection in my game uses 1 type of hitboxes:
    /// -> cylinder perpendicular to plane xz (its upper and lower circle bases are parallel to xz plane)
    /// Implementing axis-aligned bounding box (AABB hitbox) would be slightly easier, 
    /// but most of my objects are round, so collsions would be a bit less realistic
    /// </summary>
    public class CollisionManager
    {
        public Player PointOfCollision { private set; get; }
        public delegate void CollisionCheckingEventHandler(object sauce, CollisionArgs args);
        public event CollisionCheckingEventHandler CollisionChecking;

        public CollisionManager(Player player)
        {
            PointOfCollision = player;
        }

        public void CheckCollisions()
        {
            OnCollisionChecking();
        }

        protected virtual void OnCollisionChecking()
        {
            CollisionChecking?.Invoke(this, new CollisionArgs(PointOfCollision));
        }
    }
}
