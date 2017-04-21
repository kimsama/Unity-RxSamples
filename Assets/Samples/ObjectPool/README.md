# ObjectPool / AsyncObjectPool

This sample show how to use ObjectPool, though not a native part of UniRx but useful. (AsyncObjectPool is most same as ObjectPool except it works asyncronously)

> Note you need Unity 5.4.x or newly version than it to use ObjectPool or AsyncObjectPool.


* See the [blog page of @neue](http://neue.cc/2016/08/03_536.html)


## EnemyObjectPool

First, make a classs which derives ObjectPool<T> and override  *'CreateInstance()'* for its method.

``` csharp
public class EnemyObjectPool : ObjectPool<Transform>
{
    // A gameobject which to be instantiated.
    public GameObject EnemyPrefab;

    // to keep created instance under the parent gameobject in the hierarchy.
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
```

Note, for a generic type of the parent class, ```ObjectPool<T>```, only *UnityEngine.Component* derived class can be possible as its type.

Also there are other methods can be overrided if it is neccessary.

> Note instance.gameObject.SetActive(true) is called inside of OnBeforeRent()
 and instance.gameObject.SetActive(false) is called for OnBeforeReturn().

``` csharp
// Override, if you have somethiing to do before you rent an object from a pool.
protected override void OnBeforeRent(Transform instance)
// Override, if you have somethiing to do before you return an object back to a pool.
protected override void OnBeforeReturn(Transform instance)
// Override, if you have somethiing to do when on emptying a pool.
protected override void OnClear(Transform instance)
```

> Note OnClear is also called inside of Shrink and StartShrinkTimer.

## Instantiate Pool

Instantiate *'ObjectPool'* instance and set GameObject which to be intantiated within the pool.

``` csharp
pool = new EnemyObjectPool(this.gameObject.transform);
pool.EnemyPrefab = enemyPrefab;
```

Use *'Rent'* to get an object from pool.

``` csharp
Transform tm = pool.Rent ();
```
> Note, use *'RentAsync'*  instead of *'Rend'* if you derived AsyncObjectPool instead of ObjectPool.

Also you can preload an object of a pool with *'PreloadAsync'*.

``` csharp
pool.PreloadAsync(10, 1).Subscribe(_ => { Debug.Log("Done Preloading"); });
```

Note the above code creates each object per a one frame and inserts it into the pool by number of ten.

If an object is no longer needed, back it to the pool with *'Return'*.

``` csharp
pool.Return (tm);
```

## Empty Pool

You can empty the pool with *'Clear'* and the pool has no object inside of it.
``` csharp
pool.Clear ();
```

Note *'Clear'* destroy all objects in a pool.

## Shrink and StartShrinkTImer

*'ObjectPool'* of *UniRx* provides any other convenient methods based on power of Rx. *'Shrink'* is one of them.

Use *'Shrink'* if you want to decrease number of object in a pool.

``` csharp
pool.Shrink(0.6f, 4);
```
After shrinking, the pool remains 60% of object but keep four as its minimum.

There is also another method to shrink size of a pool, *'StartShrinkTimer'* which shrinks size of a pool as like *'Shrink'* does but it reduces per a given time.

``` csharp
IDisposal disposal  = pool.StartShrinkTimer(System.TimeSpan.FromSeconds(2), 0.4f, 3);
```

The above code shrink up to 40% for every two second until the number of object in the pool is three.

If you want to stop shrinking on the way of its progress, use *'Dispose'* same as like other UniRx's object.

``` csharp
disposable.Dispose ();
```
