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
    public class LevelCompletedState : State
    {
        public LevelCompletedState(StateMachine fsm, DIContainer diContainer) : base(fsm, diContainer) { }

        public override void Enter()
        {
            Debug.Log("Entered LevelCompletedState");
            GameObject levelCompletedUI = CreateUIInState(_diContainer.Resolve<PrefabRegistry>().Get("LevelCompletedUI"));
            LevelCompletedView levelCompletedView = levelCompletedUI.GetComponent<LevelCompletedView>();
            levelCompletedView.Initialize(OnNextClicked, _diContainer.Resolve<IScoreService>());

            _diContainer.Resolve<ISoundService>().PlaySound(_diContainer.Resolve<SoundRegistry>().Get("WinSound"));
        }

        private void OnNextClicked()
        {
            _fsm.ChangeState<MenuState>();
        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log("Exit LevelCompletedState");
        }
    }
}