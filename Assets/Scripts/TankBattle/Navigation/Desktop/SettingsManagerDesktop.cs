using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ExtensionMethods;
using TankBattle.Navigation;

namespace HightTide.UI
{
    public class SettingsManagerDesktop : SettingsManager
    {
        public Button _stats { get; private set; }
        public Button _photo { get; private set; }
        public Button _finishTurn { get; private set; }

        private Event _keyEvent;
        private bool _waitingForKey = false;
        private string _pressedButton;

        private void Start()
        {
            Transform shortcutsTransform = transform.FirstOrDefault(t => t.name == "ShortcutsSettings").transform;
            _stats = shortcutsTransform.FirstOrDefault(t => t.name == "Stats").GetComponentInChildren<Button>();
            _photo = shortcutsTransform.FirstOrDefault(t => t.name == "Photo").GetComponentInChildren<Button>();
            _finishTurn = shortcutsTransform.FirstOrDefault(t => t.name == "FinishTurn").GetComponentInChildren<Button>();

            _stats.GetComponentInChildren<Text>().text = _customSettings.stats.ToString();
            _photo.GetComponentInChildren<Text>().text = _customSettings.photo.ToString();
            _finishTurn.GetComponentInChildren<Text>().text = _customSettings.finishTurn.ToString();

            _stats.onClick.AddListener(KeyChange);
            _photo.onClick.AddListener(KeyChange);
            _finishTurn.onClick.AddListener(KeyChange);
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
                            case "StatsInput":
                                _customSettings.stats = vKey;
                                _stats.GetComponentInChildren<Text>().text = vKey.ToString();
                                break;
                            case "PhotoInput":
                                _customSettings.photo = vKey;
                                _photo.GetComponentInChildren<Text>().text = vKey.ToString();
                                break;
                            case "FinishTurnInput":
                                _customSettings.finishTurn = vKey;
                                _finishTurn.GetComponentInChildren<Text>().text = vKey.ToString();
                                break;
                        }
                        _waitingForKey = false;
                        _photo.interactable = true;
                        _finishTurn.interactable = true;
                        _stats.interactable = true;
                    }
                }
            }
        }

        private void KeyChange()
        {
            _waitingForKey = true;
            _pressedButton = EventSystem.current.currentSelectedGameObject.name;
            _photo.interactable = false;
            _finishTurn.interactable = false;
            _stats.interactable = false;

        }
    }
}
