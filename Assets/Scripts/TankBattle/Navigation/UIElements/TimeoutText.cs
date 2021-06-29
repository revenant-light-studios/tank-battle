using System;
using ExitGames.Client.Photon;
using Networking.Utilities;
using Photon.Pun;
using TankBattle.Global;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation.UIElements
{
    public class TimeoutText : MonoBehaviourPunCallbacks
    {
        private Text _text;
        private double _timerTimeToStart;
        private double _timerStartTime;
        private bool _timerStarted;

        public delegate void OnTimerFinishedDelegate();
        public event OnTimerFinishedDelegate OnTimerFinished;

        private float _fadeTimer;

        public bool TimerStarted
        {
            get => _timerStarted;
        }

        private void Awake()
        {
            _text = GetComponent<Text>();
            _text.enabled = false;

            _timerTimeToStart = 0d;
            _timerStartTime = 0d;
            _timerStarted = false;
        }
        
        public override void OnEnable()
        {
            base.OnEnable();
            
            // Check if countdown already started when I join
            if (!PhotonNetwork.IsMasterClient)
            {
                // Debug.Log($"Checking if room countdown already started");
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(RoomOptionsKeys.TimerStarted, out object timerStarted))
                {
                    _timerStarted = (bool)timerStarted;
                    _timerTimeToStart = (double)PhotonNetwork.CurrentRoom.CustomProperties[RoomOptionsKeys.TimeToStart];
                    _timerStartTime = (double)PhotonNetwork.CurrentRoom.CustomProperties[RoomOptionsKeys.StartTime];
                    _text.enabled = _timerStarted;
                    // Debug.Log($"Yes! Already started {PhotonNetwork.Time - _timerStartTime:0} seconds ago");
                }
            }
        }

        private void Update()
        {
            if (_timerStarted)
            {
                double delta = PhotonNetwork.Time - _timerStartTime;
                double timeToStart = _timerTimeToStart - delta;
                SetSecondsText(timeToStart);
                
                if (timeToStart <= 0d)
                {
                    _timerStarted = false;
                    OnTimerFinished?.Invoke();
                }
            }
        }


        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.TryGetValue(RoomOptionsKeys.TimeToStart, out object timeToStart))
            {
                _timerTimeToStart = (double)timeToStart;
                SetSecondsText(_timerTimeToStart);
            }
            
            if (propertiesThatChanged.TryGetValue(RoomOptionsKeys.StartTime, out object timerStart))
            {
                _timerStartTime = (double)timerStart;
            }

            if (propertiesThatChanged.TryGetValue(RoomOptionsKeys.TimerStarted, out object timerStarted))
            {
                _timerStarted = (bool)timerStarted;
                _text.enabled = _timerStarted;
                _fadeTimer = 0f;
            }
        }

        private void SetSecondsText(double seconds)
        {
            double colorValue = Math.Max(0.5f, seconds - Math.Truncate(seconds));
            Color32 color = new Color32(255, 0, 0, (byte)(255 * colorValue));
            string textString = $"VAMOS A EMPEZAR EN\n<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{seconds:0}</color>";
            //Debug.Log(textString);
            _text.text = textString;
            _text.SetAllDirty();
        }

    }
}