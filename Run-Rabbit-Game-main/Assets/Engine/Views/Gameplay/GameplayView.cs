#region 

using RabbitGame.Gameplay;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RabbitGame.Services;

#endregion

namespace RabbitGame.Views
{
    public class GameplayView : MonoBehaviour
    {
        [SerializeField] private GameObject moveButtons;
        [SerializeField] private MoveButtonView rightButton;
        [SerializeField] private MoveButtonView leftButton;

        [SerializeField] private GameObject startButtons;
        [SerializeField] private Button startButton;
        [SerializeField] private Button infoButton;

        [SerializeField] private TMP_Text levelScoreTextMesh;
        [SerializeField] private TMP_Text totalScoreTextMesh;

        [Header("HP")]
        [SerializeField] private Image[] heartImages;
        [SerializeField] private Sprite fullHeartSprite;
        [SerializeField] private Sprite emptyHeartSprite;

        private IScoreService _scoreService;
        private PlayerHealth _playerHealth;

        public void Initialize(PlayerInput input, PlayerHealth health, Action OnStartClicked, Action OnInfoClicked, IScoreService scoreService)
        {
            _scoreService = scoreService;
            _scoreService.OnScoreChanged += UpdateScoreTexts;
            UpdateScoreTexts(_scoreService.CurrentRoundScore, _scoreService.TotalScore);

            _playerHealth = health;
            _playerHealth.OnHealthChanged += UpdateHP;
            UpdateHP(_playerHealth.CurrentHealth);

            rightButton.Initialize(input);
            leftButton.Initialize(input);

            startButton.onClick.AddListener(() => 
            {
                StartLevel();
                OnStartClicked?.Invoke();
            });
            infoButton.onClick.AddListener(() => OnInfoClicked?.Invoke());

            StopLevel();
        }

        private void UpdateScoreTexts(int roundScore, int totalScore) 
        {
            levelScoreTextMesh.text = roundScore.ToString();
            totalScoreTextMesh.text = totalScore.ToString();
        }

        private void UpdateHP(int hitpoints) 
        {
            int fullHearts = hitpoints;
            if (hitpoints > heartImages.Length)
            {
                fullHearts = heartImages.Length;
            }
            foreach (var heartImage in heartImages)
            {
                heartImage.sprite = emptyHeartSprite;
            }
            for (int i = 0; i < fullHearts; i++)
            {
                heartImages[i].sprite = fullHeartSprite;
            }
        }

        private void StartLevel() 
        {
            startButtons.SetActive(false);
            moveButtons.SetActive(true);
        }

        private void StopLevel() 
        {
            startButtons.SetActive(true);
            moveButtons.SetActive(false);
        }

        private void OnDestroy()
        {
            startButton.onClick.RemoveAllListeners();
            infoButton.onClick.RemoveAllListeners();

            if (_scoreService != null)
            {
                _scoreService.OnScoreChanged -= UpdateScoreTexts;
            }
        }
    }
}
