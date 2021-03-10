using UnityEngine;

namespace TankBattle.Tanks.Bullets
{
    public interface ITankBullet
    {
        void Fire(Transform parent);
    }
}