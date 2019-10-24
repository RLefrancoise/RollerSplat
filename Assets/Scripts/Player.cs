using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using NaughtyAttributes;
using RollerSplat.Data;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Zenject;

namespace RollerSplat
{
    /// <summary>
    /// The player. It moves in the level in straight line until it finds a wall.
    /// </summary>
    public class Player : MonoBehaviour
    {
        #region Enums
        
        /// <summary>
        /// Move direction of the player
        /// </summary>
        public enum MoveDirection
        {
            Up, Down, Left, Right
        }
        
        #endregion

        #region Fields
        
        private static readonly Dictionary<MoveDirection, Quaternion> RotationsByDirection = new Dictionary<MoveDirection, Quaternion>
        {
            {MoveDirection.Up, Quaternion.LookRotation(Vector3.forward, Vector3.up)},
            {MoveDirection.Down, Quaternion.LookRotation(-Vector3.forward, Vector3.up)},
            {MoveDirection.Left, Quaternion.LookRotation(-Vector3.right, Vector3.up)},
            {MoveDirection.Right, Quaternion.LookRotation(Vector3.right, Vector3.up)}
        };
        
        /// <summary>
        /// Game settings
        /// </summary>
        private GameSettings _gameSettings;
        /// <summary>
        /// Player renderer
        /// </summary>
        private Renderer _renderer;
        /// <summary>
        /// Player rigid body
        /// </summary>
        private Rigidbody _rigidBody;
        /// <summary>
        /// Player animator
        /// </summary>
        private Animator _animator;
        /// <summary>
        /// Player trail
        /// </summary>
        private TrailRenderer _trail;
        /// <summary>
        /// Stop move flag
        /// </summary>
        private bool _stopMove;
        /// <summary>
        /// Player move tween
        /// </summary>
        private TweenerCore<Vector3, Vector3, VectorOptions> _moveTween;
        /// <summary>
        /// Place on tile command
        /// </summary>
        private ReactiveCommand<Vector2> _placeOnTile;
        /// <summary>
        /// Player current color
        /// </summary>
        [Tooltip("Player current color")]
        [SerializeField] private ColorReactiveProperty color;
        /// <summary>
        /// Was the player teleported ?
        /// </summary>
        [SerializeField] private BoolReactiveProperty wasTeleported;

        private static readonly int BounceTrigger = Animator.StringToHash("Bounce");
        private static readonly int TeleportBool = Animator.StringToHash("Teleport");

        #endregion

        #region Properties
        
        /// <summary>
        /// Can the player move ?
        /// </summary>
        [ShowNativeProperty] public bool CanMove => _moveTween == null || !_moveTween.active;
        
        /// <summary>
        /// Current player color
        /// </summary>
        public ColorReactiveProperty Color => color;
        
        /// <summary>
        /// Was the player teleported ?
        /// </summary>
        public BoolReactiveProperty WasTeleported => wasTeleported;

        /// <summary>
        /// Place the player on the given tile position
        /// </summary>
        public ReactiveCommand<Vector2> PlaceOnTile
        {
            get
            {
                if (_placeOnTile == null)
                {
                    _placeOnTile = new ReactiveCommand<Vector2>();
                    _placeOnTile.Subscribe(ExecutePlaceOnTile);
                }

                return _placeOnTile;
            }
        }

        #endregion

        #region Public Methods

        [Inject]
        public void Construct(
            GameSettings gameSettings, 
            Renderer r, 
            Rigidbody rigidBody,
            Animator animator,
            TrailRenderer trail)
        {
            _gameSettings = gameSettings;
            _renderer = r;
            _rigidBody = rigidBody;
            _animator = animator;
            _trail = trail;

            color.Subscribe(ListenColor);

            _trail.enabled = _gameSettings.playerTrail;
        }

