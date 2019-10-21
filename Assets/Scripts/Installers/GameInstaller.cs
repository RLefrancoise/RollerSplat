using Zenject;

namespace RollerSplat.Installers
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<GameSettings>().FromResources("GameSettings").AsSingle();
        }
    }
}