#region

using RabbitGame.Gameplay;
using System;
using UnityEngine;
using UnityEngine.UI;

# endregion

namespace RabbitGame.Views
{
    public class HowToPlayView : MonoBehaviour
    {
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private GameObject[] pages;
        private int _currentPage;

        private SwipeDetector _swipeDetector;

        public void Initialize(Action OnStartClicked, Action OnCloseClicked)
        {
            startGameButton.onClick.AddListener(() => OnStartClicked?.Invoke());
            closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());

            _swipeDetector = new SwipeDetector();
            _swipeDetector.OnSwipe += SwipeDetector_OnSwipe;

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
                // Right
                if (_currentPage - 1 >= 0)
                {
                    _currentPage--;
                }
            }
            else
            {
                // Left
                if (_currentPage + 1 < pages.Length)
                {
                    _currentPage++;
                }
            }
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
            closeButton.onClick.RemoveAllListeners();

            if (_swipeDetector != null)
            {
                _swipeDetector.OnSwipe -= SwipeDetector_OnSwipe;
            }
        }
    }
}
