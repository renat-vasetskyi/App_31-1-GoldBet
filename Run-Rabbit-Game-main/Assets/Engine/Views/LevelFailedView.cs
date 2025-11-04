#region

using RabbitGame.Services;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace RabbitGame.Views
{
    public class LevelFailedView : MonoBehaviour
    {
        [SerializeField] private Button tryAgainButton;

        [SerializeField] private TMP_Text roundScoreTextMesh;
        [SerializeField] private TMP_Text progressTextMesh;

        public void Initialize(Action OnTryAgainClicked, IScoreService scoreService)
        {
            tryAgainButton.onClick.AddListener(() => OnTryAgainClicked?.Invoke());

            int progressInPercent = Mathf.FloorToInt((scoreService.CurrentRoundScore / (float)ScoreService.ScoreToWin) * 100f);
            UpdateTexts(scoreService.CurrentRoundScore, progressInPercent);
        }

        private void UpdateTexts(int roundScore, int progressInPercent)
        {
            roundScoreTextMesh.text = roundScore.ToString();
            progressTextMesh.text = progressInPercent + "%";
        }

        private void OnDestroy()
        {
            tryAgainButton.onClick.RemoveAllListeners();
        }
    }
}
