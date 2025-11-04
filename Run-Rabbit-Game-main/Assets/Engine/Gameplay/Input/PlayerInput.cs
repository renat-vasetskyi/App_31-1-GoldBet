#region

using System;

#endregion

namespace RabbitGame.Gameplay
{
    public class PlayerInput
    {
        public event Action<MoveDirection, bool> OnMoveInput;
        public void Press(MoveDirection dir) => OnMoveInput?.Invoke(dir, true);
        public void Release(MoveDirection dir) => OnMoveInput?.Invoke(dir, false);
    }

    public enum MoveDirection { Left = -1, None = 0, Right = 1 }
}
