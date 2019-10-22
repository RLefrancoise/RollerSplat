using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace RollerSplat
{
    /// <summary>
    /// Human User Display
    /// </summary>
    public class HUD : MonoBehaviour
    {
        #region Fields
        
        /// <summary>
        /// Level name label
        /// </summary>
        private TMP_Text _levelName;
        /// <summary>
        /// Number of moves label
        /// </summary>
        private TMP_Text _numberOfMoves;
        /// <summary>
        /// Number of moves left gauge
        /// </summary>
        private Image _gauge;
        /// <summary>
        /// Game over display
        /// </summary>
        [SerializeField] private GameObject gameOver;
        /// <summary>
        /// Level complete display
        /// </summary>
        [SerializeField] private GameObject levelComplete;

        #endregion

        #region Properties

        /// <summary>
        /// Level name
        /// </summary>
        public string LevelName
        {
            set => _levelName.text = value;
        }

        /// <summary>
        /// Display game over ?
        /// </summary>
        public bool GameOver
        {
            set => gameOver.SetActive(value);
        }

        /// <summary>
        /// Display level complete ?
        /// </summary>
        public bool LevelComplete
        {
            set => levelComplete.SetActive(value);
        }
        
        #endregion
        [Inject]
        public void Construct(
            [Inject(Id = "LevelName")] TMP_Text levelName,
            [Inject(Id = "NumberOfMoves")] TMP_Text numberOfMoves,
            Image gauge)
        {
            _levelName = levelName;
            _numberOfMoves = numberOfMoves;
            _gauge = gauge;
        }

        #region Public Methods
        
        /// <summary>
        /// Set the number of moves
        /// </summary>
        /// <param name="currentMoves">Moves left</param>
        /// <param name="maxMoves">Max moves</param>
        public void SetNumberOfMoves(int currentMoves, int maxMoves)
        {
            _numberOfMoves.text = $"{currentMoves}/{maxMoves}";
            _gauge.fillAmount = 1f - (float) currentMoves / maxMoves;
        }
        
        #endregion
    }
}