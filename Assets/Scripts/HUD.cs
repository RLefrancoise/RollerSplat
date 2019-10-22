using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace RollerSplat
{
    public class HUD : MonoBehaviour
    {
        private TMP_Text _levelName;
        private TMP_Text _numberOfMoves;
        private Image _gauge;
        [SerializeField] private GameObject _gameOver;
        [SerializeField] private GameObject levelComplete;

        public string LevelName
        {
            set => _levelName.text = value;
        }

        public bool GameOver
        {
            set => _gameOver.SetActive(value);
        }

        public bool LevelComplete
        {
            set => levelComplete.SetActive(value);
        }
        
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

        public void SetNumberOfMoves(int currentMoves, int maxMoves)
        {
            _numberOfMoves.text = $"{currentMoves}/{maxMoves}";
            _gauge.fillAmount = 1f - (float) currentMoves / maxMoves;
        }
    }
}