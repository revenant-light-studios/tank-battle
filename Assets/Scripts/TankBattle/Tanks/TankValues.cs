using System;
using ExtensionMethods;
using Networking.Utilities;
using Photon.Pun;
using TankBattle.Tanks.Bullets;
using TankBattle.Tanks.ForceFields;
using TankBattle.Tanks.Guns;
using UnityEditor;
using UnityEngine;

namespace TankBattle.Tanks
{
    public class TankValues : MonoBehaviour
    {
        public float TotalShield;
        public float TotalArmor;
            
        private float _shieldAmount;
        public float ShieldAmount { get => _shieldAmount; }
        
        private float _armorAmount;
        public float ArmorAmount { get => _armorAmount; }
        
        private float _totalHits;
        public float TotalHits { get => _totalHits; }

        private bool _isDead;
        public bool IsDead { get => _isDead; }

        public delegate void OnValuesChangedDelegate(TankValues values);
        public event OnValuesChangedDelegate OnValuesChanged;

        public delegate void OnTankWasHitDelegate(TankValues values);
        public event OnTankWasHitDelegate OnTankWasHit;

        public delegate void OnTankWasDestroyedDelegate(TankValues values);
        public event OnTankWasDestroyedDelegate OnTankWasDestroyed;

        private TankManager _tankManager;
        private PhotonView _photonView;
        public ForceField ForceField;

        private void Awake()
        {
            _tankManager = GetComponent<TankManager>();
            if (_tankManager)
            {
                _tankManager.OnTankWeaponEnabled += (gun, weapon) =>
                {
                    if (gun != null)
                    {
                        gun.OnTankHit = OnBulletHit;
                        gun.OnBulletFired += OnBulletFired;
                    }
                };
            }
        }

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
            _shieldAmount = TotalShield;
            _armorAmount = TotalArmor;
            _isDead = false;
        }

        private float _totalBulletsFired = 0f;
        
        private void OnBulletFired()
        {
            if (_tankManager.IsMine)
            {
                _totalBulletsFired += 1;
                _photonView.Owner.SetStat(PlayerExtensions.TotalBulletsFired, _totalBulletsFired);
            }
        }
        
        private float _enemiesKilled = 0f;

        private void OnBulletHit(TankValues otherValues, float damage)
        {
                // other is a tank
                otherValues.WasHit(damage);

                if(_tankManager.IsMine)
                {
                    _totalHits += 1;
                    _photonView.Owner.SetStat(PlayerExtensions.TotalHits, _totalHits);
                    
                    if (otherValues.IsDead)
                    {
                        _enemiesKilled += 1;
                        _photonView.Owner.SetStat(PlayerExtensions.EnemiesKilled, _enemiesKilled);    
                    }
                }
        }

        public void WasHit(float damage)
        {
            // This only happens for me
            if (_shieldAmount > 0f)
            {
                ForceField.ForceFieldHit();
                _shieldAmount -= TotalShield * damage;

                if (_shieldAmount <= 0f)
                {
                    ForceField.gameObject.SetActive(false);
                }
            }
            else if(_armorAmount > 0f)
            {
                _armorAmount -= TotalArmor * damage;
                OnTankWasHit?.Invoke(this);
            }
            else
            {
                _isDead = true;
                if (_tankManager.IsMine)
                {
                    _photonView.Owner.SetAlive(false);
                    _photonView.RPC("DestroyTank", RpcTarget.All);
                }
                else
                {
                    DestroyTank();    
                }
            }
            
            // Debug.LogFormat("Shield: {0}, Armor: {1}", _shieldAmount, _armorAmount);
            OnValuesChanged?.Invoke(this);
        }
        
        [PunRPC]
        private void DestroyTank()
        {
            ParticleSystem explosion = transform.FirstOrDefault(t => t.name == "Explosion")?.GetComponent<ParticleSystem>();
            if (explosion)
            {
                _tankManager.DeactivateTank();
                explosion.Play();
                OnTankWasDestroyed?.Invoke(this);
                // Destroy(gameObject, explosion.main.duration);
            }
        }
    }
    
    #region Inspector Editor
    #if UNITY_EDITOR
    [CustomEditor(typeof(TankValues))]
    public class TankValuesEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TankValues tankValues = (TankValues)target;
            GUIStyle style = new GUIStyle(GUI.skin.textField);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Shield", $"{tankValues.ShieldAmount}", EditorStyles.textField);
            EditorGUILayout.LabelField("Armor", $"{tankValues.ArmorAmount}", style);
            GUILayout.EndVertical();
            
            if (GUILayout.Button("Take hit"))
            {
                TankValues values = (TankValues)target;
                values.WasHit(0.5f);
            }
        }
    }
    #endif
    #endregion
}