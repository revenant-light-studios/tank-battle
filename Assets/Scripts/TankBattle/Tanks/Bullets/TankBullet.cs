using System;
using UnityEngine;

namespace TankBattle.Tanks.Bullets
{
    public class TankBullet : MonoBehaviour
    {
        private float _force = 1f;

        private Rigidbody _rigidBody;
        
        public void Fire(Transform parent, float force)
        {
            _rigidBody = GetComponent<Rigidbody>();
            transform.rotation = parent.rotation;
            transform.position = parent.position + parent.forward * 4f;
            _rigidBody.AddForce(parent.forward.normalized * force, ForceMode.Impulse);
        }

        private void OnTriggerEnter(Collider other)
        {
            Destroy(gameObject);
        }
    }
}