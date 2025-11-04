#region 

using UnityEngine;
using RabbitGame.Services;
using RabbitGame.ScriptableObjects;
using RabbitGame.Core.FSM;
using RabbitGame.States;
using RabbitGame.Factories;

#endregion

namespace RabbitGame.Core
{
    public class GameStarter : MonoBehaviour
    {
        [SerializeField] private PrefabRegistry prefabRegistry;
        [SerializeField] private SoundRegistry soundRegistry;
        [SerializeField] private Transform uiRoot;
        [SerializeField] private Transform audioRoot;

        private StateMachine _stateMachine;
        private DIContainer _diContainer;

        private void Awake()
        {
            _diContainer = new DIContainer();
            RegisterServices();
            RegisterFactories();

            _stateMachine = new StateMachine();
            RegisterFSMStates();

            _stateMachine.ChangeState<SplashScreenState>();
        }

        private void Update()
        {
            _stateMachine.Update(Time.deltaTime);
        }

        private void RegisterServices() 
        {
            _diContainer.Register(prefabRegistry);
            _diContainer.Register(soundRegistry);

            SaveService saveService = new SaveService();
            _diContainer.Register<ISaveService>(saveService);
            _diContainer.Register<IScoreService>(new ScoreService(saveService));
            _diContainer.Register<ISoundService>(new SoundService(audioRoot));
        }

        private void RegisterFactories() 
        {
            _diContainer.Register(new UIFactory(uiRoot));
        }

        private void RegisterFSMStates()
        {
            _stateMachine.RegisterState<SplashScreenState>(new SplashScreenState(_stateMachine, _diContainer));
            _stateMachine.RegisterState<MenuState>(new MenuState(_stateMachine, _diContainer));
            _stateMachine.RegisterState<GameplayState>(new GameplayState(_stateMachine, _diContainer));
            _stateMachine.RegisterState<HowToPlayState>(new HowToPlayState(_stateMachine, _diContainer));
            _stateMachine.RegisterState<OnboardingState>(new OnboardingState(_stateMachine, _diContainer));
            _stateMachine.RegisterState<LevelCompletedState>(new LevelCompletedState(_stateMachine, _diContainer));
            _stateMachine.RegisterState<LevelFailedState>(new LevelFailedState(_stateMachine, _diContainer));
        }
    }
}
