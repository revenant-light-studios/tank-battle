using System;
using System.Collections.Generic;
using ExtensionMethods;
using Photon.Pun;
using TankBattle.Tanks;
using TankBattle.Tanks.Guns;
using TankBattle.Tanks.Powerups;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Items
{
    public class PickableItem : MonoBehaviour
    {
        public enum ItemType
        {
            SecondaryWeapon,
            TankPowerup
        }
        
        [SerializeField, FormerlySerializedAs("RotationSpeed")] 
        private float _rotationSpeed;
        [SerializeField, FormerlySerializedAs("ItemIcon")]
        private Transform _itemIcon;
        [SerializeField, FormerlySerializedAs("ItemPrefab")]
        private GameObject _itemPrefab;
        [SerializeField, FormerlySerializedAs("ItemType")]
        private ItemType _itemType;

        private void Start()
        {
            if (!_itemIcon)
            {
                _itemIcon = transform.FirstOrDefault(t => t.name == "Item");
            }
        }

        void Update()
        {
            _itemIcon?.Rotate(Vector3.up * (_rotationSpeed * Time.deltaTime), Space.World);    
        }

        private void OnTriggerEnter(Collider other)
        {
            GameObject root = other.transform.root.gameObject;
            TankManager tankManager = root.GetComponent<TankManager>();
            
            if (tankManager)
            {
                if (_itemType == ItemType.SecondaryWeapon)
                {
                    ApplySecondaryWeapon(tankManager);
                } else if (_itemType == ItemType.TankPowerup)
                {
                    ApplyPowerUp(tankManager);
                }
            }
        }
        private void ApplyPowerUp(TankManager tankManager)
        {
            // powerups are only applied to MY tank
            if (!tankManager.IsMine) return;
            
            GameObject powerUpGO = Instantiate(_itemPrefab);
            APowerUp powerUp = powerUpGO.GetComponent<APowerUp>();

            if (powerUp)
            {
                if (powerUp.ApplyPowerup(tankManager))
                {
                    if (PhotonNetwork.IsConnected)
                    {
                        PhotonView view = GetComponent<PhotonView>();
                        view.RPC("NetworkDestroyObject", RpcTarget.All, view.ViewID);
                    }
                    else
                    {
                        DestroyObject(gameObject);    
                    }
                }
            }
        }
        private void ApplySecondaryWeapon(TankManager tankManager)
        {
            ATankGun gun = _itemPrefab.GetComponent<ATankGun>();
            if (gun)
            {
                tankManager.PickWeapon(gun.name, TankManager.TankWeapon.Secondary);
                // Debug.Log($"{other.name} picked up a {_itemPrefab.name}");

                if (PhotonNetwork.IsConnected)
                {
                    PhotonView view = GetComponent<PhotonView>();
                    view.RPC("NetworkDestroyObject", RpcTarget.All, view.ViewID);
                }
                else
                {
                    DestroyObject(gameObject);
                }
            }
        }

        [PunRPC]
        public void NetworkDestroyObject(int viewId)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            PhotonView view = PhotonNetwork.GetPhotonView(viewId);
            DestroyObject(view.gameObject);
        }

        private void DestroyObject(GameObject gameObject)
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);   
            }
        }
    }
}
