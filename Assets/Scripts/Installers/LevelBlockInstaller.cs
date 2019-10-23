using UnityEngine;
using Zenject;

namespace RollerSplat.Installers
{
    public class LevelBlockInstaller<TInstaller> : MonoInstaller<TInstaller> where TInstaller : LevelBlockInstaller<TInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<Renderer>().FromComponentOnRoot();
        }
    }
}