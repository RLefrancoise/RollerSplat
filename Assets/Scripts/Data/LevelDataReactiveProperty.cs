using System;
using UniRx;

namespace RollerSplat.Data
{
    [Serializable]
    public class LevelDataReactiveProperty : ReactiveProperty<LevelData>
    {
        public LevelDataReactiveProperty() {}
        public LevelDataReactiveProperty(LevelData initialValue) : base(initialValue) {}
    }
}