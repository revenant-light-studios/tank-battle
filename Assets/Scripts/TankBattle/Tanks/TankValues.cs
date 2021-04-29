using System;
using ExtensionMethods;
using Photon.Pun;
using TankBattle.Tanks.Bullets;
using TankBattle.Tanks.Guns;
using TMPro;
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

        public delegate void OnValuesChangedDelegate(TankValues values);
        public OnValuesChangedDelegate OnValuesChanged;

        public delegate void OnTankWasHitDelegate(TankValues values);
        public OnTankWasHitDelegate OnTankWasHit;

        private PhotonView _photonView;

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
            
            _shieldAmount = TotalShield;
            _armorAmount = TotalArmor;

            ATankGun _gun = GetComponent<ATankGun>();
            if(_gun) _gun.OnTankHit = OnBulletHit;
        }
        private void OnBulletHit(TankValues otherValues)
        {
                // other is a tank
                otherValues.WasHit();
                HitOther();
                
                Debug.LogFormat("{0} hit {4} tank. {0} hits: {1}, {4} shield: {2}, {4} armor: {3}", 
                    name, _totalHits, otherValues._shieldAmount, otherValues._armorAmount, otherValues.transform.name);
        }

        public void WasHit()
        {
            // This only happens for me
            if (!_photonView.IsMine && PhotonNetwork.IsConnected) return;
            
            if (_shieldAmount > 0f)
            {
                _shieldAmount -= TotalShield * .1f;
            }
            else if(_armorAmount > 0f)
            {
                _armorAmount -= TotalArmor * 0.05f;    
            }
            else
            {
                if (PhotonNetwork.IsConnected && _photonView.IsMine)
                {
                    _photonView.RPC("DestroyTank", RpcTarget.All);
                }
                else
                {
                    DestroyTank();    
                }
                
            }
            
            Debug.LogFormat("Shield: {0}, Armor: {1}", _shieldAmount, _armorAmount);
            
            OnTankWasHit?.Invoke(this);
            OnValuesChanged?.Invoke(this);
        }

        private void HitOther()
        {
            if (!_photonView.IsMine && PhotonNetwork.IsConnected) return;
            
            _totalHits += 1;
        }

        [PunRPC]
        private void DestroyTank()
        {
            ParticleSystem explosion = transform.FirstOrDefault(t => t.name == "Explosion")?.GetComponent<ParticleSystem>();
            if (explosion)
            {
                transform.FirstOrDefault(t => t.name == "Turret")?.gameObject.SetActive(false);
                transform.FirstOrDefault(t => t.name == "Body")?.gameObject.SetActive(false);
                transform.FirstOrDefault(t => t.name == "ExtraFuelTank")?.gameObject.SetActive(false);
                transform.FirstOrDefault(t => t.name == "MissileThrower")?.gameObject.SetActive(false);
                explosion.Play();
                Destroy(gameObject, explosion.main.duration);
            }
        }
    }
}