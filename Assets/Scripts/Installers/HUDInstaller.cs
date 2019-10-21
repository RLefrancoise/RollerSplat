using TMPro;
using UnityEngine.UI;
using Zenject;

namespace RollerSplat.Installers
{
    public class HUDInstaller : MonoInstaller<HUDInstaller>
    {
        public TMP_Text levelName;
        public TMP_Text numberOfMoves;
        public Image radialGauge;
        
        public override void InstallBindings()
        {
            Container.Bind<TMP_Text>().WithId("LevelName").FromInstance(levelName);
            Container.Bind<TMP_Text>().WithId("NumberOfMoves").FromInstance(numberOfMoves);
            Container.Bind<Image>().FromInstance(radialGauge);
        }
    }
}