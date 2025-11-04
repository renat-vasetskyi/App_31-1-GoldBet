#region

using DG.Tweening;
using UnityEngine;

#endregion

namespace RabbitGame.Gameplay
{
    public class EnemyController : MonoBehaviour, IEnemy
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private bool _isDead = false;

        public void OnHitPlayer()
        {
            if (_isDead) 
            {
                return;
            }
            _isDead = true;

            PlayDeathAnimation();
        }

        public void FlipSpriteX(bool flip) 
        {
            spriteRenderer.flipX = flip;
        }

        private void PlayDeathAnimation() 
        {
            transform.DOScale(Vector3.zero, 0.3f)
                .SetEase(Ease.InBack);
        }
    }
}
