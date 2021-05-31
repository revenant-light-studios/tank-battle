using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation
{
    public class CreditsManager : MonoBehaviour
    {
        private NavigationsButtons _navBtns;
        private DisplayCredits _creditsMove;

        //Navigation
        public delegate void OnGoMenuDelegate();
        public OnGoMenuDelegate OnGoMenu;

        public delegate void OnGoSettingsDelegate();
        public OnGoSettingsDelegate OnGoSettings;

        private void Awake()
        {
            _creditsMove = transform.FirstOrDefault(t => t.name == "DisplayCredits").GetComponent<DisplayCredits>();
            _navBtns = transform.FirstOrDefault(t => t.name == "NavigationBtns").GetComponent<NavigationsButtons>();

            _navBtns.OnMenu += () => OnGoMenu?.Invoke();
            _navBtns.OnSettings += () => OnGoSettings?.Invoke();
        }
        // Start is called before the first frame update
        void Start()
        {
            _navBtns.SelectNavButton(NavigationsButtons.navWindows.Credits);
        }
        private void OnEnable()
        {
            // Debug.Log("Start Credits");
            _creditsMove?.StartCredits();
        }

        private void OnDisable()
        {
            // Debug.Log("Finish Credits");
            _creditsMove?.RestartPositions();
        }
    }
}