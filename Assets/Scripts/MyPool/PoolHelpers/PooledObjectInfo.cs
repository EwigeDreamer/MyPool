using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyAssets.Pool.Helpers
{
    public class PooledObjectInfo
    {
        public string Key { get; } = null;
        public GameObject Obj { get; } = null;
        List<IPooledComponent> m_Pcs = new List<IPooledComponent>();
        PooledBeacon m_Bcn = null;
        public bool IsValid { get { return Obj != null && m_Bcn != null; } }

        public event Action OnSpawnEvent;
        public event Action OnDespawnEvent;

        public PooledObjectInfo(GameObject obj, string key, SubPool subPool)
        {
            Obj = obj;
            Key = key;
            obj.GetComponentsInChildren<IPooledComponent>(true, m_Pcs);
            m_Bcn = obj.AddComponent<PooledBeacon>();
            m_Bcn.Init(subPool, this);
            foreach (var pc in m_Pcs)
                pc.Init(m_Bcn);
        }

        public void SetActive(bool state)
        {
            Obj?.SetActive(state);
        }
        public void SetPosition(Vector3 pos)
        {
            Obj.transform.position = pos;
        }
        public void SetRotation(Quaternion rot)
        {
            Obj.transform.rotation = rot;
        }
        public void SetParent(Transform parent)
        {
            Obj.transform.parent = parent;
        }
        public void OnSpawn()
        {
            foreach (var pc in m_Pcs)
                pc.OnSpawn();
            OnSpawnEvent?.Invoke();
        }
        public void OnDespawn()
        {
            foreach (var pc in m_Pcs)
                pc.OnDespawn();
            OnDespawnEvent?.Invoke();
        }
    }
}