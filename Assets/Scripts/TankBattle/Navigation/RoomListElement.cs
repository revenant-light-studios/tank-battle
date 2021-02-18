using System;
using ExtensionMethods;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation
{
    public class RoomListElement : MonoBehaviour
    {
        private Text _roomNameText;
        private Text _numberOfPlayersText;
        private Image _numberOfPlayersIcon;
        private Image _availableIcon;

        public string RoomName
        {
            get => _roomNameText.text;
            set => _roomNameText.text = value;
        }

        public String NumberOfPlayers
        {
            get => _numberOfPlayersText.text;
            set => _numberOfPlayersText.text = value;
        }

        private void Awake()
        {
            _roomNameText = transform.FirstOrDefault(t => t.name == "RoomName").GetComponent<Text>();
            _numberOfPlayersText = transform.FirstOrDefault(t => t.name == "NumberOfPlayers").GetComponent<Text>();
            _numberOfPlayersIcon = transform.FirstOrDefault(t => t.name == "NumberOfPlayers").GetComponent<Image>();
            _availableIcon = transform.FirstOrDefault(t => t.name == "AvailableIcon").GetComponent<Image>();
        }
    
    
    }
}
