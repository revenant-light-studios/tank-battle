using ExtensionMethods;
using Photon.Pun;
using TankBattle.Tanks;
using TankBattle.Tanks.Guns;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Items
{
    public class PickableItem : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("RotationSpeed")] 
        private float _rotationSpeed;
        [SerializeField, FormerlySerializedAs("ItemIcon")]
        private Transform _itemIcon;
        [SerializeField, FormerlySerializedAs("ItemPrefab")]
        private GameObject _itemPrefab;

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
            // Debug.Log($"{name}: {other.name} Jumped over me");
            
            TankManager manager = root.GetComponent<TankManager>();
            if (manager)
            {
                ATankGun gun = _itemPrefab.GetComponent<ATankGun>();
                if (gun)
                {
                    manager.PickWeapon(gun.name, TankManager.TankWeapon.Secondary);
                    // Debug.Log($"{other.name} picked up a {_itemPrefab.name}");

                    if (PhotonNetwork.IsConnected)
                    {
                        PhotonNetwork.Destroy(GetComponent<PhotonView>());
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
