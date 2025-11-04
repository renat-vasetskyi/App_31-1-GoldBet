#region

using DG.Tweening;
using RabbitGame.Core;
using RabbitGame.Core.FSM;
using RabbitGame.Gameplay;
using RabbitGame.ScriptableObjects;
using RabbitGame.Services;
using RabbitGame.Views;
using UnityEngine;

#endregion

namespace RabbitGame.States
{
    public class GameplayState : State
    {
        public GameplayState(StateMachine fsm, DIContainer diContainer) : base(fsm, diContainer) { }

        private PrefabRegistry _prefabRegistry;
        private PlayerInput _playerInput;
        private PlayerHealth _playerHealth;

        private PlayerController _playerController;
        private RoadSpawner _roadSpawner;
        private CarrotSpawner _carrotSpawner;
        private EnemySpawner _enemySpawner;

        private IScoreService _scoreService;

        private const float GameSpeed = 5f;

        public override void Enter()
        {
            Debug.Log("Entered GameplayState");

            _prefabRegistry = _diContainer.Resolve<PrefabRegistry>();
            _scoreService = _diContainer.Resolve<IScoreService>();
            _scoreService.OnLevelCompleted += GoToLevelCompletedState;
            _scoreService.ResetRoundScore();

            _playerInput = new PlayerInput();
            _playerHealth = new PlayerHealth();
            _playerHealth.OnDeath += PlayerHealth_OnDeath;

            CameraResizer cameraResizer = new CameraResizer(Camera.main, _prefabRegistry.Get("Road"));
            cameraResizer.Adjust();

            GameObject gameplayUI = CreateUIInState(_prefabRegistry.Get("GameplayUI"));
            GameplayView gameplayView = gameplayUI.GetComponent<GameplayView>();
            gameplayView.Initialize(_playerInput, _playerHealth, StartGame, GoToOnboardingState, _scoreService);

            //VFX
            CreateInState(_prefabRegistry.Get("Fog"), new Vector3(0, -4.3f), Quaternion.identity);
            //

            CreatePlayer();

            CreateRoadSpawner();
            CreateCarrotSpawner();
            CreateEnemiesSpawner();

            CreateLooseHPEffect();
        }

        private void CreatePlayer() 
        {
            Vector3 startPosition = new Vector3(0, -3f);
            float speed = 5f;
            float minX = -0.9f;
            float maxX = 0.9f;
            GameObject playerObject = CreateInState(_prefabRegistry.Get("Rabbit"), startPosition, Quaternion.identity);
            _playerController = playerObject.AddComponent<PlayerController>();
            _playerController.Initialize(_playerInput, _playerHealth, _scoreService, _diContainer.Resolve<ISoundService>(),
                _diContainer.Resolve<SoundRegistry>().Get("CarrotPickupSound"), speed, minX, maxX);
        }

        private void CreateRoadSpawner() 
        {
            int poolSize = 10;
            float roadHeight = 9.7f;
            GameObject roadParentObject = new GameObject("RoadParent");
            TrackToState(roadParentObject);
            _roadSpawner = new RoadSpawner(_prefabRegistry.Get("Road"), roadParentObject.transform, poolSize, roadHeight, GameSpeed);
        }

        private void CreateCarrotSpawner() 
        {
            int poolSize = 20;
            float carrotInterval = 2.5f;
            GameObject carrotParentObject = new GameObject("CarrotParent");
            TrackToState(carrotParentObject);
            _carrotSpawner = new CarrotSpawner(_prefabRegistry.Get("Carrot"), carrotParentObject.transform, poolSize, carrotInterval, GameSpeed);
        }

        private void CreateEnemiesSpawner() 
        {
            float leftSpawnPointX = -1f;
            float rightSpawnPointX = 1f;
            GameObject enemiesParentObject = new GameObject("EnemiesParent");
            TrackToState(enemiesParentObject);
            _enemySpawner = new EnemySpawner(_prefabRegistry.Get("Mole"), _prefabRegistry.Get("Farmer"),
                enemiesParentObject.transform, leftSpawnPointX, rightSpawnPointX, GameSpeed);
        }

        private void CreateLooseHPEffect() 
        {
            Vector3 looseHPEffectPosition = new Vector3(0, -2.35f);
            GameObject brownFog = CreateInState(_prefabRegistry.Get("BrownFog"), Vector3.zero, Quaternion.identity);
            GameObject looseHPEffect = CreateInState(_prefabRegistry.Get("LooseHPEffect"), looseHPEffectPosition, Quaternion.identity);
            new LoseHPEffect(brownFog, looseHPEffect, _playerHealth);
        }

        private void PlayerHealth_OnDeath()
        {
            float delayInSeconds = 0.6f;
            DOTween.Sequence()
                .AppendInterval(delayInSeconds)
                .AppendCallback(GoToLevelFailedState);
        }

        public override void Update(float deltaTime)
        {
            if (_roadSpawner != null)
            {
                _roadSpawner.Update(deltaTime);
            }
            if (_carrotSpawner != null)
            {
                _carrotSpawner.Update(deltaTime);
            }
            if (_enemySpawner != null)
            {
                _enemySpawner.Update(deltaTime);
            }
        }

        public override void Exit()
        {
            base.Exit();
            if (_playerHealth != null)
            {
                _playerHealth.OnDeath -= PlayerHealth_OnDeath;
            }
            if (_scoreService != null)
            {
                _scoreService.OnLevelCompleted -= GoToLevelCompletedState;
            }
            Debug.Log("Exit GameOverState");
        }


        private void StartGame() 
        {
            _roadSpawner.StartRoadMovement();
            _carrotSpawner.StartCarrotsMovement();
            _enemySpawner.StartEnemiesMovement();

            _playerController.StartPlayer();
        }

        private void StopGame() 
        {
            _roadSpawner.StopRoadMovement();
            _carrotSpawner.StopCarrotsMovement();
            _enemySpawner.StopEnemiesMovement();

            _playerController.StopPlayer();
        }

        private void GoToOnboardingState() 
        {
            _fsm.ChangeState<OnboardingState>();
        }

        private void GoToLevelFailedState() 
        {
            _fsm.ChangeState<LevelFailedState>();
        }

        private void GoToLevelCompletedState() 
        {
            _fsm.ChangeState<LevelCompletedState>();
        }
    }
}
