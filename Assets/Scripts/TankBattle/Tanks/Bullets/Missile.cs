using System;
using ExtensionMethods;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks.Bullets
{
    public class Missile : ATankBullet
    {
        [SerializeField, FormerlySerializedAs("MissileSpeed")]
        private float _speed = 5000f;
        
        private GameObject _projectile;
        
        public override void Fire(Transform parent)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.forward * _speed, ForceMode.Force);
        }

        private void OnCollisionEnter(Collision other)
        {
            OnBulletHit?.Invoke(other.gameObject);
            Destroy(gameObject);
        }
    }
}