using System.Collections.Generic;
using RollerSplat.Data;
using UniRx;
using UnityEngine;
using Zenject;

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
        private Renderer _border;

        private Animator _animator;
        private IEnumerable<Renderer> _paintPlayerRenderer;

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
        private static readonly int PaintPlayer = Animator.StringToHash("PaintPlayer");

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

        [Inject]
        public void Construct(
            [Inject(Id = "Border")] Renderer border, 
            Animator animator, 
            IEnumerable<Renderer> paintPlayerRenderer)
        {
            _border = border;
            _animator = animator;
            _paintPlayerRenderer = paintPlayerRenderer;
        }
        
        private void Start()
        {
            color.Subscribe(ListenColorChanged);
            isPaintingPlayer.Subscribe(ListenIsPaintingPlayer);
            
            color.Value = GameSettings.defaultGroundColor;
        }

        private void Update()
        {
            _border.material.SetColor(EmissionColor, expectedColor.Value * Mathf.PingPong(Time.time, 1f));

            if (isPaintingPlayer.Value)
            {
                foreach (var paintPlayerRenderer in _paintPlayerRenderer)
                {
                    paintPlayerRenderer.material.SetColor(EmissionColor, expectedColor.Value * Mathf.PingPong(Time.time, 1f));
                }
            }
        }
        
        private void OnTriggerStay(Collider other)
        {
            //If player rolls on tile, apply player color to tile
            if (other.CompareTag("Player"))
            {
                if (isPaintingPlayer.Value) other.GetComponent<Player>().Color.Value = expectedColor.Value;
                
                color.Value = other.GetComponent<Player>().Color.Value;
                isPaintedByPlayer.Value = true;
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
            Renderer.material.color = c;
        }

        private void ListenIsPaintingPlayer(bool painting)
        {
            _animator.SetBool(PaintPlayer, painting);
        }
        
        #endregion
    }
}