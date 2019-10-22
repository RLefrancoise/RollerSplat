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
        #region Fields
        
        /// <summary>
        /// Is tile painted by player ?
        /// </summary>
        [SerializeField] private BoolReactiveProperty isPaintedByPlayer;
        /// <summary>
        /// Tile color
        /// </summary>
        [SerializeField] private ColorReactiveProperty color;
        
        #endregion

        #region Properties

        public override LevelData.CellType CellType => LevelData.CellType.Ground;
        
        /// <summary>
        /// Is tile painted by player ?
        /// </summary>
        public BoolReactiveProperty IsPaintedByPlayer => isPaintedByPlayer;

        #endregion

        #region Monobehaviour Callbacks

        private void Start()
        {
            color.Subscribe(ListenColorChanged);
            color.Value = GameSettings.defaultGroundColor;
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
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Called when tile color is changed
        /// </summary>
        /// <param name="c">New tile color</param>
        private void ListenColorChanged(Color c)
        {
            Renderer.material.color = c;
        }
        
        #endregion
    }
}