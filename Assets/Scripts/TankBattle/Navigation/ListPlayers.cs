using System.Collections.Generic;
using ExtensionMethods;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace TankBattle.Navigation
{
    public class ListPlayers : MonoBehaviour
    {
        [SerializeField] private GameObject _playerListRowPrefab;
        [SerializeField] private GameObject _playerListElemPrefab;
        private int _currentRow = 0;
        private RowContent[] _playerListRows = new RowContent[10];
        private Transform _playerList;

        private void Awake()
        {
            _playerList = transform.FirstOrDefault(t => t.name == "Content").transform;
            for (int i = 0; i < _playerListRows.Length; i++)
            {
                GameObject row = Instantiate(_playerListRowPrefab, _playerList);
                _playerListRows[i] = row.GetComponent<RowContent>();
            }

        }

        public void AddPlayerToList(Player player)
        {
            Transform row = _playerListRows[_currentRow].transform;
            GameObject element = Instantiate(_playerListElemPrefab, row);
            element.name = player.UserId;
            _currentRow++;
            if (_currentRow == _playerListRows.Length)
            {
                _currentRow = 0;
            }

            PlayerElem playerElem = element.GetComponent<PlayerElem>();
            playerElem.PlayerName = string.IsNullOrEmpty(player.NickName) ? "- Anon -" : player.NickName;
        }

        public void ChangePropertiesPlayerList(Player player)
        {
            GetPlayerElement(player).GetComponent<PlayerElem>().PlayerName = player.NickName;
        }

        public void RemovePlayerFromList(Player player)
        {
            Transform elementTransform = GetPlayerElement(player);
            if (elementTransform != null)
            {
                Destroy(elementTransform.gameObject);
            }
            _currentRow--;
            if (_currentRow < 0)
            {
                _currentRow = _playerListRows.Length - 1;
            }
        }

        private Transform GetPlayerElement(Player player)
        {
            Transform elementTransform = transform.FirstOrDefault(t => t.name == player.UserId);
            return elementTransform;
        }

        public void InitPlayerList()
        {
            foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
            {
                AddPlayerToList(playerInfo.Value);
            }
        }

        public void ClearPlayerList()
        {
            List<GameObject> elementsToDelete = new List<GameObject>();
            foreach (Transform child in _playerList.transform)
            {
                elementsToDelete.Add(child.gameObject);
            }
            elementsToDelete.ForEach((child) => Destroy(child));
        }
    }
}