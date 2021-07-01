using System;
using TankBattle.Tanks.Engines;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks.Powerups
{
    public class TankSpeedUp : APowerUp
    {
        [SerializeField, FormerlySerializedAs("SpeedMultiplier"), Range(1.0f, 3.0f)]
        private float SpeedMultiplier = 1.5f;
        
        private TankManager _tankManager;
        private HoverTank _tankEngine;
        private float _startTime = Single.NaN;
        private float _prevSpeedMultiplier;
        
        public override void ApplyPowerup(TankManager tankManager)
        {
            _tankManager = tankManager;
            _tankEngine = tankManager.GetComponent<HoverTank>();
            transform.SetParent(_tankManager.transform);
            
            _prevSpeedMultiplier = _tankEngine.SpeedMultiplier;
            _tankEngine.SpeedMultiplier = SpeedMultiplier;
            _startTime = _time;
        }

        private void RemovePowerup()
        {
            _startTime = Single.NaN;
            _tankEngine.SpeedMultiplier = _prevSpeedMultiplier;
            Destroy(gameObject);
        }

        private void Update()
        {
            if (_startTime != Single.NaN)
            {
                _startTime -= Time.deltaTime;

                if (_startTime <= 0f)
                {
                    RemovePowerup(); 
                }
            }
        }
    }
}