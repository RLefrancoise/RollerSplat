using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
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
        private static readonly Dictionary<KeyCode, Quaternion> RotationsFromInput = new Dictionary<KeyCode, Quaternion>
        {
            {KeyCode.UpArrow, Quaternion.LookRotation(Vector3.forward, Vector3.up)},
            {KeyCode.DownArrow, Quaternion.LookRotation(-Vector3.forward, Vector3.up)},
            {KeyCode.RightArrow, Quaternion.LookRotation(Vector3.right, Vector3.up)},
            {KeyCode.LeftArrow, Quaternion.LookRotation(-Vector3.right, Vector3.up)},
        };
        
        private GameSettings _gameSettings;
        private Renderer _renderer;
        private Rigidbody _rigidBody;
        [SerializeField] private ColorReactiveProperty color;

        private TweenerCore<Vector3, Vector3, VectorOptions> _moveTween;
        
        public ColorReactiveProperty Color => color;
        
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

        private void Update()
        {
            //If player is already moving, don't do anything
            if(_moveTween != null && _moveTween.active) return;
            
            var move = false;

            //Check if any player move key is pressed
            foreach (var key in RotationsFromInput.Keys)
            {
                if (!Input.GetKeyDown(key)) continue;
                
                move = true;
                transform.rotation = RotationsFromInput[key];
                break;
            }

            //If no move, return
            if (!move) return;

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
                //Because we have the wall position for now, we need to substract half of the wall size to have the final point for the player to move at
                var destination = wall.transform.position - moveDirection * wall.Extents.magnitude;
                //Apply the movement
                _moveTween = _rigidBody.DOMove(destination, _gameSettings.playerMoveTime);
            }
        }

        
    }
}