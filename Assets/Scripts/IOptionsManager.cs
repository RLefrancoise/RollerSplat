namespace RollerSplat
{
    /// <summary>
    /// Interface for options manager
    /// </summary>
    public interface IOptionsManager
    {
        /// <summary>
        /// Options data
        /// </summary>
        OptionsData Options { get; }
        
        /// <summary>
        /// Has options data been saved already ?
        /// </summary>
        bool HasSavedOptions { get; }
        
        /// <summary>
        /// Save options
        /// </summary>
        /// <returns>True if saved successfully, false otherwise</returns>
        bool SaveOptions();
        /// <summary>
        /// Load options
        /// </summary>
        /// <returns>True if loaded successfully, false otherwise</returns>
        bool LoadOptions();
    }
}