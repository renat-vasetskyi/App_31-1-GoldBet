#region

using RabbitGame.Services;
using UnityEngine;

#endregion

namespace RabbitGame.Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerInput _input;
        private PlayerHealth _health;
        private PlayerAnimator _animator;
        private float _speed;

        private float _minX;
        private float _maxX;

        private MoveDirection moveDir = MoveDirection.None;

        private IScoreService _scoreService;
        private ISoundService _soundService;

        private AudioClip _collectCarrotSound;

        public void Initialize(PlayerInput input, PlayerHealth health, IScoreService scoreService, ISoundService soundService,
            AudioClip collectCarrotSound, float speed, float minX, float maxX)
        {
            _input = input;
            _health = health;
            _scoreService = scoreService;
            _soundService = soundService;
            _collectCarrotSound = collectCarrotSound;

            _speed = speed;
            _minX = minX;
            _maxX = maxX;

            if (TryGetComponent<Animator>(out Animator animator))
            {
                _animator = new PlayerAnimator(animator);
            }

            _input.OnMoveInput += HandleMoveInput;
        }

        private void OnDestroy()
        {
            if (_input != null) 
            {
                _input.OnMoveInput -= HandleMoveInput;
            }
        }

        private void Update()
        {
            MovePlayer();
        }

        private void MovePlayer() 
        {
            if (moveDir == MoveDirection.None)
            {
                return;
            }
            transform.Translate(Vector3.right * (int)moveDir * _speed * Time.deltaTime);

            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, _minX, _maxX);
            transform.position = pos;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<ICollectable>(out ICollectable collectable))
            {
                collectable.Collect();
                _scoreService.AddScore(1);
                _soundService.PlaySound(_collectCarrotSound);
                return;
            }
            if (other.TryGetComponent<IEnemy>(out IEnemy enemy))
            {
                enemy.OnHitPlayer();
                _health.TakeDamage();
                return;
            }
        }

        private void HandleMoveInput(MoveDirection dir, bool pressed)
        {
            if (pressed) 
            {
                moveDir = dir;
                return;
            }
            if (moveDir == dir)
            {
                moveDir = MoveDirection.None;
                return;
            }
        }

        public void StartPlayer() 
        {
            if (_animator != null)
            {
                _animator.PlayRun();
            }
        }

        public void StopPlayer() 
        {
            if (_animator != null)
            {
                _animator.PlayIdle();
            }
        }
    }
}
