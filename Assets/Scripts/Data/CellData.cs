using UnityEngine;

namespace RollerSplat.Data
{
    [CreateAssetMenu(fileName = "Cell Data", menuName = "Roller Splat/Data/Cell Data", order = 1)]
    public class CellData : ScriptableObject
    {
        public LevelData.CellType type;
        public GameObject prefab;
    }
}