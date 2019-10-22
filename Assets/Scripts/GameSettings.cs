using NaughtyAttributes;
using UnityEngine;

namespace RollerSplat
{
    /// <summary>
    /// Game settings
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Roller Splat/Game Settings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        #region Colors
        
        /// <summary>
        /// Default color for ground tiles
        /// </summary>
        [BoxGroup("Colors")] public Color defaultGroundColor;
        /// <summary>
        /// Default color for wall tiles
        /// </summary>
        [BoxGroup("Colors")] public Color defaultWallColor;
        
        #endregion

        #region Player

        /// <summary>
        /// Player speed
        /// </summary>
        [BoxGroup("Player")] public float playerSpeed = 0.25f;
        
        #endregion

        #region Level

        /// <summary>
        /// Level block size
        /// </summary>
        [BoxGroup("Level")] public float blockSize = 1f;
        
        #endregion
    }
}