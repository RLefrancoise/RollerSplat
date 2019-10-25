using NaughtyAttributes;
using UnityEngine;

namespace RollerSplat.Data
{
    /// <summary>
    /// Level data
    /// </summary>
    [CreateAssetMenu(fileName = "Level Data", menuName = "Roller Splat/Data/Level Data", order = 0)]
    public class LevelData : ScriptableObject
    {
        /// <summary>
        /// Type of level cell
        /// </summary>
        public enum CellType
        {
            /// <summary>
            /// Wall
            /// </summary>
            Wall,
            /// <summary>
            /// Ground
            /// </summary>
            Ground,
            /// <summary>
            /// Teleport
            /// </summary>
            Teleport
        }

        #region Camera

        /// <summary>
        /// Position of the level camera
        /// </summary>
        [BoxGroup("Camera")]
        [Tooltip("Position of the level camera")]
        public Vector3 cameraPosition;
        /// <summary>
        /// Rotation of the level camera
        /// </summary>
        [BoxGroup("Camera")]
        [Tooltip("Rotation of the level camera")]
        public Vector3 cameraRotation;

        #endregion

        #region Level

        /// <summary>
        /// Name of the level
        /// </summary>
        [BoxGroup("Level")]
        [Tooltip("Name of the level")]
        public string levelName;
        /// <summary>
        /// Player start position in tile coordinates
        /// </summary>
        [BoxGroup("Level")]
        [Tooltip("Player start position in tile coordinates")]
        public Vector2 startPosition;
        /// <summary>
        /// Number of allowed moves to complete the level
        /// </summary>
        [BoxGroup("Level")]
        [Tooltip("Number of allowed moves to complete the level")]
        public int numberOfMoves;
        /// <summary>
        /// Prefab containing the level structure
        /// </summary>
        [BoxGroup("Level")]
        [Tooltip("Prefab containing the level structure")]
        public GameObject levelPrefab;

        #endregion
    }
}