using UnityEngine;
using System.Collections.Generic;
using UniRx.Toolkit;

public class EnemyObjectPool : ObjectPool<Transform> 
{
    public GameObject EnemyPrefab;
    protected Transform parent;

    public EnemyObjectPool(Transform _parent)
    {
        parent = _parent;
    }

    protected override Transform CreateInstance()
    {
        Transform tm = GameObject.Instantiate(EnemyPrefab).GetComponent<Transform>();
        if (parent != null)
            tm.transform.SetParent(parent);

        return tm;
    }

}
