using UnityEngine;

namespace TankBattle.Tanks.Turrets
{
    public class Turret : ATankTurret
    {
        public CrossHair Crosshair;
        public float TurretRotationSpeed = 10f;
        
        [Header("Cannon settings")]
        public float CannonMinRange = 20f;
        public float CannonMaxRange = 200f;

        public override Vector3 MousePosition
        {
            set => _mousePosition = value;
        }

        private Vector3 _mousePosition;
        private Vector3 _mouseHitPosition;

        private CrossHair _crossHair;

        private void Start()
        {
            _crossHair = Instantiate(Crosshair);
        }
        public override void UpdateTurret()
        {
            // Turret pointing
            Ray ray = Camera.main.ScreenPointToRay(_mousePosition);
            
            int layerMask = 1 << LayerMask.NameToLayer("Ground");
            if (Physics.Raycast(ray, out RaycastHit hit, layerMask))
            {
                _mouseHitPosition = hit.point;
                Vector3 direction = _mouseHitPosition - transform.position;
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
                Vector3 hitPointVector = new Vector3(_mouseHitPosition.x, 0.1f, _mouseHitPosition.z) - transform.position;
                float distance = Mathf.Clamp(hitPointVector.magnitude, CannonMinRange, CannonMaxRange);
                hitPointVector = transform.position + hitPointVector.normalized * distance; 
                _crossHair.transform.position = new Vector3(hitPointVector.x, 0.1f, hitPointVector.z);

                Vector3 cross = Vector3.Cross(transform.forward, _crossHair.transform.position);
                _crossHair.TargetReached = (Mathf.Abs(cross.y) < 3f);
                // Debug.Log($"{cross}");
            }
        }
    }
}