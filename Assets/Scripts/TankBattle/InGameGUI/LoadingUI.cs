using ExitGames.Client.Photon;
using ExtensionMethods;
using Photon.Pun;
using Photon.Realtime;
using TankBattle.Navigation;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.InGameGUI
{
    public class LoadingUI : MonoBehaviour, IOnEventCallback
    {
        private Text _text;
        private ProgressBar _progressBar;

        private void Awake()
        {
            _text = transform.FirstOrDefault(t => t.name == "MainText").GetComponent<Text>();
            _progressBar = transform.FirstOrDefault(t => t.name == "ProgressBar").GetComponent<ProgressBar>();
        }

        public string Text
        {
            get => _text.text;
            set => _text.text = value;
        }

        public float Progress
        {
            get => _progressBar.Value;
            set => _progressBar.Value = value;
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
        
        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;

            if (eventCode == PlayRoomManager.LoadingEvent)
            {
                object[] data = (object[])photonEvent.CustomData;
                int progress = (int)data[0];
                string message = (string)data[1];
                ShowProgress(progress, message);
            }
        }

        public void ShowProgress(int progress, string message)
        {
            Progress = progress;
            Text = message;
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}