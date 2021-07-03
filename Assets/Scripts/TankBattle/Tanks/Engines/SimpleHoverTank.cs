using System;
using Photon.Compression;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace TankBattle.Tanks.Engines
{
    public class SimpleHoverTank : ATankEngine
    {
        [FormerlySerializedAs("HoverDistance")]
        public float HoverDistance;

        [FormerlySerializedAs("HoverForce")]
        public float HoverForce;
        public float HoverRotationForce;
        
        public float ForwardAccel = 120.0f;
        public float BackwardAccel = 90.0f;
        public float TurnRate = 70f;

        private float _speedMultiplier = 1.0f;
        public float SpeedMultiplier
        {
            get => _speedMultiplier;
            set => _speedMultiplier = value < 0.1f ? 0.1f : value;
        }

        private float _turnInput;
        private float _thrustInput;
        
        public override float InputHorizontalAxis
        {
            set => _turnInput = value;
        }
        public override float InputVerticalAxis
        {
            set => _thrustInput = value;
        }

        private float _distanceToGround;
        private Rigidbody _rigidbody;
        private LayerMask _groundLayerMask;

        private void Awake()
        {
            _groundLayerMask = LayerMask.GetMask(new string[]
            {
                "Ground"
            });

            _rigidbody = GetComponent<Rigidbody>();
        }

        public float Tolerance = 0.2f;


        public override void UpdateTank()
        {
            // Position and rotation above ground (hovering) - custom gravity
            Vector3 floorNormal = Vector3.up;
            
            if(Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, Mathf.Infinity, 
                _groundLayerMask))
            {
                _distanceToGround = hit.distance;
                floorNormal = hit.normal;
            }
            
            float distance = HoverDistance - _distanceToGround;
            if (Mathf.Abs(distance) >= Tolerance)
            {
                Vector3 position = transform.position;
                position += transform.up * (Mathf.Sign(distance) * Time.fixedDeltaTime * HoverForce);
                transform.position = position;
            }

            // floorNormal.y = 0f;
            // floorNormal.x = transform.rotation.x - floorNormal.x;
            // floorNormal.z = transform.rotation.z - floorNormal.z;
            //
            // Vector3 a = floorNormal * (delta * HoverRotationForce);
            // Vector3 currentAngularVelocity = _rigidbody.angularVelocity;
            // _rigidbody.angularVelocity += a;
            // Debug.Log($"Current angular velocity {currentAngularVelocity}, Floor normal {floorNormal}, A {a}, New angular velocity {_rigidbody.angularVelocity}");


            float prevYRotation = transform.rotation.y;
            // Quaternion rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(floorNormal), delta * HoverRotationForce);
            // rotation.y = prevYRotation;

            Quaternion rotation = Quaternion.FromToRotation(transform.rotation.eulerAngles, floorNormal);
            Debug.Log($"FloorNormal: {floorNormal}, CurrentRotation{transform.rotation}, New rotation: {rotation}");
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.fixedDeltaTime * HoverForce);

            // Movement
            if (Mathf.Abs(_thrustInput) > 0f)
            {
                // Debug.Log($"Thrust {_thrust}");
                float accel = _thrustInput > 0f ? ForwardAccel : BackwardAccel;
                accel *= _speedMultiplier;
                _rigidbody.AddForce(transform.forward * (_thrustInput * accel), ForceMode.Acceleration);
            }

            // _rigidbody.angularVelocity = Vector3.up * (_turnInput * TurnRate * _speedMultiplier);
            transform.Rotate(Vector3.up, _turnInput * TurnRate * _speedMultiplier);

        }
    }
}