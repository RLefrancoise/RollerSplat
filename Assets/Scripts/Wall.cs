using UnityEngine;
using Zenject;

namespace RollerSplat
{
    public class Wall : MonoBehaviour
    {
        private Renderer _renderer;
        private Collider _collider;

        public Vector3 Extents => _collider.bounds.extents;
        
        [Inject]
        public void Construct(GameSettings gameSettings, Renderer r, Collider col)
        {
            _renderer = r;
            _collider = col;

            _renderer.material.color = gameSettings.defaultWallColor;
        }
    }
}