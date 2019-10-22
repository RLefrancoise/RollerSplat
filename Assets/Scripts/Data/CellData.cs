using UnityEngine;

namespace RollerSplat.Data
{
    /// <summary>
    /// Level cell data
    /// </summary>
    [CreateAssetMenu(fileName = "Cell Data", menuName = "Roller Splat/Data/Cell Data", order = 1)]
    public class CellData : ScriptableObject
    {
        /// <summary>
        /// Type of the cell
        /// </summary>
        public LevelData.CellType type;
        /// <summary>
        /// Prefab of the cell
        /// </summary>
        public GameObject prefab;
    }
}