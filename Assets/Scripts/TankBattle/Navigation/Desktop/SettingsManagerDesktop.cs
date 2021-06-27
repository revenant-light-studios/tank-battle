using System;
using ExtensionMethods;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TankBattle.Navigation.Desktop
{
    public class SettingsManagerDesktop : SettingsManager
    {
        /*public Button _shootBtn { get; private set; }

        private bool _waitingForKey = false;
        private string _pressedButton;

        public override void Awake()
        {
            base.Awake();
            Transform controllersTransform = transform.FirstOrDefault(t => t.name == "Controllers").transform;
            _shootBtn = controllersTransform.FirstOrDefault(t => t.name == "ShootBtn").GetComponent<Button>();

            _shootBtn.onClick.AddListener(KeyChange);
        }

        public override void Start()
        {
            base.Start();

            //init values
            _shootBtn.GetComponentInChildren<Text>().text = _customSettings.shootBtn.ToString();
        }

        private void Update()
        {
            if (_waitingForKey)
            {
                foreach (KeyCode vKey in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(vKey))
                    {
                        switch (_pressedButton)
                        {
                            case "ShootBtn":
                                //_customSettings.shootBtn = vKey;
                                _shootBtn.GetComponentInChildren<Text>().text = vKey.ToString();
                                break;
                        }
                        _waitingForKey = false;
                        _shootBtn.interactable = true;
                    }
                }
            }
        }

        private void KeyChange()
        {
            _waitingForKey = true;
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                _pressedButton = EventSystem.current.currentSelectedGameObject.name;
                _shootBtn.interactable = false;
            }
        }
        */
    }
}

