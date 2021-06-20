using Photon.Pun;
using TankBattle.Tanks.Bullets;
using TankBattle.Tanks.ForceFields;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks.Guns
{
    public abstract class ATankGun : MonoBehaviour, IPunInstantiateMagicCallback
    {
        [SerializeField, FormerlySerializedAs("NumberOfBullets"), InspectorName("Number of bullets"), Tooltip("Number of bullets, 0 for unlimited")]
        protected int _maxNumberOfBullets = 0;
        
        [SerializeField, FormerlySerializedAs("FiringRate"), InspectorName("Fire rate"), Tooltip("Seconds between consecutive shots")] 
        protected float _firingRate = 2f;

        protected float LastFired;
        protected bool CanFire;
        protected bool _canTrack;
        public bool CanTrack => _canTrack;
        
        protected PhotonView _photonView;

        [SerializeField] public ATankBullet TankBullet;

        private int _currentNumberOfBullets;
        public delegate void OnNumberOfBulletsChangeDelegate(int numberOfBullets);
        public event OnNumberOfBulletsChangeDelegate OnNumberOfBulletsChange;

        public int CurrentNumberOfBullets
        {
            get => _currentNumberOfBullets;
            protected set
            {
                _currentNumberOfBullets = value;
                OnNumberOfBulletsChange?.Invoke(_currentNumberOfBullets);
            }
        }
        
        
        
        public delegate void OnEnergyUpdateDelegate(float currentEnergy, float minimumEnergy);
        public OnEnergyUpdateDelegate OnEnergyUpdate;
        
        protected TankManager _parentTank;
        public TankManager ParentTank
        {
            get => _parentTank;
            set => _parentTank = value;
        }

        protected virtual void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _currentNumberOfBullets = _maxNumberOfBullets;
            CanFire = true;
        }

        protected virtual void Update()
        {
            if(TriggerPressed) Fire();
            
            LastFired += Time.deltaTime;
            
            if(!CanFire && LastFired >= _firingRate)
            {
                CanFire = _maxNumberOfBullets==0 || _currentNumberOfBullets > 0;
            }
        }
        
        /// <summary>
        /// Called when the weapon is instantiated through network
        /// </summary>
        /// <param name="info"></param>
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
        
        #region Fire management
        public virtual void Fire()
        {
            // Firing is authoritative
            if (!_photonView.IsMine && PhotonNetwork.IsConnected) return;
            
            if (!CanFire) return;

            if(PhotonNetwork.IsConnected)
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

        
        [PunRPC]
        public abstract void NetworkFire();
        #endregion
        
        #region Impact management
        public delegate void OnTankHitDelegate(TankValues other, float damage);
        public OnTankHitDelegate OnTankHit;
        
        protected bool OnBulletHit(GameObject other)
        {
            // Impact is authoritative
            if (!_photonView.IsMine && PhotonNetwork.IsConnected) return false;
            
            TankValues tankValues = other.GetComponent<TankValues>();
            
            // If it's a force field then get parent tank values
            if (!tankValues)
            {
                ForceField forceField = other.GetComponent<ForceField>();
                if (forceField)
                {
                    tankValues = forceField.ParentTank.GetComponent<TankValues>();
                }
            }

            // Return true if collides with own object
            // if (tankValues && ParentTank && tankValues.gameObject == ParentTank.gameObject) return true;
            
            if (tankValues)
            {
                if (PhotonNetwork.IsConnected)
                {
                    _photonView.RPC("NetworkHit", RpcTarget.All, tankValues.GetComponent<PhotonView>().ViewID, TankBullet.Damage);
                }
                else
                {
                    DelegateHit(tankValues, TankBullet.Damage);
                }
            }

            return false;
        }

        [PunRPC]
        public void NetworkHit(int viewID, float damage)
        {
            PhotonView targetView = PhotonNetwork.GetPhotonView(viewID);
            DelegateHit(targetView.GetComponent<TankValues>(), damage);
        }

        private void DelegateHit(TankValues tankValues, float damage)
        {
            Debug.Log($"Hit {tankValues.name}");
            if (OnTankHit != null)
            {
                OnTankHit?.Invoke(tankValues, damage);    
            }
            else
            {
                tankValues.WasHit(damage);
            }
        }
        #endregion

        #region Input management
        protected TankInput TankInput;
        protected bool TriggerPressed;

        /// <summary>
        /// Associates this gun to input trigger
        /// </summary>
        /// <param name="input"></param>
        public virtual void RegisterInput(TankInput input)
        {
            TankInput = input;
            TankInput.Trigger1.OnTriggerPressed += () => TriggerPressed = true;
            TankInput.Trigger1.OnTriggerReleased += () => TriggerPressed = false;
        }
        #endregion
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
                ATankGun gun = (ATankGun)target;
                gun.Fire();
            }
        }
    }
#endif
}