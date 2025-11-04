#region

using DG.Tweening;
using System;
using UnityEngine;

#endregion

namespace RabbitGame.Views
{
    public class SplashScreenView : MonoBehaviour
    {
        [SerializeField] private Transform splashLogo;
        [SerializeField] private float animationDuration = 1f;
        [SerializeField] private float delayAfter = 1f;
        [SerializeField] private Vector3 startScale;
        [SerializeField] private Vector3 endScale;

        public void PlaySplashAnimation(Action onComplete = null) 
        {
            splashLogo.localScale = startScale;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(splashLogo.transform.DOScale(endScale, animationDuration).SetEase(Ease.OutCubic));
            sequence.AppendInterval(delayAfter);
            sequence.OnComplete(() => { onComplete?.Invoke(); });
        }
    }
}
