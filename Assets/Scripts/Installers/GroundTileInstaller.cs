using UnityEngine;
using Zenject;

namespace RollerSplat.Installers
{
    public class GroundTileInstaller : MonoInstaller<GroundTileInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<Renderer>().FromComponentOnRoot();
        }
    }
}