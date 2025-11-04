#region

using System;
using UnityEngine;

#endregion

namespace RabbitGame.Gameplay
{
    public class SwipeDetector
    {
        public event Action<Vector2> OnSwipe;

        private float _minSwipeDistance = 50f;

        private Vector2 _startPosition;
        private Vector2 _endPosition;
        private bool _isSwiping = false;

        public SwipeDetector(float minSwipeDistance = 50f)
        {
            _minSwipeDistance = minSwipeDistance;
        }

        public void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouse();
#elif UNITY_ANDROID || UNITY_IOS
            HandleTouch();
#endif
        }

        private void HandleMouse()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _startPosition = Input.mousePosition;
                _isSwiping = true;
            }
            else if (Input.GetMouseButtonUp(0) && _isSwiping)
            {
                _endPosition = Input.mousePosition;
                DetectSwipe(_startPosition, _endPosition);
                _isSwiping = false;
            }
        }

        private void HandleTouch()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    _startPosition = touch.position;
                    _isSwiping = true;
                }
                else if (touch.phase == TouchPhase.Ended && _isSwiping)
                {
                    _endPosition = touch.position;
                    DetectSwipe(_startPosition, _endPosition);
                    _isSwiping = false;
                }
            }
        }

        private void DetectSwipe(Vector2 startPosition ,Vector2 endPosition)
        {
            if (Vector2.Distance(startPosition, endPosition) < _minSwipeDistance)
                return;

            Vector2 direction = (endPosition - startPosition).normalized;
            OnSwipe?.Invoke(direction);
        }
    }
}
