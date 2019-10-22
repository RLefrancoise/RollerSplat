using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using NaughtyAttributes;
using UniRx;
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
        /// Player move tween
        /// </summary>
        private TweenerCore<Vector3, Vector3, VectorOptions> _moveTween;
        /// <summary>
        /// Move command
        /// </summary>
        private ReactiveCommand<MoveDirection> _moveCommand;
        /// <summary>
        /// Place on tile command
        /// </summary>
        private ReactiveCommand<Vector2> _placeOnTile;
        /// <summary>
        /// Player current color
        /// </summary>
        [Tooltip("Player current color")]
        [SerializeField] private ColorReactiveProperty color;

        #endregion

        #region Properties
        
        /// <summary>
        /// Can the player move ?
        /// </summary>
        [ShowNativeProperty] public bool CanMove => _moveTween == null || !_moveTween.active;
        
        /// <summary>
        /// Current player color
        /// </summary>
        public Color Color => color.Value;

        /// <summary>
        /// Move the player in the given direction
        /// </summary>
        public ReactiveCommand<MoveDirection> Move
        {
            get
            {
                if (_moveCommand == null)
                {
                    _moveCommand = new ReactiveCommand<MoveDirection>();
                    _moveCommand.Subscribe(ExecuteMove);
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

        #endregion
        
        [Inject]
        public void Construct(
            GameSettings gameSettings, 
            Renderer r, 
            Rigidbody rigidBody)
        {
            _gameSettings = gameSettings;
            _renderer = r;
            _rigidBody = rigidBody;
            _renderer.material.color = color.Value;
        }

        #region Private Methods
        
        /// <summary>
        /// Called when the move command is executed
        /// </summary>
        /// <param name="dir">Direction of the movement</param>
        private void ExecuteMove(MoveDirection dir)
        {
            if(!CanMove) return;
            
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
            var wall = hit.collider.gameObject.GetComponent<Wall>();
            if (wall)
            {
                //Get move direction
                var moveDirection = (wall.transform.position - transform.position).normalized;
                //Because we have the wall position for now, we need to subtract half of the wall size to have the final point for the player to move at
                var destination = wall.transform.position - moveDirection * _gameSettings.blockSize / 2f;
                var distance = Vector3.Distance(transform.position, destination);
                //Apply the movement
                _moveTween = _rigidBody.DOMove(destination, distance / _gameSettings.playerSpeed);
            }
        }

        /// <summary>
        /// Called when the place on tile command is executed
        /// </summary>
        /// <param name="tile">Tile position</param>
        private void ExecutePlaceOnTile(Vector2 tile)
        {
            transform.localPosition = Vector3.right * tile.x * _gameSettings.blockSize +
                                      Vector3.forward * tile.y * _gameSettings.blockSize +
                                      Vector3.up * _gameSettings.blockSize / 2f;
            _rigidBody.position = transform.position;
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
        }
        
        #endregion
    }
}