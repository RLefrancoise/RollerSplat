using RollerSplat.Data;

namespace RollerSplat
{
    /// <summary>
    /// Wall block
    /// </summary>
    public class Wall : LevelBlock
    {
        public override LevelData.CellType CellType => LevelData.CellType.Wall;
        
        private void Start()
        {
            renderer.material.color = GameSettings.defaultWallColor;
        }

    }
}