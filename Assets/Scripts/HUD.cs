using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

        private GameSettings _gameSettings;
        
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
        /// Level complete label
        /// </summary>
        private TMP_Text _levelCompleteText;
        /// <summary>
        /// Event trigger for tap to continue
        /// </summary>
        private EventTrigger _tapToContinueEventTrigger;
        
        private TweenerCore<float, float, FloatOptions> _fillAmountTween;

        /// <summary>
        /// Game over display
        /// </summary>
        [SerializeField] private GameObject gameOver;
        /// <summary>
        /// Level complete display
        /// </summary>
        [SerializeField] private GameObject levelComplete;
        /// <summary>
        /// Tap to continue display
        /// </summary>
        [SerializeField] private GameObject tapToContinue;

        #endregion

        #region Properties

        /// <summary>
        /// Level name
        /// </summary>
        public string LevelName
        {
            set
            {
                _levelName.text = value;
                _levelCompleteText.text = $"{value}\nCompleted!";
            }
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

        /// <summary>
        /// Display tap to continue ?
        /// </summary>
        public bool TapToContinue
        {
            get => tapToContinue.activeInHierarchy;
            set => tapToContinue.SetActive(value);
        }
        
        #endregion

        public event Action TapToContinueTouched;
        
        [Inject]
        public void Construct(
            GameSettings gameSettings,
            [Inject(Id = "LevelName")] TMP_Text levelName,
            [Inject(Id = "NumberOfMoves")] TMP_Text numberOfMoves,
            Image gauge,
            [Inject(Id = "LevelComplete")] TMP_Text levelCompleteText,
            EventTrigger tapToContinueEventTrigger)
        {
            _gameSettings = gameSettings;
            _levelName = levelName;
            _numberOfMoves = numberOfMoves;
            _gauge = gauge;
            _levelCompleteText = levelCompleteText;
            _tapToContinueEventTrigger = tapToContinueEventTrigger;

            var pointerClickEvent = new EventTrigger.TriggerEvent();
            pointerClickEvent.AddListener(TapToContinueClicked);
            
            _tapToContinueEventTrigger.triggers.Add(new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick,
                callback = pointerClickEvent
            });
        }

        private void TapToContinueClicked(BaseEventData eventData)
        {
            TapToContinueTouched?.Invoke();
        }
        
        /*private void Update()
        {
            if(!tapToContinue.activeInHierarchy) return;
            
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                TapToContinueTouched?.Invoke();
            }
        }*/
        
        #region Public Methods
        
        /// <summary>
        /// Set the number of moves
        /// </summary>
        /// <param name="currentMoves">Moves left</param>
        /// <param name="maxMoves">Max moves</param>
        public void SetNumberOfMoves(int currentMoves, int maxMoves)
        {
            _numberOfMoves.text = $"{currentMoves}/{maxMoves}";
            if (_fillAmountTween != null && _fillAmountTween.active)
            {
                _fillAmountTween.Complete();
            }
            _fillAmountTween = _gauge.DOFillAmount((float) currentMoves / maxMoves, _gameSettings.moveNumberGaugeFillDuration);
        }
        
        #endregion
    }
}