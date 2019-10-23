using UnityEngine;
using Zenject;

namespace RollerSplat.Installers
{
    public class LevelBlockInstaller<TInstaller> : MonoInstaller<TInstaller> where TInstaller : LevelBlockInstaller<TInstaller>
    {
        public new Renderer renderer;
        
        public override void InstallBindings()
        {
            Container.Bind<Renderer>().FromInstance(renderer);
        }
    }
}