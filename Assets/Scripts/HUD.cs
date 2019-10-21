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
        private Image _radialGauge;

        public string LevelName
        {
            set => _levelName.text = value;
        }
        
        [Inject]
        public void Construct(
            [Inject(Id = "LevelName")] TMP_Text levelName,
            [Inject(Id = "NumberOfMoves")] TMP_Text numberOfMoves,
            Image radialGauge)
        {
            _levelName = levelName;
            _numberOfMoves = numberOfMoves;
            _radialGauge = radialGauge;
        }

        public void SetNumberOfMoves(int currentMoves, int maxMoves)
        {
            _numberOfMoves.text = $"{currentMoves}/{maxMoves}";
            _radialGauge.fillAmount = (float) currentMoves / maxMoves;
        }
    }
}