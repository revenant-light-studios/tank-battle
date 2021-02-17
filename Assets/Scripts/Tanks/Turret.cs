using UnityEngine;

namespace Tanks
{
    public class Turret : MonoBehaviour
    {
        public CrossHair Crosshair;
        public float TurretRotationSpeed = 10f;
        
        [Header("Cannon settings")]
        public float CannonMinRange = 20f;
        public float CannonMaxRange = 200f;
        
        private Vector3 _mouseWorldPosition;
        
        private void Update()
        {
            // Turret pointing
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << LayerMask.NameToLayer("Ground");
            if (Physics.Raycast(ray, out RaycastHit hit, layerMask))
            {
                _mouseWorldPosition = hit.point;
                Vector3 direction = _mouseWorldPosition - transform.position;
                Quaternion lookRotation = Quaternion.LookRotation(direction) * Quaternion.Inverse(transform.parent.rotation);

                float destinationAngle = lookRotation.eulerAngles.y;
                if (destinationAngle > 180f) destinationAngle = destinationAngle - 360f;
                destinationAngle = Mathf.Clamp(destinationAngle, -130, 130);

                float sourceAngle = transform.localRotation.eulerAngles.y;
                if (sourceAngle > 180f) sourceAngle = sourceAngle - 360f;
                
                if (sourceAngle > destinationAngle)
                {
                    sourceAngle -= TurretRotationSpeed;
                    if (sourceAngle < destinationAngle) sourceAngle = destinationAngle;
                }
                else if(sourceAngle < destinationAngle)
                {
                    sourceAngle += TurretRotationSpeed;
                    if (sourceAngle > destinationAngle) sourceAngle = destinationAngle;
                }

                Quaternion quaternion = Quaternion.Euler(0f, sourceAngle, 0f);
                transform.localRotation = quaternion;

                // Hit point calculation
                Vector3 hitPointVector = new Vector3(_mouseWorldPosition.x, 0.1f, _mouseWorldPosition.z) - transform.position;
                float distance = Mathf.Clamp(hitPointVector.magnitude, CannonMinRange, CannonMaxRange);
                hitPointVector = transform.position + hitPointVector.normalized * distance; 
                Crosshair.transform.position = new Vector3(hitPointVector.x, 0.1f, hitPointVector.z);

                Vector3 cross = Vector3.Cross(transform.forward, Crosshair.transform.position);
                Crosshair.TargetReached = (Mathf.Abs(cross.y) < 3f);
                // Debug.Log($"{cross}");
            }
        }
    }
}