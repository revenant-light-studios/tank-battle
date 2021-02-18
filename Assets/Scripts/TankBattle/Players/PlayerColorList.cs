using System.Collections.Generic;
using UnityEngine;

namespace HightTide.Players
{
    [CreateAssetMenu(fileName = "ColorList", menuName = "TankBattle/ColorList", order = 0)]
    public class PlayerColorList : ScriptableObject
    {
        public List<Color> Colors;
    }
}