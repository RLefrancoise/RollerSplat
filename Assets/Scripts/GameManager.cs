using System;
using RollerSplat.Data;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Async;

namespace RollerSplat
{
    /// <summary>
    /// The game manager. It makes all the pieces (Player, Level, HUD, ...) work together 
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// All the levels of the game
        /// </summary>
        private LevelData[] _levels;
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
        /// Current level index
        /// </summary>
        [SerializeField] private IntReactiveProperty currentLevel;

        #endregion

        #region Properties

        /// <summary>
        /// Can the player move ?
        /// </summary>
        private bool CanPlayerMove => !_hud.TapToContinue && _player.CanMove && currentMoves.Value > 0 && !_level.IsLevelComplete.Value;

        #endregion
        
        [Inject]
        public void Construct(Player player, HUD hud, Level level)
        {
            _player = player;
            _hud = hud;
            _level = level;
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
            //Get all levels
            _levels = Resources.LoadAll<LevelData>("Levels");

            //No game over at start
            _hud.GameOver = false;
            //No level complete at start
            _hud.LevelComplete = false;
            //No tap to continue at start
            _hud.TapToContinue = false;

            //Listen level completed
            _level.IsLevelComplete.SkipLatestValueOnSubscribe().Subscribe(ListenLevelCompleted);
            
            //Listen if player has touched the screen when tap to continue is displayed
            _hud.TapToContinueTouched += ListenTapToContinueTouched;
            
            //Current moves
            currentMoves.SkipLatestValueOnSubscribe().Subscribe(UpdateNumberOfMoves);
            
            //Listen current level index
            currentLevel.Subscribe(GoToLevel);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if(!CanPlayerMove) return;
            
            if (Input.GetKeyDown(KeyCode.UpArrow)) MovePlayer(Player.MoveDirection.Up);
            if (Input.GetKeyDown(KeyCode.DownArrow)) MovePlayer(Player.MoveDirection.Down);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) MovePlayer(Player.MoveDirection.Left);
            if (Input.GetKeyDown(KeyCode.RightArrow)) MovePlayer(Player.MoveDirection.Right);
        }
#endif

        #endregion

        #region Private Methods

        /// <summary>
        /// Called when HUD tap to continue is touched
        /// </summary>
        private void ListenTapToContinueTouched()
        {
            _hud.TapToContinue = false;
        }
        
        /// <summary>
        /// Called when current level index is changed
        /// </summary>
        /// <param name="levelIndex">Index of the level to go to</param>
        private void GoToLevel(int levelIndex)
        {
            if (levelIndex >= _levels.Length)
            {
                Debug.LogErrorFormat("GameManager:GoToLevel : No more levels. (index: {0})", levelIndex);
                currentLevel.Value = 0;
            }
            else
            {
                var levelData = _levels[levelIndex];
            
                //Update level name
                _hud.LevelName = levelData.levelName;
            
                //load the level
                _level.Load.Execute(_levels[levelIndex]);
            
                //Update current moves
                currentMoves.Value = levelData.numberOfMoves;
                
                //Place player on start tile
                _player.PlaceOnTile.Execute(levelData.startPosition);
            
                //Ask the player to tap to continue
                _hud.TapToContinue = true;
                _hud.LevelComplete = false;
                _hud.GameOver = false;
            }
        }

        /// <summary>
        /// Called when the level completed flag is changed
        /// </summary>
        /// <param name="levelCompleted">Is the level completed ?</param>
        private async void ListenLevelCompleted(bool levelCompleted)
        {
            if (levelCompleted)
            {
                _hud.GameOver = false;
                _hud.LevelComplete = true;
                
                //If level completed, make the player bounce & go to the next level
                await _player.StopMove(_level.LastGroundTilePaintedByPlayer.Root.position);
                await _player.Bounce();
                //Go to next level
                currentLevel.Value = currentLevel.Value + 1;
            }
        }
        
        /// <summary>
        /// Called when the number of moves left has changed
        /// </summary>
        /// <param name="numberOfMoves"></param>
        private void UpdateNumberOfMoves(int numberOfMoves)
        {
            if(_level.Data != null)
                _hud.SetNumberOfMoves(currentMoves.Value, _level.Data.numberOfMoves);
            
            if (numberOfMoves == 0)
            {
                if (!_level.IsLevelComplete.Value)
                {
                    _hud.GameOver = true;
                    
                    Debug.Log("Game Over");
                    UniTask.Delay(TimeSpan.FromSeconds(3f)).ToObservable().Subscribe(_ =>
                    {
                        //Replay the level
                        GoToLevel(currentLevel.Value);
                    });
                }
            }
            else
            {
                _hud.GameOver = false;
            }
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
            if(Mathf.Abs(swipeLength.x) < 0.05f && Mathf.Abs(swipeLength.y) <= 0.05f) return;
                
            if(Mathf.Abs(swipeLength.x) > Mathf.Abs(swipeLength.y))
            {
                if (swipeLength.x >= 0.05f)
                {
                    MovePlayer(Player.MoveDirection.Right);
                }
                else if (swipeLength.x <= -0.05f)
                {
                    MovePlayer(Player.MoveDirection.Left);
                }
            }
            else
            {
                if (swipeLength.y >= 0.05f)
                {
                    MovePlayer(Player.MoveDirection.Up);
                }
                else if (swipeLength.y <= -0.05f)
                {
                    MovePlayer(Player.MoveDirection.Down);
                }
            }
        }

        /// <summary>
        /// Move the player and decreases remaining moves
        /// </summary>
        /// <param name="direction">Direction of the movement</param>
        private async void MovePlayer(Player.MoveDirection direction)
        {
            var moveSuccess = await _player.Move(direction);
            if (moveSuccess)
            {
                //Update number of moves
                currentMoves.Value = Mathf.Max(0, currentMoves.Value - 1);
            }
        }
        
        #endregion
    }
}