using UnityEngine;
using Zenject;

namespace RollerSplat.Installers
{
    public class WallInstaller : MonoInstaller<WallInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<Renderer>().FromComponentOnRoot();
        }
    }
}