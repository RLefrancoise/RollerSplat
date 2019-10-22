using System;
using RollerSplat.Data;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
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

        private Vector2 _swipeStartScreenPosition;
        
        [SerializeField] private ScreenTransformGesture swipeGesture;
        
        [SerializeField] private IntReactiveProperty currentMoves;
        [SerializeField] private LevelData levelToLoad;

        private bool CanPlayerMove => _player.CanMove && currentMoves.Value > 0 && !_level.IsLevelComplete.Value;
        
        [Inject]
        public void Construct(Player player, HUD hud, Level level)
        {
            _player = player;
            _hud = hud;
            _level = level;

            //Current moves
            currentMoves = new IntReactiveProperty();
            currentMoves.Subscribe(UpdateNumberOfMoves);
        }

        private void OnEnable()
        {
            swipeGesture.TransformStarted += StartSwipe;
            swipeGesture.StateChanged += SwipeGesture;
        }

        private void OnDisable()
        {
            swipeGesture.TransformStarted -= StartSwipe;
            swipeGesture.StateChanged -= SwipeGesture;
        }
        
        private void Start()
        {
            //Listen player movement
            _player.Move.Subscribe(ListenPlayerMove);
            //Listen level load
            _level.Load.Subscribe(ListenLoadLevel);
            //Listen level completed status
            _level.IsLevelComplete.Subscribe(ListenLevelCompleted);

            //No game over at start
            _hud.GameOver = false;
            //No level complete at start
            _hud.LevelComplete = false;
            
            //If data are set, load them
            if (levelToLoad) _level.Load.Execute(levelToLoad);
        }
        
        private void Update()
        {
            if(!CanPlayerMove) return;
            
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

        private void ListenLevelCompleted(bool levelCompleted)
        {
            _hud.LevelComplete = levelCompleted;
            
            //If no more moves & level is not complete, game over
            _hud.GameOver = currentMoves.Value == 0 && !levelCompleted;
        }
        
        private void UpdateNumberOfMoves(int numberOfMoves)
        {
            if(_level.Data != null)
                _hud.SetNumberOfMoves(currentMoves.Value, _level.Data.numberOfMoves);
        }

        private void StartSwipe(object sender, EventArgs e)
        {
            if(!CanPlayerMove) return;
            _swipeStartScreenPosition = swipeGesture.NormalizedScreenPosition;
        }
        
        private void SwipeGesture(object sender, GestureStateChangeEventArgs e)
        {
            if(!CanPlayerMove) return;
            if (e.State != Gesture.GestureState.Recognized) return;
            
            var swipeLength = swipeGesture.NormalizedScreenPosition - _swipeStartScreenPosition;
            if(Mathf.Abs(swipeLength.x) < 0.2f && Mathf.Abs(swipeLength.y) <= 0.2f) return;
                
            if (swipeLength.x >= 0.2f)
            {
                _player.Move.Execute(Player.MoveDirection.Right);
            }
            else if (swipeLength.x <= -0.2f)
            {
                _player.Move.Execute(Player.MoveDirection.Left);
            }
            else if (swipeLength.y >= 0.2f)
            {
                _player.Move.Execute(Player.MoveDirection.Up);
            }
            else if (swipeLength.y <= -0.2f)
            {
                _player.Move.Execute(Player.MoveDirection.Down);
            }
        }
    }
}