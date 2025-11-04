#region

using DG.Tweening;
using UnityEngine;

#endregion

namespace RabbitGame.Gameplay
{
    public class LoseHPEffect
    {
        private GameObject _background;
        private Transform _explosion;

        private PlayerHealth _playerHealth;

        public LoseHPEffect(GameObject background, GameObject explosion, PlayerHealth playerHealth)
        {
            _background = background;
            _explosion = explosion.transform;

            _playerHealth = playerHealth;
            playerHealth.OnHealthChanged += PlayerHealth_OnHealthChanged;

            _background.SetActive(false);
            _explosion.gameObject.SetActive(false);
        }

        private void PlayerHealth_OnHealthChanged(int currentHP)
        {
            Play();
        }

        private void Play()
        {
            _background.SetActive(true);

            Vector3 explosionStartScale = _explosion.localScale;
            _explosion.localScale = Vector3.zero;
            _explosion.localRotation = Quaternion.Euler(0, 0, 90);
            _explosion.gameObject.SetActive(true);

            Sequence sequence = DOTween.Sequence();

            sequence
                .Join(_explosion.DOScale(explosionStartScale.x, 0.4f).SetEase(Ease.OutBack))
                .Join(_explosion.DORotate(Vector3.zero, 0.4f).SetEase(Ease.OutBack))
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    if (_background && _explosion)
                    {
                        _background.SetActive(false);
                        _explosion.gameObject.SetActive(false);
                    }
                });
        }
    }
}
