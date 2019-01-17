using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyAssets.Pool
{
    public interface IPooledObject
    {
        bool Despawn();
        event Action<GameObject> OnSpawn;
        event Action<GameObject> OnDespawn;
    }
}