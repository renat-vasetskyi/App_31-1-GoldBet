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
    public class MenuState : State
    {
        public MenuState(StateMachine fsm, DIContainer diContainer) : base(fsm, diContainer) { }

        public override void Enter()
        {
            Debug.Log("Entered MenuState");
            GameObject mainMenuUI = CreateUIInState(_diContainer.Resolve<PrefabRegistry>().Get("MainMenuUI"));
            MainMenuView mainMenuView = mainMenuUI.GetComponent<MainMenuView>();
            mainMenuView.Initialize(OnPlayClicked, OnInfoClicked);

            _diContainer.Resolve<ISoundService>().PlayMusic(_diContainer.Resolve<SoundRegistry>().Get("MainMusic"));
        }

        private void OnPlayClicked()
        {
            _fsm.ChangeState<GameplayState>();
        }

        private void OnInfoClicked() 
        {
            _fsm.ChangeState<HowToPlayState>();
        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log("Exit MenuState");
        }
    }
}
