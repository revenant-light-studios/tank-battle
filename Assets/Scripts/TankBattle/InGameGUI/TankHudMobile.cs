using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using ExtensionMethods;
using Photon.Pun;
using TankBattle.Global;
using TankBattle.Tanks;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.InGameGUI
{
    public class TankHudMobile : TankHud
    {

        private VirtualJoystick _cameraJoystick;
        public VirtualJoystick CameraJoystick
        {
            get { return _cameraJoystick; }
        }

        private VirtualJoystick _movementJoystick;
        public VirtualJoystick MovementJoystick
        {
            get { return _movementJoystick; }
        }

        private void Awake()
        {
            Transform _cameraJoystickTrans = transform.FirstOrDefault(t => t.name == "CameraJoystick").transform;
            _cameraJoystick = _cameraJoystickTrans.FirstOrDefault(t => t.name == "JoystickContainer")?.GetComponent<VirtualJoystick>();
            
            Transform _movementJoystickTrans = transform.FirstOrDefault(t => t.name == "MovementJoystick").transform;
            _movementJoystick = _movementJoystickTrans.FirstOrDefault(t => t.name == "JoystickContainer")?.GetComponent<VirtualJoystick>();
        }

    }
}
