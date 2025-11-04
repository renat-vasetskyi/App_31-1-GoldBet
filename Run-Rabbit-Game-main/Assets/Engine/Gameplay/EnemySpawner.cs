#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace RabbitGame.Gameplay
{
    public class EnemySpawner
    {
        private GameObject _molePrefab;
        private GameObject _farmerPrefab;
        private Transform _enemiesParent;
        private float _leftSpawnPointX;
        private float _rightSpawnPointX;
        private float _speed;

        private float _timer;
        private float _nextSpawnTime;

        private bool _canMove = false;

        private List<GameObject> _activeEnemies = new List<GameObject>();

        public EnemySpawner(
            GameObject molePrefab,
            GameObject farmerPrefab,
            Transform enemiesParent,
            float leftSpawnPointX,
            float rightSpawnPointX,
            float speed)
        {
            _molePrefab = molePrefab;
            _farmerPrefab = farmerPrefab;
            _enemiesParent = enemiesParent;
            _leftSpawnPointX = leftSpawnPointX;
            _rightSpawnPointX = rightSpawnPointX;
            _speed = speed;

            ScheduleNextSpawn();
        }

        public void Update(float deltaTime)
        {
            HandleTimer(deltaTime);
            MoveEnemies(deltaTime);
        }

        private void HandleTimer(float deltaTime) 
        {
            if (!_canMove)
            {
                return;
            }
            _timer += deltaTime;
            if (_timer >= _nextSpawnTime)
            {
                SpawnEnemy();
                ScheduleNextSpawn();
            }
        }

        private void MoveEnemies(float deltaTime) 
        {
            if (!_canMove)
            {
                return;
            }
            float positionYToDelete = -20f;
            for (int i = _activeEnemies.Count - 1; i >= 0; i--)
            {
                _activeEnemies[i].transform.Translate(Vector3.down * _speed * deltaTime);
                if (_activeEnemies[i].transform.position.y < positionYToDelete)
                {
                    Object.Destroy(_activeEnemies[i]);
                    _activeEnemies.RemoveAt(i);
                }
            }
        }

        private void ScheduleNextSpawn()
        {
            float minSpawnDelay = 2f;
            float maxSpawnDelay = 4f;
            _timer = 0f;
            _nextSpawnTime = Random.Range(minSpawnDelay, maxSpawnDelay);
        }

        private void SpawnEnemy()
        {
            GameObject prefab = Random.value < 0.5f ? _molePrefab : _farmerPrefab;
            float spawnPointX = Random.value < 0.5f ? _leftSpawnPointX : _rightSpawnPointX;
            float spawnPointY = 13f;
            GameObject enemyObject = GameObject.Instantiate(prefab, new Vector3(spawnPointX, spawnPointY), Quaternion.identity, _enemiesParent);
            EnemyController enemyController = enemyObject.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.FlipSpriteX(spawnPointX < 0);
            }
            _activeEnemies.Add(enemyObject);
        }

        public void StartEnemiesMovement()
        {
            _canMove = true;
        }

        public void StopEnemiesMovement()
        {
            _canMove = false;
        }
    }
}
