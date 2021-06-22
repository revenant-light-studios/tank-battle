using System;
using UnityEngine;

namespace TankBattle.InGameGUI.LockedTank
{
    public class LockedTank : MonoBehaviour
    {
        public GameObject TrackerTank;
        public GameObject TrackedTank;
        public float distance;
        public float height;

        private void Update()
        {
            if (TrackedTank && TrackerTank)
            {
                Vector3 _targetPosition = TrackedTank.transform.position;
                Vector3 _trackerPosition = TrackerTank.transform.position;
                Vector3 direction = (_targetPosition - _trackerPosition).normalized;
                Vector3 yPos = Vector3.up * height;
                
                transform.position = _targetPosition + (direction * distance) + yPos;
                transform.LookAt(_targetPosition + yPos);
            }
        }
    }
}