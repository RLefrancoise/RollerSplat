using UnityEngine;
using Zenject;

namespace RollerSplat.Installers
{
    public class GameSceneInstaller : MonoInstaller<GameSceneInstaller>
    {
        public Level level;
        public Player player;
        public HUD hud;
        public TouchManager touchManager;
        public OptionsManager optionsManager;
        
        public override void InstallBindings()
        {
            Container.Bind<Camera>().FromInstance(Camera.main).AsSingle();
            Container.Bind<Level>().FromInstance(level).AsSingle();
            Container.Bind<Player>().FromInstance(player).AsSingle();
            Container.Bind<HUD>().FromInstance(hud).AsSingle();
            Container.BindInterfacesAndSelfTo<ITouchManager>().FromInstance(touchManager);
            Container.BindInterfacesAndSelfTo<IOptionsManager>().FromInstance(optionsManager);
            Container.BindInterfacesAndSelfTo<IHapticManager>().FromInstance(new HapticManager()).AsSingle();
        }
    }
}