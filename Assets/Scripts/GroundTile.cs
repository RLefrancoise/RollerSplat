using RollerSplat.Data;
using UniRx;
using UnityEngine;

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
        /// Tile animator
        /// </summary>
        [SerializeField] private Animator animator;
        /// <summary>
        /// Renderers for paint player animation
        /// </summary>
        [SerializeField] private Renderer[] paintPlayerRenderers;
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
        
        private void Start()
        {
            color.Subscribe(ListenColorChanged);
            isPaintingPlayer.Subscribe(ListenIsPaintingPlayer);
            
            color.Value = GameSettings.defaultGroundColor;
        }

        private void Update()
        {
            border.material.SetColor(EmissionColor, expectedColor.Value * Mathf.PingPong(Time.time, 1f));

            if (isPaintingPlayer.Value)
            {
                foreach (var paintPlayerRenderer in paintPlayerRenderers)
                {
                    paintPlayerRenderer.material.SetColor(EmissionColor, expectedColor.Value * Mathf.PingPong(Time.time, 1f));
                }
            }
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
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Called when tile color is changed
        /// </summary>
        /// <param name="c">New tile color</param>
        private void ListenColorChanged(Color c)
        {
            renderer.material.color = c;
        }

        private void ListenIsPaintingPlayer(bool painting)
        {
            animator.SetBool(PaintPlayer, painting);
        }
        
        #endregion
    }
}