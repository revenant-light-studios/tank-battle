using System;
using ExtensionMethods;
using UnityEngine;

namespace TankBattle.Navigation.UIElements
{
    public class TankMiniatureBaseController : MonoBehaviour
    {
        private Transform _dummyTank;
        [SerializeField] private float _rotationSpeed;

        private void Awake()
        {
            _dummyTank = transform.FirstOrDefault(t => t.name == "DummyTank");
        }

        private void Update()
        {
            _dummyTank.transform.Rotate(Vector3.up * (_rotationSpeed * Time.deltaTime));
        }
    }
}
