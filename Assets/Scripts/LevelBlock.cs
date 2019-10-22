using RollerSplat.Data;
using UnityEngine;
using Zenject;

namespace RollerSplat
{
    public abstract class LevelBlock : MonoBehaviour
    {
        [SerializeField] protected Transform root;
        protected GameSettings GameSettings;
        protected Renderer Renderer;

        public abstract LevelData.CellType CellType { get; }
        public Transform Root => root;
        
        [Inject]
        public void Construct(GameSettings gameSettings, Renderer r)
        {
            GameSettings = gameSettings;
            Renderer = r;
        }
    }
}