using System;
using ExtensionMethods;
using UnityEngine;

namespace Tanks
{
    public class Tank : MonoBehaviour
    {
        public bool ExtraFuelTank;
        public bool RocketLauncher;
        public float FlightDistance = 2f;
        public float _force;
        public PIDController altitudPIDController;
        
        private Rigidbody _rigidbody;
        private GameObject _tankBase;
        private GameObject _extraFuelTank;
        private GameObject _rocketLauncher;
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _tankBase = transform.FirstOrDefault(t => t.name == "Body").gameObject;
            _extraFuelTank = transform.FirstOrDefault(t => t.name == "ExtraFuelTank").gameObject;
            _rocketLauncher = transform.FirstOrDefault(t => t.name == "MissileThrower").gameObject;
        }

        private void Start()
        {
            _extraFuelTank.SetActive(ExtraFuelTank);
            _rocketLauncher.SetActive(RocketLauncher);
        }

        float FloorDistance(Vector3 position)
        {
            if (Physics.Raycast(position, -transform.up, out RaycastHit raycastHit))
            {
                return raycastHit.distance;
            }

            return 0f;
        }

        private void FixedUpdate()
        {
            float curAltitude = Mathf.Max(0.1f, FloorDistance(transform.position));
            float error = FlightDistance - curAltitude;
            float thrust = Mathf.Clamp01(altitudPIDController.Update(error));
            _rigidbody.AddForce(Vector3.up * (thrust * _force));
            
        }
    }
}
