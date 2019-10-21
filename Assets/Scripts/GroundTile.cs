using UniRx;
using UnityEngine;
using Zenject;

namespace RollerSplat
{
    /// <summary>
    /// A ground tile. It changes color when the player is rolling on it
    /// </summary>
    public class GroundTile : MonoBehaviour
    {
        private Renderer _renderer;
        [SerializeField] private ColorReactiveProperty color;

        [Inject]
        public void Construct(GameSettings gameSettings, Renderer r)
        {
            _renderer = r;
            color.Subscribe(ListenColorChanged);
            color.Value = gameSettings.defaultGroundColor;
        }

        private void ListenColorChanged(Color c)
        {
            _renderer.material.color = c;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            //If player rolls on tile, apply player color to tile
            if (other.CompareTag("Player"))
            {
                color.Value = other.GetComponent<Player>().Color;
            }
        }
    }
}