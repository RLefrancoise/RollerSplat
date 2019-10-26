using UnityEngine;
using Zenject;

namespace RollerSplat
{
    /// <summary>
    /// Sound player
    /// </summary>
    public class SoundPlayer : MonoBehaviour, ISoundPlayer
    {
        /// <summary>
        /// Options manager
        /// </summary>
        private IOptionsManager _optionsManager;

        [Inject]
        public void Construct(IOptionsManager optionsManager)
        {
            _optionsManager = optionsManager;
        }
        
        public void PlaySound(AudioSource sound)
        {
            if (!_optionsManager.Options.Sound) return;
            sound.Play();
        }
    }
}