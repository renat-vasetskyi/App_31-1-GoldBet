#region

using RabbitGame.Core;
using RabbitGame.Core.FSM;
using RabbitGame.ScriptableObjects;
using RabbitGame.Views;
using UnityEngine;

#endregion

namespace RabbitGame.States
{
    public class HowToPlayState : State
    {
        public HowToPlayState(StateMachine fsm, DIContainer diContainer) : base(fsm, diContainer) { }

        public override void Enter()
        {
            Debug.Log("Entered HowToPlayState");
            GameObject howToPlayUI = CreateUIInState(_diContainer.Resolve<PrefabRegistry>().Get("HowToPlayUI"));
            HowToPlayView howToPlayView = howToPlayUI.GetComponent<HowToPlayView>();
            howToPlayView.Initialize(OnStartGameClicked, OnCloseClicked);
        }

        private void OnStartGameClicked()
        {
            _fsm.ChangeState<GameplayState>();
        }

        private void OnCloseClicked()
        {
            _fsm.ChangeState<MenuState>();
        }


        public override void Exit()
        {
            base.Exit();
            Debug.Log("Exit HowToPlayState");
        }
    }
}