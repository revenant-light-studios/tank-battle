using UnityEngine;

namespace TankBattle.Tanks.Bullets
{
    public class PhysicsTankBullet : ATankBullet
    {
        [SerializeField] private float _force = 1f;
        private Rigidbody _rigidBody;
        
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"Hit {other.name}");
            Destroy(gameObject);
        }
        public override void Fire(Transform parent)
        {
            PhysicsTankBullet bullet = Instantiate(this);
            _rigidBody = bullet.GetComponent<Rigidbody>();
            bullet.transform.rotation = parent.rotation;
            bullet.transform.position = parent.position + parent.forward * 4f;
            _rigidBody.AddForce(parent.forward.normalized * _force, ForceMode.Impulse);
            Debug.Log("Fire");
        }
    }
}