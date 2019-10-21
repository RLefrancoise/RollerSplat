using UnityEngine;
using Zenject;

namespace RollerSplat
{
    public class PlayerInput : MonoBehaviour
    {
        private Player _player;
        
        [Inject]
        public void Construct(Player player)
        {
            _player = player;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) _player.Move.Execute(Player.MoveDirection.Up);
            if (Input.GetKeyDown(KeyCode.DownArrow)) _player.Move.Execute(Player.MoveDirection.Down);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) _player.Move.Execute(Player.MoveDirection.Left);
            if (Input.GetKeyDown(KeyCode.RightArrow)) _player.Move.Execute(Player.MoveDirection.Right);
        }
    }
}