using System;
using SimpleEngine.GameScene;

namespace SimpleEngine.Collisions
{
    /// <summary>
    /// Used for sending player as argument of a collsion event
    /// </summary>
    public class CollisionArgs : EventArgs
    {
        public Player PointOfCollision { set; get; }

        public CollisionArgs(Player pointOfCollision)
        {
            PointOfCollision = pointOfCollision;
        }
    }
}
