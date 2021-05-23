using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Navigation
{
    public class SelectGameUI : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("IsDesktop")]
        private bool _isDesktop = false;

        private GameObject _desktop;
        private GameObject _mobile;

        private void Awake()
        {
            _desktop = transform.FirstOrDefault(t => t.name == "Desktop").gameObject;
            _mobile = transform.FirstOrDefault(t => t.name == "Mobile").gameObject;
        }
        // Start is called before the first frame update
        void Start()
        {
            if (_isDesktop)
            {
                _mobile.SetActive(false);
            }
            else
            {
                _desktop.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
