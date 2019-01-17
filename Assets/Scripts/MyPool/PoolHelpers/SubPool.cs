using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets.Pool.Helpers
{

    public class SubPool
    {
        const int defaultCount = 10;

        public string Key { get; } = string.Empty;
        GameObject m_Reference = null;
        Transform m_Container = null;
        PoolBehaviour m_PoolBehaviour;
        public bool IsInited { get; } = false;

        List<PooledObjectInfo> m_InactiveObjs = new List<PooledObjectInfo>();
        List<PooledObjectInfo> m_ActiveObjs = new List<PooledObjectInfo>();

        public SubPool(string key, GameObject reference, int count, Transform container, PoolBehaviour poolBehaviour)
        {
            if (string.IsNullOrEmpty(key) || reference == null || container == null)
                return;
            Key = key;
            m_Reference = reference;
            m_Container = container;
            m_PoolBehaviour = poolBehaviour;
            if (count < 1)
                count = defaultCount;
            IsInited = true;
            for (int i = 0; i < count; ++i)
                CreateObject();
        }

        void CreateObject()
        {
            if (!IsInited)
                return;
            GameObject obj = Object.Instantiate(m_Reference, m_Container);
            obj.name = string.Format("{0} [{1}]", m_Reference.name, obj.GetInstanceID());
            if (obj.activeInHierarchy)
                obj.SetActive(false);
            m_InactiveObjs.Add(new PooledObjectInfo(obj, Key, this));
        }

        public bool ContainsObject(GameObject obj)
        {
            foreach (var objInf in m_InactiveObjs)
                if (objInf.Obj == obj)
                    return true;
            foreach (var objInf in m_ActiveObjs)
                if (objInf.Obj == obj)
                    return true;
            return false;
        }

        public GameObject Spawn(Vector3 position, Quaternion rotation, Transform parent)
        {
            if (!IsInited)
                return null;
            if (m_InactiveObjs.Count < 1)
            {
                switch (m_PoolBehaviour)
                {
                    case PoolBehaviour.CreateNewThings:
                    {
                        CreateObject();
                    }
                    break;
                    case PoolBehaviour.ReuseActiveThings:
                    {
                        if (m_ActiveObjs.Count < 1)
                            return null;
                        if (!Despawn(m_ActiveObjs[0].Obj))
                            return null;
                    }
                    break;
                    default:
                        return null;
                }
            }
            PooledObjectInfo objInfForSpawn = m_InactiveObjs[m_InactiveObjs.Count - 1];
            m_InactiveObjs.RemoveAt(m_InactiveObjs.Count - 1);
            objInfForSpawn.SetParent(parent);
            objInfForSpawn.SetPosition(position);
            objInfForSpawn.SetRotation(rotation);
            objInfForSpawn.SetActive(true);
            objInfForSpawn.OnSpawn();
            m_ActiveObjs.Add(objInfForSpawn);
            return objInfForSpawn.Obj;
        }
        public bool Despawn(GameObject obj)
        {
            if (!IsInited)
                return false;
            if (obj == null)
                return false;
            int count = m_ActiveObjs.Count;
            if (count < 1)
                return false;
            PooledObjectInfo objInfForDespawn = null;
            for (int i =0; i < count; ++i)
                if (m_ActiveObjs[i].Obj == obj)
                {
                    objInfForDespawn = m_ActiveObjs[i];
                    m_ActiveObjs.RemoveAt(i);
                    break;
                }
            if (objInfForDespawn == null)
                return false;
            objInfForDespawn.OnDespawn();
            objInfForDespawn.SetActive(false);
            objInfForDespawn.SetParent(m_Container);
            m_InactiveObjs.Add(objInfForDespawn);
            return true;
        }
    }
}
