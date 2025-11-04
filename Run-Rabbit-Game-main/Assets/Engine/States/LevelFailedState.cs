#region

using RabbitGame.Core;
using RabbitGame.Core.FSM;
using RabbitGame.ScriptableObjects;
using RabbitGame.Services;
using RabbitGame.Views;
using UnityEngine;

#endregion

namespace RabbitGame.States
{
    public class LevelFailedState : State
    {
        public LevelFailedState(StateMachine fsm, DIContainer diContainer) : base(fsm, diContainer) { }

        public override void Enter()
        {
            Debug.Log("Entered LevelFailedState");
            GameObject levelFailedUI = CreateUIInState(_diContainer.Resolve<PrefabRegistry>().Get("LevelFailedUI"));
            LevelFailedView mainMenuView = levelFailedUI.GetComponent<LevelFailedView>();
            mainMenuView.Initialize(OnTryAgainClicked, _diContainer.Resolve<IScoreService>());

            _diContainer.Resolve<ISoundService>().PlaySound(_diContainer.Resolve<SoundRegistry>().Get("LoseSound"));
        }

        private void OnTryAgainClicked()
        {
            _fsm.ChangeState<MenuState>();
        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log("Exit LevelFailedState");
        }
    }
}