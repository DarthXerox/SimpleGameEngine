﻿using SimpleEngine.GameScene;
using SimpleEngine.Utils;

namespace SimpleEngine.Collisions
{
    /// <summary>
    /// Defines an interface for a world 3D object that can collide with a player
    /// </summary>
    public interface ICollidable
    {
        bool IsColliding(Player player);
        void OnCollisionCheck(object source, CollisionArgs args);
        void ReactToCollision(Player player, Transformations transformations);
    }
}
