using RollerSplat.Data;
using UnityEngine;
using Zenject;
using UniRx;

namespace RollerSplat
{
    public class Level : MonoBehaviour
    {
        private Camera _levelCamera;
        private ReactiveCommand<LevelData> _loadCommand;

        public LevelData Data { get; private set; }

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
        public void Construct(Camera levelCamera)
        {
            _levelCamera = levelCamera;
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
        }
    }
}