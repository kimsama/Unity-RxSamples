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

    // You can override the following methods if it is neccessary.
    // Note instance.gameObject.SetActive(true) is called inside of OnBeforeRent() 
    // and instance.gameObject.SetActive(false) is called for OnBeforeReturn().
    // protected override void OnBeforeRent(Transform instance)
    // protected override void OnBeforeReturn(Transform instance)
    // protected override void OnClear(Transform instance)
}
