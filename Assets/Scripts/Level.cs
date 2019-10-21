using System.Collections.Generic;
using System.Linq;
using Lean.Pool;
using RollerSplat.Data;
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
        private GameSettings _gameSettings;
        /// <summary>
        /// The camera used to display the level
        /// </summary>
        private Camera _levelCamera;

        private Player _player;
        /// <summary>
        /// Load command. It loads the given level
        /// </summary>
        private ReactiveCommand<LevelData> _loadCommand;

        /// <summary>
        /// Data of the current level
        /// </summary>
        public LevelData Data { get; private set; }
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

        [Inject]
        public void Construct(GameSettings gameSettings, Camera levelCamera, Player player)
        {
            _gameSettings = gameSettings;
            _levelCamera = levelCamera;
            _player = player;
        }

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
            
            //for each line
            var currentColumn = 0;
            var currentRow = 0;

            var levelPrefabs = new Dictionary<char, GameObject>
            {
                {'G', levelData.cells.First(c => c.type == LevelData.CellType.Ground).prefab},
                {'W', levelData.cells.First(c => c.type == LevelData.CellType.Wall).prefab},
            };
            
            foreach (var column in levelContent)
            {
                foreach (var cell in column.Trim())
                {
                    var tile = LeanPool.Spawn(
                        levelPrefabs[cell], 
                        transform.position + (Vector3.forward * currentColumn * _gameSettings.blockSize) + (Vector3.right * currentRow * _gameSettings.blockSize), 
                        Quaternion.identity);
                    tile.transform.localScale = Vector3.one * _gameSettings.blockSize;
                    tile.transform.SetParent(transform);

                    currentRow++;
                }

                currentRow = 0;
                currentColumn++;
            }
            
            //Adjust the level position according to its size to center it on the screen
            transform.position = transform.position - (Vector3.forward * Mathf.FloorToInt(levelData.size.y / 2f)) - (Vector3.right * Mathf.FloorToInt(levelData.size.x / 2f));
            
            //Place player 
            _player.PlaceOnTile.Execute(levelData.startPosition);
        }
    }
}