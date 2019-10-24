using UnityEngine;
using Zenject;

namespace RollerSplat.Installers
{
    public class PlayerInstaller : MonoInstaller<PlayerInstaller>
    {
        public new Renderer renderer;
        public Rigidbody rigidBody;
        public Animator animator;
        public SphereCollider collider;
        public TrailRenderer trail;
        
        public override void InstallBindings()
        {
            Container.Bind<Renderer>().FromInstance(renderer);
            Container.Bind<Rigidbody>().FromInstance(rigidBody);
            Container.Bind<Animator>().FromInstance(animator);
            Container.Bind<SphereCollider>().FromInstance(collider);
            Container.Bind<TrailRenderer>().FromInstance(trail);
        }
    }
}