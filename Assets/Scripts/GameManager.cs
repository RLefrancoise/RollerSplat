using RollerSplat.Data;
using UnityEngine;
using Zenject;
using UniRx;

namespace RollerSplat
{
    public class GameManager : MonoBehaviour
    {
        private Player _player;
        private HUD _hud;
        private Level _level;

        private IntReactiveProperty _currentMoves;

        [Inject]
        public void Construct(Player player, HUD hud, Level level)
        {
            _player = player;
            _hud = hud;
            _level = level;

            //Current moves
            _currentMoves = new IntReactiveProperty();
            _currentMoves.Subscribe(UpdateNumberOfMoves);
            
            //Listen player movement
            _player.Move.Subscribe(ListenPlayerMove);

            //Listen level load
            _level.Load.Subscribe(OnLoadLevel);
        }
        
        private void Update()
        {
            if(!_player.CanMove) return;
            
            if (Input.GetKeyDown(KeyCode.UpArrow)) _player.Move.Execute(Player.MoveDirection.Up);
            if (Input.GetKeyDown(KeyCode.DownArrow)) _player.Move.Execute(Player.MoveDirection.Down);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) _player.Move.Execute(Player.MoveDirection.Left);
            if (Input.GetKeyDown(KeyCode.RightArrow)) _player.Move.Execute(Player.MoveDirection.Right);
        }

        private void ListenPlayerMove(Player.MoveDirection direction)
        {
            //Update number of moves
            _currentMoves.Value = Mathf.Max(0, _currentMoves.Value - 1);
        }

        private void OnLoadLevel(LevelData level)
        {
            //Update current moves
            _currentMoves.Value = level.numberOfMoves;
            //Update level name
            _hud.LevelName = level.levelName;
        }
        
        private void UpdateNumberOfMoves(int numberOfMoves)
        {
            if(_level.Data != null)
                _hud.SetNumberOfMoves(_currentMoves.Value, _level.Data.numberOfMoves);
        }
    }
}