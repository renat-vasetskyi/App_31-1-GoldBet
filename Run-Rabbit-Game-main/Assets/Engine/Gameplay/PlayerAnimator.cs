#region

using UnityEngine;

#endregion

namespace RabbitGame.Gameplay
{
    public class PlayerAnimator
    {
        private readonly Animator _animator;
        private static readonly int StateHash = Animator.StringToHash("State");

        public PlayerAnimator(Animator animator)
        {
            _animator = animator;
        }

        public void PlayIdle()
        {
            _animator.SetInteger(StateHash, 0);
        }

        public void PlayRun()
        {
            _animator.SetInteger(StateHash, 1);
        }
    }
}
