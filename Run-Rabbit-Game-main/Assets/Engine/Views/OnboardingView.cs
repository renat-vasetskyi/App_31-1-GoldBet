#region

using RabbitGame.Gameplay;
using System;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace RabbitGame.Views
{
    public class OnboardingView : MonoBehaviour
    {
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button[] nextButtons;

        [SerializeField] private GameObject[] pages;
        private int _currentPage;

        private SwipeDetector _swipeDetector;

        public void Initialize(Action OnStartClicked)
        {
            startGameButton.onClick.AddListener(() => OnStartClicked?.Invoke());

            _swipeDetector = new SwipeDetector();
            _swipeDetector.OnSwipe += SwipeDetector_OnSwipe;

            foreach (var nextButton in nextButtons)
            {
                nextButton.onClick.AddListener(LeftSwipe);
            }

            _currentPage = 0;
            UpdatePages();
        }

        private void SwipeDetector_OnSwipe(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
            {
                return;
            }
            if (direction.x > 0)
            {
                RightSwipe();
            }
            else
            {
                LeftSwipe();
            }
        }

        private void RightSwipe() 
        {
            if (_currentPage - 1 < 0)
            {
                return;
            }
            _currentPage--;
            UpdatePages();
        }

        private void LeftSwipe() 
        {
            if (_currentPage + 1 >= pages.Length)
            {
                return;
            }
            _currentPage++;
            UpdatePages();
        }

        private void Update()
        {
            _swipeDetector.Update();
        }

        private void UpdatePages()
        {
            foreach (var page in pages)
            {
                page.SetActive(false);
            }
            pages[_currentPage].SetActive(true);
        }

        private void OnDestroy()
        {
            startGameButton.onClick.RemoveAllListeners();

            foreach (var nextButton in nextButtons)
            {
                nextButton.onClick.RemoveAllListeners();
            }

            if (_swipeDetector != null)
            {
                _swipeDetector.OnSwipe -= SwipeDetector_OnSwipe;
            }
        }
    }
}
