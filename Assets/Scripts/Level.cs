using System.Linq;
using RollerSplat.Data;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.SceneManagement;

namespace RollerSplat
{
    /// <summary>
    /// The game level
    /// </summary>
    public class Level : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// Sound player
        /// </summary>
        private ISoundPlayer _soundPlayer;
        
        /// <summary>
        /// The camera used to display the level
        /// </summary>
        private Camera _levelCamera;
        
        /// <summary>
        /// Load command. It loads the given level
        /// </summary>
        private ReactiveCommand<LevelData> _loadCommand;

        /// <summary>
        /// Is level complete ?
        /// </summary>
        private BoolReactiveProperty _isLevelComplete;

        /// <summary>
        /// Background music
        /// </summary>
        [SerializeField] private AudioSource backgroundMusic;
        
        #endregion

        #region Properties
        
        /// <summary>
        /// All the blocks of the level (Walls, Ground, ...)
        /// </summary>
        public ReactiveCollection<LevelBlock> Blocks { get; private set; }

        /// <summary>
        /// Is level complete ?
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsLevelComplete => _isLevelComplete.ToReadOnlyReactiveProperty();
        
        /// <summary>
        /// Data of the current level
        /// </summary>
        public LevelData Data { get; private set; }
        
        /// <summary>
        /// Last ground tile that has been painted by the player
        /// </summary>
        public GroundTile LastGroundTilePaintedByPlayer { get; private set; }
        
        /// <summary>
        /// Load command. It loads the given level
        /// </summary>
        public ReactiveCommand<LevelData> Load
        {
            get
            {
                if (_loadCommand == null)
                {
                    _loadCommand = new ReactiveCommand<LevelData>();
                    _loadCommand.Subscribe(ExecuteLoad);
                }

                return _loadCommand;
            }
        }
        
        #endregion

        [Inject]
        public void Construct(Camera levelCamera, ISoundPlayer soundPlayer)
        {
            _levelCamera = levelCamera;
            _soundPlayer = soundPlayer;
            
            _isLevelComplete = new BoolReactiveProperty();
            
            Blocks = new ReactiveCollection<LevelBlock>();
            Blocks.ObserveAdd().Subscribe(ListenBlockAdded);
        }

        /// <summary>
        /// Play background music
        /// </summary>
        /// <param name="play"></param>
        public void PlayBackgroundMusic(bool play)
        {
            if(play) _soundPlayer.PlaySound(backgroundMusic);
            else backgroundMusic.Stop();
        }
        
        /// <summary>
        /// Called when a level block is added to the level
        /// </summary>
        /// <param name="addEvent">Add block event</param>
        private void ListenBlockAdded(CollectionAddEvent<LevelBlock> addEvent)
        {
            var block = addEvent.Value;
            switch (block.CellType)
            {
                case LevelData.CellType.Wall:
                case LevelData.CellType.Teleport:
                    break;
                case LevelData.CellType.Ground:
                    var groundBlock = (GroundTile) block;
                    groundBlock.IsPaintedByPlayer.Subscribe(painted =>
                    {
                        if (painted) LastGroundTilePaintedByPlayer = groundBlock;
                    });
                    groundBlock.IsPaintedByPlayer.Subscribe(ListenGroundBlockPaintedByPlayer);
                    break;
            }
        }
        
        /// <summary>
        /// Called when a ground block painted flag has changed
        /// </summary>
        /// <param name="painted">Is painted</param>
        private void ListenGroundBlockPaintedByPlayer(bool painted)
        {
            if (painted)
            {
                _isLevelComplete.SetValueAndForceNotify(Blocks.Where(b => b.CellType == LevelData.CellType.Ground)
                    .Select(b => (GroundTile) b)
                    .All(g => g.IsPaintedByPlayer.Value));
            }
            else
            {
                _isLevelComplete.SetValueAndForceNotify(false);
            }
        }

        /// <summary>
        /// Called when load is executed
        /// </summary>
        /// <param name="levelData">Data of the level to load</param>
        private void ExecuteLoad(LevelData levelData)
        {   
            Data = levelData;
            
            if (levelData == null)
            {
                Debug.LogErrorFormat("Level:ExecuteLoad - Level data is null");
                return;
            }

            //Clear previous level content
            Blocks.Clear();

            for (var i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i);
                if(child.GetComponent<Player>()) continue;
                
                Destroy(child.gameObject);
            }

            _levelCamera.gameObject.SetActive(true);
            _levelCamera.transform.position = levelData.cameraPosition;
            _levelCamera.transform.rotation = Quaternion.Euler(levelData.cameraRotation);
            
            //Instantiate new level structure
            var level = Instantiate(levelData.levelPrefab, Vector3.zero, Quaternion.identity, transform);
            foreach (var levelBlock in level.GetComponentsInChildren<LevelBlock>())
            {
                Blocks.Add(levelBlock);
            }            
        }
    }
}