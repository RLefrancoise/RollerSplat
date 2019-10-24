﻿using System;
using DG.Tweening;
using RollerSplat.Data;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityQuery;

namespace RollerSplat
{
    /// <summary>
    /// A ground tile. It changes color when the player is rolling on it
    /// </summary>
    public class GroundTile : LevelBlock
    {
        #region Fields

        /// <summary>
        /// Border renderer
        /// </summary>
        [SerializeField] private Renderer border;
        /// <summary>
        /// Splat renderer
        /// </summary>
        [SerializeField] private Renderer splat;
        /// <summary>
        /// Paint splash effect
        /// </summary>
        [SerializeField] private ParticleSystem paintSplash;
        /// <summary>
        /// Is tile painted by player ?
        /// </summary>
        [SerializeField] private BoolReactiveProperty isPaintedByPlayer;
        /// <summary>
        /// Tile color
        /// </summary>
        [SerializeField] private ColorReactiveProperty color;
        /// <summary>
        /// Expected color
        /// </summary>
        [SerializeField] private ColorReactiveProperty expectedColor;
        /// <summary>
        /// Is this tile painting the player ?
        /// </summary>
        [SerializeField] private BoolReactiveProperty isPaintingPlayer;
        
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        #endregion

        #region Properties

        public override LevelData.CellType CellType => LevelData.CellType.Ground;

        /// <summary>
        /// Is tile painted by player ?
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsPaintedByPlayer => isPaintedByPlayer.CombineLatest(color,
            (painted, c) => painted && color.Value == expectedColor.Value).ToReadOnlyReactiveProperty();

        #endregion

        #region Monobehaviour Callbacks
        
        private void Start()
        {
            isPaintedByPlayer.Subscribe(ListenIsPaintedByPlayer);
            color.Subscribe(ListenColorChanged);
            expectedColor.Subscribe(ListenExpectedColorChanged);
            isPaintingPlayer.Subscribe(ListenIsPaintingPlayer);
            
            if(!isPaintedByPlayer.Value) color.Value = GameSettings.defaultGroundColor;
        }

        private void Update()
        {
            border.material.color = expectedColor.Value;
            border.material.SetColor(EmissionColor, expectedColor.Value * Mathf.PingPong(Time.time, 1f));
        }
        
        private void OnTriggerEnter(Collider other)
        {
            //If player rolls on tile, apply player color to tile
            if (other.CompareTag("Player"))
            {
                if (isPaintingPlayer.Value) other.GetComponent<Player>().Color.Value = expectedColor.Value;
                
                color.Value = other.GetComponent<Player>().Color.Value;
                isPaintedByPlayer.Value = true;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = expectedColor.Value.WithAlpha(0.5f);
            Gizmos.DrawCube(transform.position + Vector3.up * 0.01f, renderer.bounds.size);
            
            if (isPaintingPlayer.Value)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(transform.position + Vector3.up * renderer.bounds.extents.magnitude / 2f, renderer.bounds.extents.magnitude / 2f);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Called when tile color is changed
        /// </summary>
        /// <param name="c">New tile color</param>
        private void ListenColorChanged(Color c)
        {
            renderer.material.DOColor(c, GameSettings.groundColorationDuration);
            var colorOverLifetime = paintSplash.colorOverLifetime;
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(new Gradient()
            {
                colorKeys = new[] {new GradientColorKey(c, 0f), new GradientColorKey(c, 1f) },
                alphaKeys = new[] {new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
            });
        }

        /// <summary>
        /// Called when expected color is changed
        /// </summary>
        /// <param name="c"></param>
        private void ListenExpectedColorChanged(Color c)
        {
            splat.material.color = c;
            splat.material.SetColor(EmissionColor, c);
        }
        
        /// <summary>
        /// Called when is painting player flag is changed
        /// </summary>
        /// <param name="painting"></param>
        private void ListenIsPaintingPlayer(bool painting)
        {
            splat.gameObject.SetActive(painting);
        }

        /// <summary>
        /// Called when is painted by player flag is changed
        /// </summary>
        /// <param name="painted"></param>
        private void ListenIsPaintedByPlayer(bool painted)
        {   
            paintSplash.gameObject.SetActive(painted);
        }
        
        #endregion
    }
}