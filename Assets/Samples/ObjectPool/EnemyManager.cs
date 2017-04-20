using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using UniRx;

/// <summary>
/// A simple demonstration of UniRx.Toolkit.ObjectPool
/// 
/// </summary>
public class EnemyManager : MonoBehaviour 
{
    public Button buttonRent;
    public Button buttonReturn;

    EnemyObjectPool pool;
    List<Transform> enemyList;

	// Use this for initialization
	void Start () 
    {
        enemyList = new List<Transform>();

        GameObject enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");

        // Create a new object pool for enemy gameobject.
        pool = new EnemyObjectPool(this.gameObject.transform);
        pool.EnemyPrefab = enemyPrefab;

        // prelaoding for caching.
        pool.PreloadAsync(10, 1).Subscribe(_ => { Debug.Log("Done Preloading"); });

        // Rent one from the pool. This pop a gameobject to be shown up.
        buttonRent.OnClickAsObservable().Subscribe(_ =>
        {
            var inst = pool.Rent();
            enemyList.Add(inst);
        });

        // Return rented one to the pool.
        buttonReturn.OnClickAsObservable().Subscribe(_ => 
        {
            var tm = enemyList.FirstOrDefault();
            if (tm != null)
            {
                pool.Return(tm);
                enemyList.Remove(tm);
            }
        });
	}
	
    void OnDestroy()
    {
        // Destroy all preloaded object in the pool.
        pool.Clear();
    }
}
