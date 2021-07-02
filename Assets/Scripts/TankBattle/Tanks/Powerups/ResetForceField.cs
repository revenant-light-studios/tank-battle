using UnityEngine;

namespace TankBattle.Tanks.Powerups
{
    public class ResetForceField : APowerUp
    {

        public override bool ApplyPowerup(TankManager tankManager)
        {
            TankValues _tankValues = tankManager.GetComponent<TankValues>();
            Debug.Log($"Current shield amount {_tankValues.ShieldAmount}");
            if (_tankValues.ShieldAmount >= 99f)
            {
                Destroy(gameObject);
                return false;
            }
            
            tankManager.ResetForceField();
            return true;
        }
    }
}