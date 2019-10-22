using RollerSplat.Data;

namespace RollerSplat
{
    public class Wall : LevelBlock
    {
        public override LevelData.CellType CellType => LevelData.CellType.Wall;
        
        private void Start()
        {
            Renderer.material.color = GameSettings.defaultWallColor;
        }

    }
}