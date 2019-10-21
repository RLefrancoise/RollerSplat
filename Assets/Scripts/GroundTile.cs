using UniRx;
using UnityEngine;

namespace RollerSplat
{
    public class GroundTile : MonoBehaviour
    { 
        public ReactiveProperty<Color> Color { get; private set; }

        private void Awake()
        {
            Color = new ReactiveProperty<Color>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            //If player rolls on tile, apply player color to tile
            if (other.CompareTag("Player"))
            {
                Color.Value = other.GetComponent<Player>().Color.Value;
                Debug.LogFormat("GroundTile:OnTriggerEnter - Player rolls on tile {0} - Color: {1}", name, Color.Value);
            }
        }
    }
}