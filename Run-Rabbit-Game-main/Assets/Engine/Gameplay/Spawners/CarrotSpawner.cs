#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace RabbitGame.Gameplay
{
    public class CarrotSpawner
    {
        private List<CarrotController> _pool = new List<CarrotController>();
        private float _carrotInterval;
        private float _speed;
        private int _topIndex;

        private bool _canMove = false;

        public CarrotSpawner(GameObject carrotPrefab, Transform carrotParent, int poolSize, float carrotInterval, float speed)
        {
            _carrotInterval = carrotInterval;
            _speed = speed;

            FillPool(carrotPrefab, carrotParent, poolSize);
        }

        public void Update(float deltaTime)
        {
            if (!_canMove) return;

            MoveCarrots(deltaTime);
        }

        public void StartCarrotsMovement()
        {
            _canMove = true;
        }

        public void StopCarrotsMovement()
        {
            _canMove = false;
        }

        private void FillPool(GameObject carrotPrefab, Transform parent, int poolSize)
        {
            for (int i = 0; i < poolSize; i++)
            {
                var carrotObject = Object.Instantiate(carrotPrefab, parent);
                carrotObject.transform.position = new Vector3(0, i * _carrotInterval, 0);

                var carrotController = carrotObject.GetComponent<CarrotController>();
                carrotController.ResetCarrot();

                _pool.Add(carrotController);
            }

            _topIndex = _pool.Count - 1;
        }

        private void MoveCarrots(float deltaTime)
        {
            foreach (var carrot in _pool)
            {
                carrot.transform.position += Vector3.down * _speed * deltaTime;
            }

            float intervalToDelete = -_carrotInterval * 10;
            for (int i = 0; i < _pool.Count; i++)
            {
                if (_pool[i].transform.position.y < intervalToDelete)
                {
                    float newY = _pool[_topIndex].transform.position.y + _carrotInterval;
                    _pool[i].transform.position = new Vector3(0, newY, 0);
                    _pool[i].ResetCarrot();

                    _topIndex = i;
                }
            }
        }
    }
}
