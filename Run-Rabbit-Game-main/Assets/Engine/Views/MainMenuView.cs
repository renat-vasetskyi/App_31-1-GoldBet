#region

using System;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace RabbitGame.Views
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button infoButton;

        public void Initialize(Action OnPlayClicked, Action OnInfoClicked)
        {
            playButton.onClick.AddListener(() => OnPlayClicked?.Invoke());
            infoButton.onClick.AddListener(() => OnInfoClicked?.Invoke());
        }

        private void OnDestroy()
        {
            playButton.onClick.RemoveAllListeners();
            infoButton.onClick.RemoveAllListeners();
        }
    }
}
