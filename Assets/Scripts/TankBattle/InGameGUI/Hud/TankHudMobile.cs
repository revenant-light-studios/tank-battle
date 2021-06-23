using ExtensionMethods;
using TankBattle.InputManagers;
using TankBattle.Tanks;
using TankBattle.Tanks.Guns;

namespace TankBattle.InGameGUI.Hud
{
    public class TankHudMobile : TankHud
    {
        private VirtualJoystick _movementJoystick;
        private VirtualJoystick _aimJoystick;
        private VirtualButton _primaryShootButton;
        private VirtualButton _secondaryShootButton;
        private VirtualButton _lockButton;

        protected override void Awake()
        {
            base.Awake();

            _movementJoystick = transform.FirstOrDefault(t => t.name == "MovementJoystick").GetComponent<VirtualJoystick>();
            _aimJoystick = transform.FirstOrDefault(t => t.name == "AimJoystick").GetComponent<VirtualJoystick>();
            _primaryShootButton = transform.FirstOrDefault(t => t.name == "PrimaryShootButton").GetComponent<VirtualButton>();
            _secondaryShootButton = transform.FirstOrDefault(t => t.name == "SecondaryShootButton").GetComponent<VirtualButton>();
            _secondaryShootButton.gameObject.SetActive(false);
            _lockButton = transform.FirstOrDefault(t => t.name == "LockButton").GetComponent<VirtualButton>();
            _lockButton.gameObject.SetActive(false);
        }

        protected override void OnTankWeaponEnabled(ATankGun gun, TankManager.TankWeapon weapon)
        {
            base.OnTankWeaponEnabled(gun, weapon);

            if (weapon == TankManager.TankWeapon.Secondary)
            {
                if (gun != null)
                {
                    _secondaryShootButton.gameObject.SetActive(true);
                    _lockButton.gameObject.SetActive(gun.CanTrack);
                    _secondaryShootButton.Text = $"{gun.CurrentNumberOfBullets}";
                    // gun.OnNumberOfBulletsChange += bullets => _secondaryShootButton.Text = $"{bullets}";
                }
                else
                {
                    _secondaryShootButton.gameObject.SetActive(false);
                    _lockButton.gameObject.SetActive(false);
                }
            }
        }

        protected override void NumberOfBulletsChange(int numberOfBullets)
        {
            _secondaryShootButton.Text = $"{numberOfBullets}";
        }
    }
}