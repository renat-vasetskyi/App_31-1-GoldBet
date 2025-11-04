#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace RabbitGame.Gameplay
{
    public class RoadSpawner
    {
        private List<GameObject> _pool = new List<GameObject>();
        private float _roadHeight;
        private float _speed;
        private int _topIndex;

        private bool _canMove = false;

        public RoadSpawner(GameObject roadPrefab, Transform roadParent, int poolSize, float roadHeight, float speed)
        {
            _roadHeight = roadHeight;
            _speed = speed;

            FillPool(roadPrefab, roadParent, poolSize);
        }

        public void Update(float deltaTime)
        {
            MoveRoads(deltaTime);
        }

        public void StartRoadMovement() 
        {
            _canMove = true;
        }

        public void StopRoadMovement()
        {
            _canMove = false;
        }

        private void FillPool(GameObject roadPrefab, Transform roadParent, int poolSize) 
        {
            for (int i = 0; i < poolSize; i++)
            {
                var road = Object.Instantiate(roadPrefab, roadParent);
                road.transform.position = new Vector3(0, i * _roadHeight, 0);
                _pool.Add(road);
            }

            _topIndex = _pool.Count - 1;
        }

        private void MoveRoads(float deltaTime) 
        {
            if (!_canMove)
            {
                return;
            }

            foreach (var road in _pool)
            {
                road.transform.position += Vector3.down * _speed * deltaTime;
            }

            for (int i = 0; i < _pool.Count; i++)
            {
                if (_pool[i].transform.position.y < -_roadHeight)
                {
                    float newY = _pool[_topIndex].transform.position.y + _roadHeight;
                    _pool[i].transform.position = new Vector3(0, newY, 0);
                    _topIndex = i;
                }
            }
        }
    }
}
