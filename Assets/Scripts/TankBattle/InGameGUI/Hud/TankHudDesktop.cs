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
                }
                else
                {
                    _secondaryWeaponIcon.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// For this method to be called base.OnTankWeaponEnabled must have been called,
        /// or the event registered explicitly
        /// </summary>
        /// <param name="numberOfBullets"></param>
        protected override void NumberOfBulletsChange(int numberOfBullets)
        {
            _secondaryWeaponIcon.Text = $"{numberOfBullets}";
        }
    }
}