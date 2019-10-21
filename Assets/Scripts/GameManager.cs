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

        [SerializeField] private IntReactiveProperty currentMoves;
        [SerializeField] private LevelData levelToLoad;
        
        [Inject]
        public void Construct(Player player, HUD hud, Level level)
        {
            _player = player;
            _hud = hud;
            _level = level;

            //Current moves
            currentMoves = new IntReactiveProperty();
            currentMoves.Subscribe(UpdateNumberOfMoves);
            
            //Listen player movement
            _player.Move.Subscribe(ListenPlayerMove);

            //Listen level load
            _level.Load.Subscribe(ListenLoadLevel);
        }
        
        private void Start()
        {
            //If data are set, load them
            if (levelToLoad) _level.Load.Execute(levelToLoad);
        }
        
        private void Update()
        {
            if(!_player.CanMove) return;
            if(currentMoves.Value == 0) return;
            
            if (Input.GetKeyDown(KeyCode.UpArrow)) _player.Move.Execute(Player.MoveDirection.Up);
            if (Input.GetKeyDown(KeyCode.DownArrow)) _player.Move.Execute(Player.MoveDirection.Down);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) _player.Move.Execute(Player.MoveDirection.Left);
            if (Input.GetKeyDown(KeyCode.RightArrow)) _player.Move.Execute(Player.MoveDirection.Right);
        }

        private void ListenPlayerMove(Player.MoveDirection direction)
        {
            //Update number of moves
            currentMoves.Value = Mathf.Max(0, currentMoves.Value - 1);
        }

        private void ListenLoadLevel(LevelData level)
        {
            //Update current moves
            currentMoves.Value = level.numberOfMoves;
            //Update level name
            _hud.LevelName = level.levelName;
        }
        
        private void UpdateNumberOfMoves(int numberOfMoves)
        {
            if(_level.Data != null)
                _hud.SetNumberOfMoves(currentMoves.Value, _level.Data.numberOfMoves);

            //If no more moves, game over
            _hud.GameOver = numberOfMoves == 0;
        }
    }
}