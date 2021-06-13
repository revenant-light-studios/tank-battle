using System;
using ExtensionMethods;
using Photon.Pun;
using TankBattle.Tanks.Bullets;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks.Guns
{
    public abstract class ATankGun : MonoBehaviour, IPunInstantiateMagicCallback
    {
        
        [SerializeField, FormerlySerializedAs("FiringRate"), InspectorName("Fire rate"), Tooltip("Seconds between consecutive shots")] 
        protected float _firingRate = 2f;

        protected float LastFired;
        protected bool CanFire;

        protected virtual void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }

        protected virtual void Update()
        {
            LastFired += Time.deltaTime;
            
            if(!CanFire && LastFired >= _firingRate)
            {
                CanFire = true;
            }
        }
        
        protected PhotonView _photonView;
        
        public virtual void Fire()
        {
            if (!CanFire) return;
            
            if(PhotonNetwork.IsConnected && _photonView.IsMine)
            {
                _photonView.RPC("NetworkFire", RpcTarget.All);
            }
            else
            {
                NetworkFire();
            }

            LastFired = 0.0f;
            CanFire = false;
        }

        [SerializeField] public ATankBullet TankBullet;
        
        [PunRPC]
        public abstract void NetworkFire();

        public delegate void OnEnergyUpdateDelegate(float currentEnergy, float minimumEnergy);
        public OnEnergyUpdateDelegate OnEnergyUpdate;

        public delegate void OnTankHitDelegate(TankValues other, ATankBullet bullet);
        public OnTankHitDelegate OnTankHit;
        
        protected void OnBulletHit(GameObject other)
        {
            TankValues tankValues = other.GetComponent<TankValues>();
            if (tankValues != null)
            {
                // Debug.Log($"Hit {other.name}");
                if (OnTankHit != null)
                {
                    OnTankHit?.Invoke(tankValues, TankBullet);    
                }
                else
                {
                    tankValues.WasHit(TankBullet);
                }
            }
        }
        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            // Read instantiation data
            object[] instantiationData = info.photonView.InstantiationData;
            int viewId = (int)instantiationData[0];
            TankManager.TankWeapon weaponType = (TankManager.TankWeapon)instantiationData[1];
            
            // Parent tank view
            PhotonView parentView = PhotonNetwork.GetPhotonView(viewId);
            TankManager tankManager = parentView.GetComponent<TankManager>();

            if (weaponType == TankManager.TankWeapon.Primary)
            {
                tankManager.PrimaryGun = this;
            }
            else
            {
                tankManager.SecondaryGun = this;
            }
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ATankGun), true)]
    public class ATankGunEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Fire"))
            {
                ATankGun missile = (ATankGun)target;
                missile.Fire();
            }
        }
    }
#endif
}