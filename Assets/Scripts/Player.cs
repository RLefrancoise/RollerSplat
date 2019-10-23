using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using NaughtyAttributes;
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
        /// Player move tween
        /// </summary>
        private TweenerCore<Vector3, Vector3, VectorOptions> _moveTween;
        /// <summary>
        /// Move command
        /// </summary>
        private AsyncReactiveCommand<MoveDirection> _moveCommand;
        /// <summary>
        /// Place on tile command
        /// </summary>
        private ReactiveCommand<Vector2> _placeOnTile;
        /// <summary>
        /// Bounce the player
        /// </summary>
        private AsyncReactiveCommand _bounce;
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
        /// Move the player in the given direction
        /// </summary>
        public AsyncReactiveCommand<MoveDirection> Move
        {
            get
            {
                if (_moveCommand == null)
                {
                    _moveCommand = new AsyncReactiveCommand<MoveDirection>();
                    _moveCommand.Subscribe(direction => ExecuteMove(direction).ToObservable().AsUnitObservable());
                }

                return _moveCommand;
            }
        }

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

        /// <summary>
        /// Bounce the player
        /// </summary>
        public AsyncReactiveCommand Bounce
        {
            get
            {
                if (_bounce == null)
                {
                    _bounce = new AsyncReactiveCommand();
                    _bounce.Subscribe(_ => ExecuteBounce().ToObservable().AsUnitObservable());
                }

                return _bounce;
            }
        }

        #endregion
        
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

        public async UniTask StopMove(Vector3 tilePosition)
        {
            _moveTween.Kill();
            _moveTween = null;
            var distance = Vector3.Distance(transform.position, tilePosition);
            _moveTween = transform.DOMove(tilePosition, distance / _gameSettings.playerSpeed);
            await _moveTween.ToUniTask();
        }

        #region Private Methods
        
        /// <summary>
        /// Called when the move command is executed
        /// </summary>
        /// <param name="dir">Direction of the movement</param>
        private async UniTask ExecuteMove(MoveDirection dir)
        {
            if (!CanMove) return;

            //Remove teleported flag
            wasTeleported.Value = false;
            
            //Rotate to the right direction
            transform.rotation = RotationsByDirection[dir];
            
            //If no wall was hit, return
            if (!Physics.Raycast(
                new Ray(transform.position, transform.forward),
                out var hit,
                1000f,
                LayerMask.GetMask("Walls"),
                QueryTriggerInteraction.Ignore)) return;
            
            //Get wall that has been hit
            var wall = hit.collider.gameObject.GetComponentInParent<Wall>();
            if (wall)
            {
                //Get move direction
                var moveDirection = (wall.transform.position - transform.position).normalized;
                //Because we have the wall position for now, we need to subtract the wall size to have the final point for the player to move at
                var destination = wall.transform.position - moveDirection * _gameSettings.blockSize;
                var distance = Vector3.Distance(transform.position, destination);
                //Apply the movement
                _moveTween = transform.DOMove(destination, distance / _gameSettings.playerSpeed);
                await _moveTween.ToUniTask();
            }
        }

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
        private async UniTask ExecuteBounce()
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