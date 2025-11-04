#region

using RabbitGame.Core;
using RabbitGame.Core.FSM;
using RabbitGame.ScriptableObjects;
using RabbitGame.Views;
using UnityEngine;

#endregion

namespace RabbitGame.States
{
    public class SplashScreenState : State
    {
        public SplashScreenState(StateMachine fsm, DIContainer diContainer) : base(fsm, diContainer) { }

        public override void Enter()
        {
            Debug.Log("Entered SplashScreenState");
            GameObject splashScreenUI = CreateUIInState(_diContainer.Resolve<PrefabRegistry>().Get("SplashScreenUI"));

            SplashScreenView splashScreenView = splashScreenUI.GetComponent<SplashScreenView>();
            splashScreenView.PlaySplashAnimation(GoToMenuState);
        }

        private void GoToMenuState() 
        {
            _fsm.ChangeState<MenuState>();
        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log("Exit SplashScreenState");
        }
    }
}
