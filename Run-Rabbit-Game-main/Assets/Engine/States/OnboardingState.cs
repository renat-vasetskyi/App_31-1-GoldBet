#region

using RabbitGame.Core;
using RabbitGame.Core.FSM;
using RabbitGame.ScriptableObjects;
using RabbitGame.Views;
using UnityEngine;

#endregion

namespace RabbitGame.States
{
    public class OnboardingState : State
    {
        public OnboardingState(StateMachine fsm, DIContainer diContainer) : base(fsm, diContainer) { }

        public override void Enter()
        {
            Debug.Log("Entered OnboardingState");
            GameObject onboardingUI = CreateUIInState(_diContainer.Resolve<PrefabRegistry>().Get("OnboardingUI"));
            OnboardingView onboardingView = onboardingUI.GetComponent<OnboardingView>();
            onboardingView.Initialize(OnStartGameClicked);
        }

        private void OnStartGameClicked()
        {
            _fsm.ChangeState<GameplayState>();
        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log("Exit OnboardingState");
        }
    }
}