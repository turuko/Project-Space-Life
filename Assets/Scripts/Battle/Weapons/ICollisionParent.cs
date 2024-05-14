using UnityEngine;

namespace Battle.Weapons
{
    public interface ICollisionParent
    {
        public void OnCollision(Collision other);
    }
}