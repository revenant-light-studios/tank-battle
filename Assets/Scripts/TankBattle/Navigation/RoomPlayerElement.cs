using ExtensionMethods;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation
{
    public class RoomPlayerElement : MonoBehaviour
    {
        private Text _playerNameText;
        private Image _readyIconImage;

        public string PlayerName
        {
            get => _playerNameText.text;
            set => _playerNameText.text = value;
        }

        public Sprite ReadyIcon
        {
            get => _readyIconImage.sprite;
            set => _readyIconImage.sprite = value;
        }

        private void Awake()
        {
            _playerNameText = transform.FirstOrDefault(t => t.name == "PlayerName").GetComponent<Text>();
            _readyIconImage = transform.FirstOrDefault(t => t.name == "ReadyIcon").GetComponent<Image>();
        }
    }
}
