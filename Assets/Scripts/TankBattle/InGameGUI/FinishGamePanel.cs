using System.Collections.Generic;
using ExtensionMethods;
using Networking.Utilities;
using Photon.Pun;
using Photon.Realtime;
using TankBattle.InGameGUI.Hud;
using TankBattle.Navigation;
using TankBattle.Tanks;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.InGameGUI
{
    public class FinishGamePanel : MonoBehaviourPunCallbacks
    {
        class EndGameStats
        {
            public string PlayerName;
            public float TotalHits;
            public float TotalBulletsFired;
            public float TotalDamage;
            public float EnemiesKilled;
            public double DeadTimestamp;
            public bool IsAlive;

            public EndGameStats(string playerName, bool isAlive, float totalHits, float totalBulletsFired, float enemiesKilled,  double deadTimestamp, float totalDamage = 0f)
            {
                PlayerName = playerName;
                IsAlive = isAlive;
                TotalHits = totalHits;
                TotalBulletsFired = totalBulletsFired;
                TotalDamage = totalDamage;
                EnemiesKilled = enemiesKilled;
                DeadTimestamp = deadTimestamp;
            }

            public static int StatsComparer(EndGameStats egs1, EndGameStats egs2)
            {
                if (egs1.IsAlive && egs2.IsAlive)
                {
                    return egs2.DeadTimestamp.CompareTo(egs2.DeadTimestamp);
                } else if (egs1.IsAlive)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }

        private List<EndGameStats> _endGameStatsList = new List<EndGameStats>();
        
        private Text _winnerName;
        private Text _winnerHits;
        private Button _exitRoomBtn;

        private ATankHud _tankHud;
        private TankInput _tankInput;

        private void Awake()
        {
            _winnerName = transform.FirstOrDefault(t => t.name == "WinnerNameText").GetComponent<Text>();
            _winnerHits = transform.FirstOrDefault(t => t.name == "WinnerHitsText").GetComponent<Text>();
            _exitRoomBtn = transform.FirstOrDefault(t => t.name == "ExitBtn").GetComponent<Button>();
            _exitRoomBtn.onClick.AddListener(LeaveRoom);
        }
        
        public void Show()
        {
            gameObject.SetActive(true);

            foreach (KeyValuePair<int,Player> keyValuePair in PhotonNetwork.CurrentRoom.Players)
            {
                Player player = keyValuePair.Value;
                string name = player.NickName;
                player.CustomProperties.TryGetValue(PlayerExtensions.TotalHits, out object totalHits);
                player.CustomProperties.TryGetValue(PlayerExtensions.TotalBulletsFired, out object totalBulltesFired);
                player.CustomProperties.TryGetValue(PlayerExtensions.TotalDamage, out object totalDamage);
                player.CustomProperties.TryGetValue(PlayerExtensions.EnemiesKilled, out object enemiesKilled);
                player.CustomProperties.TryGetValue(PlayerExtensions.PlayerAlive, out object playerAlive);
                player.CustomProperties.TryGetValue(PlayerExtensions.DeadTimestamp, out object deadTimestamp);

                EndGameStats stats = new EndGameStats(
                    name,
                    (bool)playerAlive,
                    totalHits != null ? (float)totalHits : 0f,
                    totalBulltesFired != null ? (float)totalBulltesFired : 0f,
                    enemiesKilled != null ? (float)enemiesKilled : 0f,
                    (bool)playerAlive ? PhotonNetwork.Time : (double)deadTimestamp,
                    totalDamage != null ? (float)totalDamage : 0f);
                
                // Debug.Log($"Adding player {stats.PlayerName} stats:\n" +
                //     $"TotalHits: {stats.TotalHits}\n" +
                //     $"TotalBulletsFired: {stats.TotalBulletsFired}\n" +
                //     $"EnemiesKilled: {stats.EnemiesKilled}\n" +
                //     $"PlayerAlive: {playerAlive}\n" +
                //     $"DeadTimestamp: {stats.DeadTimestamp}");
                
                _endGameStatsList.Add(stats);
            }

            if (_endGameStatsList.Count > 0)
            {
                _endGameStatsList.Sort(EndGameStats.StatsComparer);
                _winnerName.text = _endGameStatsList[0].PlayerName;
                _winnerHits.text = 
                    $"{_endGameStatsList[0].EnemiesKilled} enemigos abatidos\n" + 
                    $"{_endGameStatsList[0].TotalHits} hits\n" +
                    $"{_endGameStatsList[0].TotalBulletsFired} disparos";
            }
        }
        
        private void LeaveRoom()
        {
            PlayRoomManager.Instance.ExitPlay();
        }
    }
}

