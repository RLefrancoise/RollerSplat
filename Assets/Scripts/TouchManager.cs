using System;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;

namespace RollerSplat
{
    /// <summary>
    /// Touch manager using TouchScript
    /// </summary>
    public class TouchManager : MonoBehaviour, ITouchManager
    {
        /// <summary>
        /// Swipe gesture start position. Used to compute swipe gesture
        /// </summary>
        private Vector2 _swipeStartScreenPosition;

        private bool _swipeDetected;
        
        /// <summary>
        /// Swipe gesture listener
        /// </summary>
        [SerializeField] private ScreenTransformGesture swipeGesture;

        [SerializeField] private float swipeThreshold = 0.05f;
        
        public float SwipeThreshold
        {
            get => swipeThreshold;
            set => swipeThreshold = value;
        }
        
        public event Action<SwipeDirection> SwipeDetected;

        private void OnEnable()
        {
            swipeGesture.TransformStarted += StartSwipe;
            swipeGesture.Transformed += SwipeGesture;
        }

        private void OnDisable()
        {
            swipeGesture.TransformStarted -= StartSwipe;
            swipeGesture.Transformed -= SwipeGesture;
        }
        
        /// <summary>
        /// Called when the swipe gesture detection has started
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartSwipe(object sender, EventArgs e)
        {
            _swipeDetected = false;
            _swipeStartScreenPosition = swipeGesture.NormalizedScreenPosition;
        }
        
        /// <summary>
        /// Called when the swipe gesture has ended
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwipeGesture(object sender, EventArgs e)
        {
            if(_swipeDetected) return;

            var screenRatio = (float) Screen.width / Screen.height;
            
            var swipeLength = swipeGesture.NormalizedScreenPosition - _swipeStartScreenPosition;
            if(Mathf.Abs(swipeLength.x) < SwipeThreshold && Mathf.Abs(swipeLength.y) <= SwipeThreshold * screenRatio) return;

            _swipeDetected = true;
            
            if(Mathf.Abs(swipeLength.x) > Mathf.Abs(swipeLength.y))
            {
                if (swipeLength.x >= SwipeThreshold)
                {
                    SwipeDetected?.Invoke(SwipeDirection.Right);
                }
                else if (swipeLength.x <= -SwipeThreshold)
                {
                    SwipeDetected?.Invoke(SwipeDirection.Left);
                }
            }
            else
            {
                if (swipeLength.y >= SwipeThreshold * screenRatio)
                {
                    SwipeDetected?.Invoke(SwipeDirection.Up);
                }
                else if (swipeLength.y <= -SwipeThreshold * screenRatio)
                {
                    SwipeDetected?.Invoke(SwipeDirection.Down);
                }
            }
        }
    }
}