using UnityEngine;

namespace TankBattle.Tanks.Turrets
{
    public abstract class ATankTurret : MonoBehaviour
    {
        public abstract void UpdateTurret(Vector3 angle);
        
        public delegate void OnTurretMoveDelegate(Vector3 position);
        protected OnTurretMoveDelegate _onTurretMove;
        
        public event OnTurretMoveDelegate OnTurretMove
        {
            add { _onTurretMove += value; }
            remove { _onTurretMove -= value; }
        }
    }
}