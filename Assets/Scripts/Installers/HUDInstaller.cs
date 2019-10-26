using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace RollerSplat.Installers
{
    public class HUDInstaller : MonoInstaller<HUDInstaller>
    {
        public TMP_Text levelName;
        public TMP_Text numberOfMoves;
        public Image gauge;
        public TMP_Text levelComplete;
        public EventTrigger tapToContinueEventTrigger;
        public OptionToggle vibrationToggle;
        public OptionToggle soundToggle;
        
        public override void InstallBindings()
        {
            Container.Bind<TMP_Text>().WithId("LevelName").FromInstance(levelName);
            Container.Bind<TMP_Text>().WithId("NumberOfMoves").FromInstance(numberOfMoves);
            Container.Bind<Image>().FromInstance(gauge);
            Container.Bind<TMP_Text>().WithId("LevelComplete").FromInstance(levelComplete);
            Container.Bind<EventTrigger>().FromInstance(tapToContinueEventTrigger);
            Container.Bind<OptionToggle>().WithId("Vibration").FromInstance(vibrationToggle);
            Container.Bind<OptionToggle>().WithId("Sound").FromInstance(soundToggle);
        }
    }
}