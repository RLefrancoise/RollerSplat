using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UniRx.Async;

namespace RollerSplat
{
    public static class TweenExtensions
    {
        public static UniTask ToUniTask<T1, T2, TOptions>(this TweenerCore<T1, T2, TOptions> tween) where TOptions : struct, IPlugOptions
        {
            var completed = false;
            tween.onComplete += () => completed = true;
            return UniTask.WaitUntil(() => completed);
        }
    }
}