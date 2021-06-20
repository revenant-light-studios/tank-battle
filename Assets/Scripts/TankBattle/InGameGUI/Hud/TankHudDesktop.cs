using ExtensionMethods;
using TankBattle.Tanks;
using TankBattle.Tanks.Guns;

namespace TankBattle.InGameGUI.Hud
{
    public class TankHudDesktop : TankHud
    {
        private SecondaryWeaponIcon _secondaryWeaponIcon;

        protected override void Awake()
        {
            base.Awake();
            
            _secondaryWeaponIcon = transform.FirstOrDefault(t => t.name == "SecondaryWeaponIcon").GetComponent<SecondaryWeaponIcon>();
            _secondaryWeaponIcon.gameObject.SetActive(false);            
        }

        protected override void OnTankWeaponEnabled(ATankGun gun, TankManager.TankWeapon weapon)
        {
            base.OnTankWeaponEnabled(gun, weapon);

            if (weapon == TankManager.TankWeapon.Secondary)
            {
                if (gun != null)
                {
                    _secondaryWeaponIcon.gameObject.SetActive(true);
                    _secondaryWeaponIcon.Icon = gun.Icon;
                    _secondaryWeaponIcon.Text = $"{gun.CurrentNumberOfBullets}";
                    gun.OnNumberOfBulletsChange += bullets => _secondaryWeaponIcon.Text = $"{bullets}";
                }
                else
                {
                    _secondaryWeaponIcon.gameObject.SetActive(false);
                }
            }
        }
    }
}