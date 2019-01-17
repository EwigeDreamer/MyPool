using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyAssets.Pool.Helpers;

namespace MyAssets.Pool.Helpers
{
    public class PooledBeacon : MonoBehaviour, IPooledObject
    {
        SubPool m_SubPool = null;
        PooledObjectInfo m_ObjInf = null;
        public void Init(SubPool subPool, PooledObjectInfo objInf)
        {
            m_SubPool = subPool;
            m_ObjInf = objInf;
            if (!IsInited)
                return;
            m_ObjInf.OnSpawnEvent += OnSpawnMethod;
            m_ObjInf.OnDespawnEvent += OnDespawnMethod;
        }
        public bool IsInited { get { return m_SubPool != null && m_ObjInf != null; } }

        [ContextMenu("DESPAWN OBJECT")]
        public bool Despawn()
        {
            if (!IsInited)
                return false;
            return m_SubPool.Despawn(gameObject);
        }

        public event System.Action<GameObject> OnSpawn;
        public event System.Action<GameObject> OnDespawn;

        private void OnSpawnMethod()
        {
            OnSpawn?.Invoke(gameObject);
        }
        private void OnDespawnMethod()
        {
            OnDespawn?.Invoke(gameObject);
        }

        private void OnDestroy()
        {
            if (!IsInited)
                return;
            m_ObjInf.OnSpawnEvent -= OnSpawnMethod;
            m_ObjInf.OnDespawnEvent -= OnDespawnMethod;
        }
    }
}