#region

using RabbitGame.Gameplay;
using MoveDirection = RabbitGame.Gameplay.MoveDirection;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace RabbitGame.Views
{
    public class MoveButtonView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private MoveDirection direction;
        private PlayerInput _input;

        public void Initialize(PlayerInput input)
        {
            _input = input;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_input == null)
            {
                return;
            }
            _input.Press(direction);
        }

        public void OnPointerUp(PointerEventData eventData) 
        {
            if (_input == null)
            {
                return;
            }
            _input.Release(direction);
        }
    }
}
