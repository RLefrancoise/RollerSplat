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
        [BoxGroup("Player")]
        [Header("Movement")]
        [MinValue(0.01f)]
        public float playerSpeed = 0.25f;
        /// <summary>
        /// Player is using trail ?
        /// </summary>
        [BoxGroup("Player")]
        [Header("Trail")]
        public bool playerTrail = true;
        /// <summary>
        /// Player trail start width
        /// </summary>
        [BoxGroup("Player")]
        [Range(0f, 1f)]
        public float playerTrailStartWidth = 1f;
        /// <summary>
        /// Player trail end width
        /// </summary>
        [BoxGroup("Player")]
        [Range(0f, 1f)]
        public float playerTrailEndWidth = 1f;
        /// <summary>
        /// Player start start alpha
        /// </summary>
        [BoxGroup("Player")]
        [Range(0f, 1f)]
        public float playerTrailStartAlpha = 1f;
        /// <summary>
        /// Player trail end alpha
        /// </summary>
        [BoxGroup("Player")]
        [Range(0f, 1f)]
        public float playerTrailEndAlpha = 0f;
        /// <summary>
        /// Player brake duration
        /// </summary>
        [BoxGroup("Player")]
        [Header("Brake")]
        [MinValue(0.01f)]
        public float playerBrakeDuration = 0.25f;
        /// <summary>
        /// Player brake vibrato
        /// </summary>
        [BoxGroup("Player")]
        [Range(0, 10)]
        public int playerBrakeVibrato = 5;
        /// <summary>
        /// Player brake elasticity
        /// </summary>
        [BoxGroup("Player")]
        [Range(0f, 1f)]
        public float playerBrakeElasticity = 1f;
        #endregion

        #region Level

        /// <summary>
        /// Level block size
        /// </summary>
        [BoxGroup("Level")] public float blockSize = 1f;
        /// <summary>
        /// Ground coloration duration
        /// </summary>
        [BoxGroup("Level")]
        [MinValue(0f)]
        public float groundColorationDuration = 0.5f;
        
        #endregion

        #region HUD

        /// <summary>
        /// Duration of move number gauge fill
        /// </summary>
        [BoxGroup("HUD")]
        [MinValue(0f)]
        public float moveNumberGaugeFillDuration = 0.5f;

        #endregion
    }
}