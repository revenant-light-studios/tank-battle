using ExtensionMethods;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation2
{
    public class PlayerElem : MonoBehaviour
    {
        private Text _playerNameText;
        public string PlayerName
        {
            get => _playerNameText.text;
            set => _playerNameText.text = value;
        }

        private void Awake()
        {
            _playerNameText = transform.FirstOrDefault(t => t.name == "PlayerName").GetComponent<Text>();
        }
    }
}