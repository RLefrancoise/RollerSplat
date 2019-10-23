using System.Collections.Generic;
using System.Linq;
using Lean.Pool;
using RollerSplat.Data;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;
using Zenject;
using UniRx;

namespace RollerSplat
{
    /// <summary>
    /// The game level
    /// </summary>
    public class Level : MonoBehaviour
    {
        #region Fields
        
        /// <summary>
        /// Game settings
        /// </summary>
        private GameSettings _gameSettings;
        
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
                    //_loadCommand.Subscribe(ExecuteLoad);
                    _loadCommand.Subscribe(ExecuteLoadPrefab);
                }

                return _loadCommand;
            }
        }
        
        #endregion

        [Inject]
        public void Construct(GameSettings gameSettings, Camera levelCamera)
        {
            _gameSettings = gameSettings;
            _levelCamera = levelCamera;
            
            _isLevelComplete = new BoolReactiveProperty();
            
            Blocks = new ReactiveCollection<LevelBlock>();
            Blocks.ObserveAdd().Subscribe(ListenBlockAdded);
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

        private void ExecuteLoadPrefab(LevelData levelData)
        {
            Data = levelData;
            
            if (levelData == null)
            {
                Debug.LogErrorFormat("Level:ExecuteLoad - Level data is null");
                return;
            }

            _levelCamera.transform.position = levelData.cameraPosition;
            _levelCamera.transform.rotation = Quaternion.Euler(levelData.cameraRotation);

            //Clear previous level content
            Blocks.Clear();

            for (var i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i);
                if(child.GetComponent<Player>()) continue;
                
                Destroy(child.gameObject);
            }
            
            //Instantiate new level structure
            var level = Instantiate(levelData.levelPrefab, Vector3.zero, Quaternion.identity, transform);
            foreach (var levelBlock in level.GetComponentsInChildren<LevelBlock>())
            {
                Blocks.Add(levelBlock);
            }
        }
        
        /// <summary>
        /// Called when load command is executed
        /// </summary>
        /// <param name="levelData">Level data to load</param>
        private void ExecuteLoad(LevelData levelData)
        {
            Data = levelData;
            
            if (levelData == null)
            {
                Debug.LogErrorFormat("Level:ExecuteLoad - Level data is null");
                return;
            }

            _levelCamera.transform.position = levelData.cameraPosition;
            _levelCamera.transform.rotation = Quaternion.Euler(levelData.cameraRotation);
            
            //read level file
            var levelContent = levelData.levelFile.text.Trim().Split('\n');
            //level size
            var levelSize = new Vector2(levelContent[0].Trim().Length, levelContent.Length);
            
            var currentColumn = 0;
            var currentRow = 0;

            //assign each level file letter to the right prefab
            var levelPrefabs = new Dictionary<char, GameObject>
            {
                {'G', levelData.cells.First(c => c.type == LevelData.CellType.Ground).prefab},
                {'W', levelData.cells.First(c => c.type == LevelData.CellType.Wall).prefab},
            };
            
            //Clear blocks list
            foreach (var block in Blocks)
            {
                Destroy(block.Root.gameObject);
            }
            Blocks.Clear();
            
            //for each column
            foreach (var column in levelContent)
            {
                //for each row
                foreach (var cell in column.Trim())
                {
                    //instantiate the level block
                    var levelBlock = Instantiate(
                        levelPrefabs[cell], 
                        transform.position + (-Vector3.forward * currentColumn * _gameSettings.blockSize) + (Vector3.right * currentRow * _gameSettings.blockSize), 
                        Quaternion.identity,
                        transform).GetComponentInChildren<LevelBlock>();
                    
                    //apply the right scale to the level block
                    levelBlock.Root.localScale = Vector3.one * _gameSettings.blockSize;

                    //add the block to the list of blocks of the level
                    Blocks.Add(levelBlock);
                    
                    currentRow++;
                }

                currentRow = 0;
                currentColumn++;
            }
            
            //Adjust the level position according to its size to center it on the screen
            transform.position = (Vector3.forward * Mathf.FloorToInt(levelSize.y / 2f)) 
                                 - (Vector3.right * (levelSize.x - 1f) / 2f);
        }
    }
}