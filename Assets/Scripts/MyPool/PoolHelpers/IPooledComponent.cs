using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyAssets.Pool.Helpers
{
    public interface IPooledComponent
    {
        void Init(PooledBeacon beacon);
        void OnSpawn();
        void OnDespawn();
    }
}