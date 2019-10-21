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
        [SerializeField] private LevelDataReactiveProperty data;

        public LevelData Data => data.Value;

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
            data.Subscribe(ListenLevelDataChanged);
        }

        private void Start()
        {
            //If data are set, load them
            if (Data != null) Load.Execute(Data);
        }

        private void ListenLevelDataChanged(LevelData levelData)
        {
            if (levelData == null)
            {
                Debug.LogErrorFormat("Level:ListenLevelDataChanged - Level data is null");
                return;
            }

            _levelCamera.transform.position = levelData.cameraPosition;
            _levelCamera.transform.rotation = Quaternion.Euler(levelData.cameraRotation);
        }

        private void ExecuteLoad(LevelData levelData)
        {
            data.Value = levelData;
        }
    }
}