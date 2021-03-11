using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExtensionMethods;
using TankBattle.Navigation;

namespace HightTide.UI.Mobile
{
    public class RoomListManagerMobile : RoomListManager
    {
        private Button _openJoinPanelBtn;
        private Button _cancelBtn;
        private Transform _joinPanel;

        private void Start()
        {
            _openJoinPanelBtn = transform.FirstOrDefault(t => t.name == "OpenJoinPanelBtn").GetComponent<Button>();
            _cancelBtn = transform.FirstOrDefault(t => t.name == "CancelBtn").GetComponent<Button>();
            _joinPanel = transform.FirstOrDefault(t => t.name == "JoinPanel").GetComponent<Transform>();

            _openJoinPanelBtn.onClick.AddListener(ShowJoinPanel);
            _cancelBtn.onClick.AddListener(HideJoinPanel);
            _joinPanel.gameObject.SetActive(false);
        }

        private void ShowJoinPanel()
        {
            _joinPanel.gameObject.SetActive(true);
            _returnButton.interactable = false;
            _createRoomButton.interactable = false;
            _openJoinPanelBtn.interactable = false;
        }

        private void HideJoinPanel()
        {
            _joinPanel.gameObject.SetActive(false);
            _returnButton.interactable = true;
            _createRoomButton.interactable = true;
            _openJoinPanelBtn.interactable = true;
        }

        public override void OnDisable()
        {
            base.OnEnable();
            _joinPanel.gameObject.SetActive(false);
        }
    }
}

