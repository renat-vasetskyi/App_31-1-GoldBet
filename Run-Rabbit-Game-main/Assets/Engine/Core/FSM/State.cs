#region

using RabbitGame.Factories;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace RabbitGame.Core.FSM
{
    public abstract class State : IState
    {
        protected readonly StateMachine _fsm;
        protected readonly DIContainer _diContainer;

        private readonly List<Object> _createdObjects = new List<Object>();

        protected State(StateMachine fsm, DIContainer diContainer)
        {
            _fsm = fsm;
            _diContainer = diContainer;
        }

        public virtual void Enter() { }
        public virtual void Exit()
        {
            foreach (var obj in _createdObjects)
            {
                if (obj != null)
                {
                    if (obj is GameObject go) 
                    {
                        GameObject.Destroy(go);
                    }
                    else
                    {
                        Object.Destroy(obj);
                    }
                }
            }
            _createdObjects.Clear();
        }
        public virtual void Update(float deltaTime) { }

        protected GameObject CreateUIInState(GameObject UIPrefab)
        {
            GameObject UIElement = _diContainer.Resolve<UIFactory>().CreateUIElement(UIPrefab);
            _createdObjects.Add(UIElement);
            return UIElement;
        }

        protected T CreateInState<T>(T prefab, Vector3 position, Quaternion rotation) where T : Object
        {
            T instance = Object.Instantiate(prefab, position, rotation);
            _createdObjects.Add(instance);
            return instance;
        }

        protected void TrackToState(Object obj)
        {
            if (obj == null)
            {
                return;
            }
            if (_createdObjects.Contains(obj))
            {
                return;
            }
            _createdObjects.Add(obj);
        }
    }
}
