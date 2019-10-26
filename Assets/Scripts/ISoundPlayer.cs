using UnityEngine;

namespace RollerSplat
{
    /// <summary>
    /// Interface for sound player
    /// </summary>
    public interface ISoundPlayer
    {
        /// <summary>
        /// Play the given sound
        /// </summary>
        /// <param name="sound">Sound to play</param>
        void PlaySound(AudioSource sound);
    }
}