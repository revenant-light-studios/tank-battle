using ExtensionMethods;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.InGameGUI
{
    public class PauseMenu : MonoBehaviour
    {
        private PhotonView _photonView;
        private Button _exitBtn;
        private Button _resumeBtn;

        public delegate void OnExitDelegate();
        public OnExitDelegate OnExit;

        public delegate void OnResumeDelegate();
        public OnResumeDelegate OnResume;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _exitBtn = transform.FirstOrDefault(t => t.name == "ExitBtn").GetComponent<Button>();
            _resumeBtn = transform.FirstOrDefault(t => t.name == "ResumeBtn").GetComponent<Button>();

            _exitBtn.onClick.AddListener(LeaveRoom);
            _resumeBtn.onClick.AddListener(ResumeGame);
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void ResumeGame()
        {
            
            transform.gameObject.SetActive(false);
        }

        private void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Destroy(_photonView);
        }

    }
}
