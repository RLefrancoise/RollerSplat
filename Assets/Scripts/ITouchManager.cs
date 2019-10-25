using System;

namespace RollerSplat
{
    /// <summary>
    /// Interface for touch manager
    /// </summary>
    public interface ITouchManager
    {
        /// <summary>
        /// Threshold for swipe detection. It it between 0 and 1 and is a percentage of the screen width
        /// </summary>
        float SwipeThreshold { get; set; }
        
        /// <summary>
        /// Called when a swipe is detected
        /// </summary>
        event Action<SwipeDirection> SwipeDetected;
    }
    
    /// <summary>
    /// Swipe direction
    /// </summary>
    public enum SwipeDirection
    {
        Left, Right, Up, Down
    }
}