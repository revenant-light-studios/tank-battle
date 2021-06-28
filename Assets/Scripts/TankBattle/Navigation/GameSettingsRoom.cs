using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation
{
    public class GameSettingsRoom : MonoBehaviour
    {
        private int MAX_NUM_PLAYERS = 20;
        private int MIN_NUM_PLAYERS = 2;
        private int MAX_NUM_DUMMIES = 10;

        private int _numDummies = 0;
        private int _numPlayers = 2;

        private Text _numPlayersText;
        private Text _numDummiesText;
        private Button _sumPlayers;
        private Button _subtractPlayers;
        private Button _sumDummies;
        private Button _subtractDummies;
        private Toggle _secundaryGunsToggle;

        private void Awake()
        {
            _numPlayersText = transform.FirstOrDefault(t => t.name == "PlayersNumPanel").GetComponentInChildren<Text>();
            _numPlayersText.text = $"{_numPlayers}";
            _sumPlayers = transform.FirstOrDefault(t => t.name == "PlusPlayersNumButton").GetComponent<Button>();
            _subtractPlayers = transform.FirstOrDefault(t => t.name == "SubtractPlayersNumButton").GetComponent<Button>();

            _numDummiesText = transform.FirstOrDefault(t => t.name == "DummiesNumPanel").GetComponentInChildren<Text>();
            _numDummiesText.text = $"{_numDummies}";
            _sumDummies = transform.FirstOrDefault(t => t.name == "PlusDummiesButton").GetComponent<Button>();
            _subtractDummies = transform.FirstOrDefault(t => t.name == "SubtractDummiesButton").GetComponent<Button>();

            _secundaryGunsToggle = transform.FirstOrDefault(t => t.name == "SecundaryGuns").GetComponentInChildren<Toggle>();

            _sumPlayers.onClick.AddListener(() => UpdateNumPlayers(_numPlayers + 1));
            _subtractPlayers.onClick.AddListener(() => UpdateNumPlayers(_numPlayers - 1));
            _sumDummies.onClick.AddListener(() => UpdateNumDummies(_numDummies + 1));
            _subtractDummies.onClick.AddListener(() => UpdateNumDummies(_numDummies - 1));
            _secundaryGunsToggle.onValueChanged.AddListener(UpdateSecundaryGuns);

            SetSettingsVisible(false);

        }

        private void UpdateNumPlayers(int numPlayers)
        {
            var num = Mathf.Clamp(numPlayers, MIN_NUM_PLAYERS,MAX_NUM_PLAYERS);
            _numPlayers = num;
            _numPlayersText.text = $"{num}";
        }

        private void UpdateNumDummies(int numDummies)
        {
            var num = Mathf.Clamp(numDummies, 0, MAX_NUM_DUMMIES);
            _numDummies = num;
            _numDummiesText.text = $"{num}";
        }

        private void UpdateSecundaryGuns(bool withSecundaryGuns)
        {
            Debug.Log(withSecundaryGuns);
            //Cambiar la opción del servidor con el valor del toggle
        }

        public void SetSettingsVisible(bool active)
        {
            _sumDummies.gameObject.SetActive(active);
            _sumPlayers.gameObject.SetActive(active);
            _subtractDummies.gameObject.SetActive(active);
            _subtractPlayers.gameObject.SetActive(active);

            _secundaryGunsToggle.interactable = active;
        }

    }
}
