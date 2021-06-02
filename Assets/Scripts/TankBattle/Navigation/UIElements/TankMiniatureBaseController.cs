using ExtensionMethods;
using UnityEngine;

namespace TankBattle.Navigation.UIElements
{
    public class TankMiniatureBaseController : MonoBehaviour
    {
        [SerializeField] private Transform _dummyTank;
        [SerializeField] private float _rotationSpeed;

        private void Awake()
        {
            if(!_dummyTank) {
                _dummyTank = transform.FirstOrDefault(t => t.name == "DummyTank");
            }
        }

        private void Update()
        {
            _dummyTank.transform.Rotate(Vector3.up * (_rotationSpeed * Time.deltaTime));
        }
    }
}
