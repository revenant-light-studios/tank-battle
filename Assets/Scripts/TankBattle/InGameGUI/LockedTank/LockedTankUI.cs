using System;
using ExtensionMethods;
using TankBattle.Tanks;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.InGameGUI.LockedTank
{
    public class LockedTankUI : MonoBehaviour
    {
        private Text _name;
        private ProgressBar _life;
        private ProgressBar _shield;
        private TankValues _tankValues;

        public TankValues TankValues
        {
            get => _tankValues;
            set
            {
                if (_tankValues != null)
                {
                    SetActive(false);
                    _tankValues.OnValuesChanged -= OnTankValuesChanged;
                    _tankValues.OnTankWasDestroyed -= OnTankWasDestroyed;
                }
                
                _tankValues = value;

                if (value != null)
                {
                    // Debug.Log("Setup locked tank image");
                    SetActive(true);
                    _life.MaxValue = (int)_tankValues.TotalArmor;
                    _shield.MaxValue = (int)_tankValues.TotalShield;
                    _life.Value = (int)_tankValues.ArmorAmount;
                    _shield.Value = (int)_tankValues.ShieldAmount;
                    
                    OnTankValuesChanged(_tankValues);
                    _tankValues.OnValuesChanged += OnTankValuesChanged;
                    _tankValues.OnTankWasDestroyed += OnTankWasDestroyed;
                }
            }
        }
        private void OnTankWasDestroyed(TankValues values)
        {
            TankValues = null;
        }
        private void OnTankValuesChanged(TankValues values)
        {
            _name.text = values.name;
            _shield.Value = (int)values.ShieldAmount;
            _life.Value = (int)values.ArmorAmount;
            
            // Debug.Log($"Locked tank {_name.text}: Shield {_shield.Value}, Life {_life.Value}, ({values.ShieldAmount}, {values.ArmorAmount})");
        }

        private void Awake()
        {
            _name = transform.FirstOrDefault(t => t.name == "TankName").GetComponent<Text>();
            
            _shield = transform.FirstOrDefault(t => t.name == "Shield").GetComponent<ProgressBar>();
            _shield.MinValue = 0;
            _shield.MaxValue = 100;
            _shield.Value = 100;
            _shield.ShowValue = false;
            
            _life = transform.FirstOrDefault(t => t.name == "Life").GetComponent<ProgressBar>();
            _life.MinValue = 0;
            _life.MaxValue = 100;
            _life.Value = 100;
            _life.ShowValue = false;
        }

        public void SetActive(bool show = true)
        {
            gameObject.SetActive(show);
        }
        
    }
}