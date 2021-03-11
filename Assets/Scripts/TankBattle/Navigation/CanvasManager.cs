using ExtensionMethods;
using Photon.Pun;
using UnityEngine;


namespace TankBattle.Navigation2
{
    [RequireComponent(typeof(PhotonView))]
    public class CanvasManager : MonoBehaviour
    {
        enum navScreen
        {
            MainMenu,
            Credits,
            Settings
        }
        private bool _isDesktop = true;

        private CreditsManager _credits;
        private MainMenuManager _mainMenu;
        private SettingsManager _settings;

        private GameObject _desktop;
        private GameObject _mobile;

        private void Awake()
        {
            _desktop = transform.FirstOrDefault(t => t.name == "Desktop").gameObject;
            _mobile = transform.FirstOrDefault(t => t.name == "Mobile").gameObject;

            if (_isDesktop)
            {
                _mainMenu = _desktop.transform.FirstOrDefault(t => t.name == "MainMenu").GetComponent<MainMenuManager>();
                _credits = _desktop.transform.FirstOrDefault(t => t.name == "Credits").GetComponent<CreditsManager>();
                _settings = _desktop.transform.FirstOrDefault(t => t.name == "Settings").GetComponent<SettingsManager>();
            }

            _credits.OnGoMenu += () => Navigate(navScreen.MainMenu);
            _credits.OnGoSettings += () => Navigate(navScreen.Settings);
            _settings.OnGoCredits += () => Navigate(navScreen.Credits);
            _settings.OnGoMenu += () => Navigate(navScreen.MainMenu);
            _mainMenu.OnGoCredits += () => Navigate(navScreen.Credits);
            _mainMenu.OnGoSettings += () => Navigate(navScreen.Settings);
        }

        void Start()
        {
            _credits.gameObject.SetActive(false);
            _settings.gameObject.SetActive(false);
            _mainMenu.gameObject.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void Navigate(navScreen next)
        {
            HideAllCanvas();
            switch (next)
            {
                case navScreen.MainMenu:
                    _mainMenu.gameObject.SetActive(true);
                    break;

                case navScreen.Credits:
                    _credits.gameObject.SetActive(true);
                    break;

                case navScreen.Settings:
                    _settings.gameObject.SetActive(true);
                    break;
            }
        }

        void HideAllCanvas()
        {
            _mainMenu.gameObject.SetActive(false);
            _credits.gameObject.SetActive(false);
            _settings.gameObject.SetActive(false);
        }
    }
}