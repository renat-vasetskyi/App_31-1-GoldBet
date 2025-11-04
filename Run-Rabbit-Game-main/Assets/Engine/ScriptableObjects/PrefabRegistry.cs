#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace RabbitGame.ScriptableObjects
{
    [System.Serializable]
    public class PrefabEntry
    {
        public string Key;
        public GameObject Prefab;
    }

    [CreateAssetMenu(menuName = "ScriptableObjects/PrefabRegistry", fileName = "PrefabRegistry")]
    public class PrefabRegistry : ScriptableObject
    {
        public List<PrefabEntry> Entries;

        private Dictionary<string, GameObject> _map;

        public GameObject Get(string key)
        {
            if (_map == null) 
            {
                _map = Entries.ToDictionary(e => e.Key, e => e.Prefab);
            }
            if (!_map.ContainsKey(key))
            {
                Debug.LogError($"Prefab registry does not contain key: {key}");
                return null;
            }
            return _map[key];
        }
    }
}
