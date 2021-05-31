using ExtensionMethods;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation
{
    public class NavigationsButtons : MonoBehaviour
    {
        public enum navWindows
        {
            Menu,
            Settings,
            Credits
        };

        private Button _creditsBtn;
        private Button _menuBtn;
        private Button _settingsBtn;

        public delegate void OnCreditsDelegate();
        public OnCreditsDelegate OnCredits;

        public delegate void OnMenuDelegate();
        public OnMenuDelegate OnMenu;

        public delegate void OnSettingsDelegate();
        public OnSettingsDelegate OnSettings;

        private void Awake()
        {
            _creditsBtn = transform.FirstOrDefault(t => t.name == "CreditsBtn").GetComponent<Button>();
            _menuBtn = transform.FirstOrDefault(t => t.name == "MenuBtn").GetComponent<Button>();
            _settingsBtn = transform.FirstOrDefault(t => t.name == "SettingsBtn").GetComponent<Button>();

            _creditsBtn.onClick.AddListener(OpenCredits);
            _menuBtn.onClick.AddListener(OpenMenu);
            _settingsBtn.onClick.AddListener(OpenSettings);
        }

        public void SelectNavButton (navWindows window)
        {
            switch (window)
            {
                case navWindows.Menu:
                    _menuBtn.transform.localScale = new Vector2(1.2f, 1.2f);
                    _settingsBtn.transform.localScale = Vector2.one;
                    _creditsBtn.transform.localScale = Vector2.one;
                    break;

                case navWindows.Credits:
                    _creditsBtn.transform.localScale = new Vector2(1.2f, 1.2f);
                    _settingsBtn.transform.localScale = Vector2.one;
                    _menuBtn.transform.localScale = Vector2.one;
                    break;

                case navWindows.Settings:
                    _settingsBtn.transform.localScale = new Vector2(1.2f, 1.2f);
                    _menuBtn.transform.localScale = Vector2.one;
                    _creditsBtn.transform.localScale = Vector2.one;
                    break;
            }
        }


        void OpenCredits()
        {
            // Debug.Log("Credits");
            OnCredits?.Invoke();
        }

        void OpenMenu()
        {
            // Debug.Log("Menu");
            OnMenu?.Invoke();
        }

        void OpenSettings()
        {
            // Debug.Log("Settings");
            OnSettings?.Invoke();
        }
    }
}