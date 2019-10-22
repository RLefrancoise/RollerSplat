using System;
using UniRx;

namespace RollerSplat.Data
{
    /// <summary>
    /// Reactive property for level data
    /// </summary>
    [Serializable]
    public class LevelDataReactiveProperty : ReactiveProperty<LevelData>
    {
        public LevelDataReactiveProperty() {}
        public LevelDataReactiveProperty(LevelData initialValue) : base(initialValue) {}
    }
}