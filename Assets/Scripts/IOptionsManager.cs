using UniRx.Async;

namespace RollerSplat
{
    public interface IOptionsManager
    {
        OptionsData Options { get; }
        
        bool HasSavedOptions { get; }
        
        bool SaveOptions();
        bool LoadOptions();
    }
}