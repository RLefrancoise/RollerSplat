using UniRx;
using UnityEngine;

namespace RollerSplat
{
    public class Player : MonoBehaviour
    {
        public ReactiveProperty<Color> Color { get; private set; }

        private void Awake()
        {
            Color = new ReactiveProperty<Color>();
        }
    }
}