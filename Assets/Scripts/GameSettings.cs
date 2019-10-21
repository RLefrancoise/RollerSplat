using NaughtyAttributes;
using UnityEngine;

namespace RollerSplat
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Roller Splat/Game Settings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [BoxGroup("Colors")] public Color defaultGroundColor;
        [BoxGroup("Colors")] public Color defaultWallColor;

        [BoxGroup("Player")] public float playerSpeed = 0.25f;
    }
}