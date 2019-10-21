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
        private Canvas _gameOver;

        public string LevelName
        {
            set => _levelName.text = value;
        }

        public bool GameOver
        {
            set => _gameOver.enabled = value;
        }
        
        [Inject]
        public void Construct(
            [Inject(Id = "LevelName")] TMP_Text levelName,
            [Inject(Id = "NumberOfMoves")] TMP_Text numberOfMoves,
            Image gauge,
            Canvas gameOver)
        {
            _levelName = levelName;
            _numberOfMoves = numberOfMoves;
            _gauge = gauge;
            _gameOver = gameOver;
        }

        public void SetNumberOfMoves(int currentMoves, int maxMoves)
        {
            _numberOfMoves.text = $"{currentMoves}/{maxMoves}";
            _gauge.fillAmount = (float) currentMoves / maxMoves;
        }
    }
}