        /// <summary>
        /// Stop the player at tile position
        /// </summary>
        /// <param name="tilePosition">Position in world space</param>
        /// <returns></returns>
        public async UniTask StopMove(Vector3 tilePosition)
        {
            _stopMove = true;
            _moveTween.Kill();
            _moveTween = null;
            var distance = Vector3.Distance(transform.position, tilePosition);
            _moveTween = transform.DOMove(tilePosition, distance / _gameSettings.playerSpeed).SetEase(Ease.Linear);
            await _moveTween.ToUniTask();
        }

        /// <summary>
        /// Called when the move command is executed
        /// </summary>
        /// <param name="dir">Direction of the movement</param>
        public async UniTask<bool> Move(MoveDirection dir)
        {
            if (!CanMove) return false;

            //Reset teleported flag
            wasTeleported.Value = false;

            //Reset stop move flag
            _stopMove = false;
            
            //Rotate to the right direction
            transform.rotation = RotationsByDirection[dir];
            
            //Get all possible hits in player direction
            var levelBlocks = Physics.RaycastAll(new Ray(transform.position, transform.forward)).Select(h => h.collider.GetComponentInParent<LevelBlock>()).ToList();
            //If no hits, don't move
            if (levelBlocks.Count == 0) return false;
            
            //Sort blocks by player distance
            levelBlocks.Sort(new LevelBlockDistanceComparer(transform.position));
            
            //If first hit is a wall, don't move
            if (levelBlocks[0].CellType == LevelData.CellType.Wall) return false;

            foreach (var levelBlock in levelBlocks)
            {
                //If stop move requested, stop iterating
                if(_stopMove) break;
                
                //Move until we find a wall
                if (levelBlock.CellType == LevelData.CellType.Wall) break;
                
                //Apply the movement
                _moveTween = transform.DOMove(levelBlock.Root.position, _gameSettings.blockSize / _gameSettings.playerSpeed).SetEase(Ease.Linear);
                await _moveTween.ToUniTask();
            }

            return true;
        }

        /// <summary>
        /// Play teleport animation
        /// </summary>
        /// <param name="teleport">Teleport on or off</param>
        /// <returns></returns>
        public async UniTask Teleport(bool teleport)
        {
            _animator.SetBool(TeleportBool, teleport);
            await UniTask.WaitUntil(() => 
                _animator.GetCurrentAnimatorStateInfo(0).IsTag("Teleport") && 
                _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Called when the place on tile command is executed
        /// </summary>
        /// <param name="tile">Tile position</param>
        private void ExecutePlaceOnTile(Vector2 tile)
        {
            //Set teleported flag to true
            wasTeleported.Value = true;
            
            if(_gameSettings.playerTrail) _trail.enabled = false;
            
            transform.localPosition = Vector3.right * tile.x * _gameSettings.blockSize +
                                      -Vector3.forward * tile.y * _gameSettings.blockSize +
                                      Vector3.up * _gameSettings.blockSize / 2f;
            _rigidBody.position = transform.position;
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;

            if(_gameSettings.playerTrail) _trail.enabled = true;
        }

        /// <summary>
        /// Called when bounce is executed
        /// </summary>
        /// <returns></returns>
        public async UniTask Bounce()
        {
            _animator.SetTrigger(BounceTrigger);
            await UniTask.Delay(TimeSpan.FromSeconds(3f));
        }

        /// <summary>
        /// Called when player color is changed
        /// </summary>
        /// <param name="c"></param>
        private void ListenColor(Color c)
        {
            _renderer.material.DOColor(color.Value, 0.25f);

            if(_gameSettings.playerTrail)
            {
                var trailGradient = new Gradient
                {
                    colorKeys = new[] {new GradientColorKey(c, 0f), new GradientColorKey(UnityEngine.Color.white, 1f)},
                    alphaKeys = new[] {new GradientAlphaKey(0.5f, 0f), new GradientAlphaKey(0f, 1f)}
                };
    
                _trail.colorGradient = trailGradient;
            }
        }
        
        #endregion
    }
}