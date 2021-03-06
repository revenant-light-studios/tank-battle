using System;
using UnityEngine;

namespace TankBattle.Tanks.Engines
{
    public abstract class ATankEngine : MonoBehaviour
    {
        public abstract float InputHorizontalAxis { set; }
        public abstract float InputVerticalAxis { set; }
        public abstract void UpdateTank();

        protected virtual void FixedUpdate()
        {
            UpdateTank();
        }
    }
}