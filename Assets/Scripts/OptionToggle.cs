using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace RollerSplat
{
    [RequireComponent(typeof(Toggle))]
    public class OptionToggle : MonoBehaviour
    {
        private Toggle _toggle;
        public GameObject on;
        public GameObject off;

        public bool IsOn
        {
            get => _toggle.isOn;
            set => _toggle.isOn = value;
        } 
        
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