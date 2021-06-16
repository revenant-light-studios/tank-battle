using UnityEngine;

namespace TankBattle.Global
{
    public class ParticleCollisionDelegate : MonoBehaviour
    {
        public delegate void OnParticleCollisionDelegate(GameObject other);
        public OnParticleCollisionDelegate OnChildParticleCollision;
        
        private void OnParticleCollision(GameObject other)
        {
            OnChildParticleCollision?.Invoke(other);
        }
    }
}