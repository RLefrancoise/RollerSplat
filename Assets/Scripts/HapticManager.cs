using UnityEngine;

namespace RollerSplat
{
    public class HapticManager : IHapticManager
    {
        public bool VibrationEnabled { get; set; }

        public void Vibrate()
        {
            if(!VibrationEnabled) return;
            Handheld.Vibrate();
        }
    }
}