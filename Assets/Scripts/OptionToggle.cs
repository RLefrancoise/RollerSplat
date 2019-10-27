using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace RollerSplat
{
    /// <summary>
    /// Option toggle button
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class OptionToggle : MonoBehaviour
    {
        /// <summary>
        /// UI Toggle
        /// </summary>
        private Toggle _toggle;
        /// <summary>
        /// Toggle on image
        /// </summary>
        public GameObject on;
        /// <summary>
        /// Toggle off image
        /// </summary>
        public GameObject off;

        /// <summary>
        /// Is toggle on ?
        /// </summary>
        public bool IsOn
        {
            get => _toggle.isOn;
            set => _toggle.isOn = value;
        } 
        
        /// <summary>
        /// Called when toggle value is changed
        /// </summary>
        public event Action<bool> OptionToggled; 
        
        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.OnValueChangedAsObservable().Subscribe(isOn =>
            {
                on.SetActive(isOn);
                off.SetActive(!isOn);
                OptionToggled?.Invoke(isOn);
            });
        }
    }
}