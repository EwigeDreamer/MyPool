using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyHelpers;

namespace MyAssets.Pool
{ 
    [RequireComponent(typeof(MyPool))]
    public class MyPoolInitializer : MonoBehaviour, IPoolKeys
    {
# pragma warning disable 649
        [SerializeField] private MyPoolInfo[] m_PoolInfs;
#pragma warning restore 649

        private void Awake()
        {
            MyPool pool = GetComponent<MyPool>();
            if (pool == null)
                return;
            foreach (var poolInf in m_PoolInfs)
                pool.CreatePool(poolInf.Key, poolInf.Prefab, poolInf.Count, poolInf.IfPoolEmpty);
        }

        [System.Serializable]
        private struct MyPoolInfo
        {
# pragma warning disable 649
            [SerializeField] private string m_Key;
            public string Key { get { return m_Key; } }

            [SerializeField] [ResourcesPath] private string m_Path;
            public GameObject Prefab { get { return !string.IsNullOrEmpty(m_Path) ? Resources.Load<GameObject>(m_Path) : null; } }

            [SerializeField] private int m_Count;
            public int Count { get { return m_Count; } }

            [SerializeField] private PoolBehaviour m_IfPoolEmpty;
            public PoolBehaviour IfPoolEmpty { get { return m_IfPoolEmpty; } }
#pragma warning restore 649
        }

        string[] IPoolKeys.Keys
        {
            get
            {
                List<string> keys = new List<string>();
                if (m_PoolInfs != null)
                    foreach (var poolInf in m_PoolInfs)
                        keys.Add(poolInf.Key);
                return keys.ToArray();
            }
        }
    }
}
