using System;
using RollerSplat.Data;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;
using Zenject;
using UniRx;

namespace RollerSplat
{
    /// <summary>
    /// The game manager. It makes all the pieces (Player, Level, HUD, ...) work together 
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Fields
        /// <summary>
        /// The player
        /// </summary>
        private Player _player;
        /// <summary>
        /// The HUD
        /// </summary>
        private HUD _hud;
        /// <summary>
        /// The level
        /// </summary>
        private Level _level;
        /// <summary>
        /// Swipe gesture start position. Used to compute swipe gesture
        /// </summary>
        private Vector2 _swipeStartScreenPosition;
        /// <summary>
        /// Swipe gesture listener
        /// </summary>
        [SerializeField] private ScreenTransformGesture swipeGesture;
        /// <summary>
        /// Current number of moves left
        /// </summary>
        [SerializeField] private IntReactiveProperty currentMoves;
        /// <summary>
        /// Level data to load
        /// </summary>
        [SerializeField] private LevelDataReactiveProperty levelToLoad;

        #endregion

        #region Properties

        /// <summary>
        /// Can the player move ?
        /// </summary>
        private bool CanPlayerMove => _player.CanMove && currentMoves.Value > 0 && !_level.IsLevelComplete.Value;

        #endregion
        
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

        #region Monobehaviour Callbacks
        
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
            _level.Load.Subscribe(ListenLevelLoaded);
            //Listen level completed status
            _level.IsLevelComplete.Subscribe(ListenLevelCompleted);

            //No game over at start
            _hud.GameOver = false;
            //No level complete at start
            _hud.LevelComplete = false;

            //Listen level data
            levelToLoad.Subscribe(ListenLevelData);
        }
        
        #if UNITY_EDITOR
        private void Update()
        {
            if(!CanPlayerMove) return;
            
            if (Input.GetKeyDown(KeyCode.UpArrow)) _player.Move.Execute(Player.MoveDirection.Up);
            if (Input.GetKeyDown(KeyCode.DownArrow)) _player.Move.Execute(Player.MoveDirection.Down);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) _player.Move.Execute(Player.MoveDirection.Left);
            if (Input.GetKeyDown(KeyCode.RightArrow)) _player.Move.Execute(Player.MoveDirection.Right);
        }
        #endif

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Called when player has moved
        /// </summary>
        /// <param name="direction">Direction of the movement</param>
        private void ListenPlayerMove(Player.MoveDirection direction)
        {
            //Update number of moves
            currentMoves.Value = Mathf.Max(0, currentMoves.Value - 1);
        }

        /// <summary>
        /// Called when level data have changed
        /// </summary>
        /// <param name="level">New level data</param>
        private void ListenLevelData(LevelData level)
        {
            _level.Load.Execute(level);
        }
        
        /// <summary>
        /// Listen when the level has been loaded
        /// </summary>
        /// <param name="level">Data of the loaded level</param>
        private void ListenLevelLoaded(LevelData level)
        {
            //Place player on start tile
            _player.PlaceOnTile.Execute(level.startPosition);
            
            //Update current moves
            currentMoves.Value = level.numberOfMoves;
            //Update level name
            _hud.LevelName = level.levelName;
        }

        /// <summary>
        /// Called when the level completed flag is changed
        /// </summary>
        /// <param name="levelCompleted">Is the level completed ?</param>
        private void ListenLevelCompleted(bool levelCompleted)
        {
            _hud.LevelComplete = levelCompleted;
            
            //If no more moves & level is not complete, game over
            _hud.GameOver = currentMoves.Value == 0 && !levelCompleted;
        }
        
        /// <summary>
        /// Called when the number of moves left has changed
        /// </summary>
        /// <param name="numberOfMoves"></param>
        private void UpdateNumberOfMoves(int numberOfMoves)
        {
            if(_level.Data != null)
                _hud.SetNumberOfMoves(currentMoves.Value, _level.Data.numberOfMoves);
        }

        /// <summary>
        /// Called when the swipe gesture detection has started
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartSwipe(object sender, EventArgs e)
        {
            if(!CanPlayerMove) return;
            _swipeStartScreenPosition = swipeGesture.NormalizedScreenPosition;
        }
        
        /// <summary>
        /// Called when the swipe gesture has ended
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        
        #endregion
    }
}