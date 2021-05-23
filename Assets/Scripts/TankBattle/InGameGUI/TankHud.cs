using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using ExtensionMethods;
using Photon.Pun;
using TankBattle.Global;
using TankBattle.Tanks;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.InGameGUI
{
    public class TankHud : MonoBehaviour
    {
        protected ValueBar _lifeBar;
        protected ValueBar _shieldBar;

        protected TankValues _tankValues;
        protected HitImage _hitImage;

        protected void Awake()
        {
            _hitImage = transform.FirstOrDefault(t => t.name == "HitImage")?.GetComponent<HitImage>();
            _lifeBar = transform.FirstOrDefault(t => t.name == "LifeBar")?.GetComponent<ValueBar>();
            _shieldBar = transform.FirstOrDefault(t => t.name == "ShieldBar")?.GetComponent<ValueBar>();
        }
        
        public void RegisterTank(GameObject tank)
        {
            _tankValues = tank.GetComponent<TankValues>();
            if (_tankValues != null)
            {
                _tankValues.OnValuesChanged = OnTankValuesChanged;
                _tankValues.OnTankWasHit = OnTankWasHit;
                Debug.LogFormat("Tank {0} registered with hud", tank.name);
            }
        }
        protected void OnTankWasHit(TankValues values)
        { 
            _hitImage?.HitFlash();
        }
        protected void OnTankValuesChanged(TankValues values)
        {
            if(_lifeBar) _lifeBar.CurrentValue = values.ArmorAmount / values.TotalArmor;
            if(_shieldBar) _shieldBar.CurrentValue = values.ShieldAmount / values.TotalShield;
        }
    }
}