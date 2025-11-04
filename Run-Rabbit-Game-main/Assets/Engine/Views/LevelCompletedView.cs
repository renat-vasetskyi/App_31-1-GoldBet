#region 

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RabbitGame.Services;

#endregion

namespace RabbitGame.Views
{
    public class LevelCompletedView : MonoBehaviour
    {
        [SerializeField] private Button nextButton;

        [SerializeField] private TMP_Text roundScoreTextMesh;
        [SerializeField] private TMP_Text totalScoreTextMesh;

        public void Initialize(Action OnNextClicked, IScoreService scoreService)
        {
            nextButton.onClick.AddListener(() => OnNextClicked?.Invoke());

            UpdateTexts(scoreService.CurrentRoundScore, scoreService.TotalScore);
        }

        private void UpdateTexts(int roundScore, int totalScore) 
        {
            roundScoreTextMesh.text = roundScore.ToString();
            totalScoreTextMesh.text = totalScore.ToString();
        }

        private void OnDestroy()
        {
            nextButton.onClick.RemoveAllListeners();
        }
    }
}
