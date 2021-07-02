using ExtensionMethods;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.InGameGUI
{
    public class PlayerStats : MonoBehaviour
    {
        private Text _playerName;
        private Text _statsText;

        public string PlayerName
        {
            get => _playerName != null ? _playerName.text : "";
            set
            {
                if (_playerName == null)
                {
                    _playerName = transform.FirstOrDefault(t => t.name == "PlayerNameText").GetComponent<Text>();
                }
                _playerName.text = value;
            }
        }

        public string StatsText
        {
            get => _statsText != null ? _statsText.text : "";
            set
            {
                if (_statsText == null)
                {
                    _statsText = transform.FirstOrDefault(t => t.name == "PlayerStatsText").GetComponent<Text>();
                }
                _statsText.text = value;
            }
        }

        public void SetPlayerStats(FinishGamePanel.EndGameStats stats)
        {
            PlayerName = stats.PlayerName;
            StatsText = $"{stats.EnemiesKilled} enemigos abatidos\n" + 
                $"{stats.TotalHits} hits\n" +
                $"{stats.TotalBulletsFired} disparos";
        }

        private void Awake()
        {
            _playerName = transform.FirstOrDefault(t => t.name == "PlayerNameText").GetComponent<Text>();
            _statsText = transform.FirstOrDefault(t => t.name == "PlayerStatsText").GetComponent<Text>();
        }
    }
}