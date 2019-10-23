using RollerSplat.Data;
using UnityEngine;
using Zenject;

namespace RollerSplat
{
    /// <summary>
    /// Base class for level blocks
    /// </summary>
    public abstract class LevelBlock : MonoBehaviour
    {
        #region Fields
        
        /// <summary>
        /// Root of the block
        /// </summary>
        [SerializeField] protected Transform root;
        
        /// <summary>
        /// Game settings
        /// </summary>
        protected GameSettings GameSettings;
        
        /// <summary>
        /// Block renderer
        /// </summary>
        [SerializeField] protected new Renderer renderer;

        #endregion

        #region Properties

        /// <summary>
        /// Block type
        /// </summary>
        public abstract LevelData.CellType CellType { get; }
        
        /// <summary>
        /// Root of the block
        /// </summary>
        public Transform Root => root;
        
        #endregion
        
        [Inject]
        public void Construct(GameSettings gameSettings)
        {
            GameSettings = gameSettings;
        }
    }
}