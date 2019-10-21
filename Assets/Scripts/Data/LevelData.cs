using NaughtyAttributes;
using UnityEngine;

namespace RollerSplat.Data
{
    [CreateAssetMenu(fileName = "Level Data", menuName = "Roller Splat/Data/Level Data", order = 0)]
    public class LevelData : ScriptableObject
    {
        public enum CellType
        {
            Wall,
            Ground
        }
        
        [BoxGroup("Camera")] public Vector3 cameraPosition;
        [BoxGroup("Camera")] public Vector3 cameraRotation;

        [BoxGroup("Level")] public string levelName;
        [BoxGroup("Level")] public Vector2 size;
        [BoxGroup("Level")] public Vector2 startPosition;
        [BoxGroup("Level")] public int numberOfMoves;
        [BoxGroup("Level")] public CellData[] cells;
        [BoxGroup("Level")] public TextAsset levelFile;
    }
}