using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks.Powerups
{
    public abstract class APowerUp : MonoBehaviour, IPowerUp
    {
        [SerializeField, FormerlySerializedAs("PowerupTime"), Tooltip("Time in seconds, -1 infinite")]
        protected float _time;
        
        public abstract void ApplyPowerup(TankManager tankManager);
    }
}