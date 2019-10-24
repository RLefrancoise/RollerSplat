using System.Collections.Generic;
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

    public class LevelBlockDistanceComparer : IComparer<LevelBlock>
    {
        private readonly Vector3 _referencePosition;

        public LevelBlockDistanceComparer(Vector3 referencePosition)
        {
            _referencePosition = referencePosition;
        }
        
        public int Compare(LevelBlock x, LevelBlock y)
        {
            var xDistance = Vector3.Distance(_referencePosition, x.Root.position);
            var yDistance = Vector3.Distance(_referencePosition, y.Root.position);

            if (xDistance < yDistance) return -1;
            if (xDistance > yDistance) return 1;
            return 0;
        }
    }
}