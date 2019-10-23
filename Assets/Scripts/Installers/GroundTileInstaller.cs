using System.Collections.Generic;
using UnityEngine;

namespace RollerSplat.Installers
{
    public class GroundTileInstaller : LevelBlockInstaller<GroundTileInstaller>
    {
        public Renderer border;
        public Animator animator;
        public Renderer[] paintPlayerRenderer;
        
        public override void InstallBindings()
        {
            base.InstallBindings();
            Container.Bind<Renderer>().WithId("Border").FromInstance(border);
            Container.Bind<Animator>().FromInstance(animator);
            Container.Bind<IEnumerable<Renderer>>().FromInstance(paintPlayerRenderer);
        }
    }
}