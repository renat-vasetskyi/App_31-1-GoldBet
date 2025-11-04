#region 

using RabbitGame.ScriptableObjects;
using UnityEngine;

#endregion

namespace RabbitGame.Factories
{
    public class UIFactory
    {
        private Transform _uiRoot;

        public UIFactory(Transform uiRoot)
        {
            _uiRoot = uiRoot;
        }

        public GameObject CreateUIElement(GameObject uiElementPrefab)
        {
            return GameObject.Instantiate(uiElementPrefab, _uiRoot);
        }
    }
}
