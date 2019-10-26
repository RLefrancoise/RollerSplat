namespace RollerSplat
{
    public interface IHapticManager
    {
        bool VibrationEnabled { get; set; }
        void Vibrate();
    }
}