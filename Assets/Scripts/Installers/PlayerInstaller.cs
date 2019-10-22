using UnityEngine;
using Zenject;

namespace RollerSplat.Installers
{
    public class PlayerInstaller : MonoInstaller<PlayerInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<Renderer>().FromComponentOnRoot();
            Container.Bind<Rigidbody>().FromComponentOnRoot();
            Container.Bind<Animator>().FromComponentOnRoot();
        }
    }
}