using UnityEngine;

namespace TankBattle.Tanks.Bullets
{
    public abstract class ATankBullet : MonoBehaviour, ITankBullet
    {
        public abstract void Fire(Transform parent);
    }
}