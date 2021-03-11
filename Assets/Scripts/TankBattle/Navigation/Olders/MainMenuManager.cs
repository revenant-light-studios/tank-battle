using ExtensionMethods;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation
{
    public class MainMenuManager : MonoBehaviour
    {
        private Button _playBtn;
        private Button _settingsBtn;
        private Button _creditsBtn;

        public delegate void OnPlayDelegate();
        public OnPlayDelegate OnPlay;

        public delegate void OnSettingDelegate();
        public OnSettingDelegate OnSettings;

        public delegate void OnCreditsDelegate();
        public OnCreditsDelegate OnCredits;

        private void Awake()
        {
            _playBtn = transform.FirstOrDefault(t => t.name == "PlayBtn").GetComponent<Button>();
            _settingsBtn = transform.FirstOrDefault(t => t.name == "SettingsBtn").GetComponent<Button>();
            _creditsBtn = transform.FirstOrDefault(t => t.name == "CreditsBtn").GetComponent<Button>();

            _playBtn.onClick.AddListener(OpenPlay);
            _settingsBtn.onClick.AddListener(OpenSettings);
            _creditsBtn.onClick.AddListener(OpenCredits);
        }

        private void OpenCredits()
        {
            OnCredits?.Invoke();
        }

        private void OpenSettings()
        {
            OnSettings?.Invoke();
        }

        private void OpenPlay()
        {
            OnPlay?.Invoke();
        }
    }
}
