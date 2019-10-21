using NaughtyAttributes;
using UnityEngine;

namespace RollerSplat
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "RollerSplat/Game Settings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [BoxGroup("Colors")] public Color defaultGroundColor;
        [BoxGroup("Colors")] public Color defaultWallColor;

        [BoxGroup("Player")] public float playerMoveTime = 0.25f;
    }
}