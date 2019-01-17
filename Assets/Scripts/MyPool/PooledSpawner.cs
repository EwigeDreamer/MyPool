using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyAssets.Pool.Helpers;

namespace MyAssets.Pool
{
    public class PooledSpawner : MonoBehaviour
    {
# pragma warning disable 649
        [SerializeField] private bool m_SpawnOnStart;
        [SerializeField][PoolKey] private string m_PoolKey;
#pragma warning restore 649
        public string PoolKey { get { return m_PoolKey; } }
        GameObject m_Obj = null;
        public event Action<GameObject> OnSpawn;

        private void Start()
        {
            if (m_SpawnOnStart)
                Spawn();
        }

        public GameObject Spawn()
        {
            Unsubscribe(m_Obj);
            GameObject go = MyPool.Instance?.Spawn(m_PoolKey, transform.position, transform.rotation);
            if (go == null)
                return null;
            m_Obj = go;
            Subscribe(m_Obj);
            OnSpawn?.Invoke(m_Obj);
            return m_Obj;
        }

        public GameObject Get()
        {
            return m_Obj;
        }

        private void Despawned(GameObject obj)
        {
            Unsubscribe(obj);
            if (m_Obj == obj)
                m_Obj = null;
        }

        void Subscribe(GameObject obj)
        {
            if (obj == null)
                return;
            PooledBeacon bc = obj.GetComponent<PooledBeacon>();
            if (bc == null)
                return;
            bc.OnDespawn += Despawned;
        }
        void Unsubscribe(GameObject obj)
        {
            if (obj == null)
                return;
            PooledBeacon bc = obj.GetComponent<PooledBeacon>();
            if (bc == null)
                return;
            bc.OnDespawn -= Despawned;
        }

#if UNITY_EDITOR
        [ContextMenu("SPAWN OBJECT")]
        void ManualSpawn()
        {
            Spawn();
        }
#endif
    }
}