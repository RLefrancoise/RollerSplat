using RollerSplat.Data;
using UniRx;
using UnityEngine;

namespace RollerSplat
{
    /// <summary>
    /// A ground tile. It changes color when the player is rolling on it
    /// </summary>
    public class GroundTile : LevelBlock
    {
        [SerializeField] private BoolReactiveProperty isPaintedByPlayer;
        [SerializeField] private ColorReactiveProperty color;
        
        public override LevelData.CellType CellType => LevelData.CellType.Ground;
        public BoolReactiveProperty IsPaintedByPlayer => isPaintedByPlayer;
        
        private void Start()
        {
            color.Subscribe(ListenColorChanged);
            color.Value = GameSettings.defaultGroundColor;
        }
        
        private void ListenColorChanged(Color c)
        {
            Renderer.material.color = c;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            //If player rolls on tile, apply player color to tile
            if (other.CompareTag("Player"))
            {
                color.Value = other.GetComponent<Player>().Color;
                isPaintedByPlayer.Value = true;
            }
        }
    }
}