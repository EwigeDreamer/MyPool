using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyAssets.Pool.Helpers;

namespace MyAssets.Pool
{
    public enum PoolBehaviour { DoNothing, ReuseActiveThings, CreateNewThings }

    public class MyPool : MonoBehaviour
    {
        private bool SetInstance()
        {
            if (Instance != null)
                return false;
            Instance = this;
            return true;
        }
        public static MyPool Instance { get; private set; } = null;
        private void Awake()
        {
            if (!SetInstance())
            {
                enabled = false;
                return;
            }
        }

        List<SubPool> m_SubPoolList = new List<SubPool>();

        public bool CreatePool(string key, GameObject obj, int count, PoolBehaviour poolBehaviour)
        {
            if (string.IsNullOrEmpty(key) || obj == null || ContainsKey(key))
            return false;
            Transform container = (new GameObject(string.Format("[container] {0}", key))).transform;
            container.parent = transform;
            GameObject reference = Instantiate(obj);
            reference.name = obj.name;
            reference.transform.parent = container;
            if (reference.activeInHierarchy)
                reference.SetActive(false);
            m_SubPoolList.Add(new SubPool(key, reference, count, container, poolBehaviour));
            return true;
        }
        public GameObject Spawn(string key, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject spawnedObj = null;
            foreach (var subPool in m_SubPoolList)
                if (subPool.Key == key)
                {
                    spawnedObj = subPool.Spawn(position, rotation, parent);
                    break;
                }
            return spawnedObj;
        }
        public bool Despawn(GameObject obj)
        {
            foreach (var subPool in m_SubPoolList)
                if (subPool.Despawn(obj))
                    return true;
            return false;
        }
        public bool ContainsKey(string key)
        {
            foreach (var subPool in m_SubPoolList)
                if (subPool.Key == key)
                    return true;
            return false;
        }
        public bool ContainsObject(GameObject obj)
        {
            foreach (var subPool in m_SubPoolList)
                if (subPool.ContainsObject(obj))
                    return true;
            return false;
        }
    }
}