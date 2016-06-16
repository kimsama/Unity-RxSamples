using UnityEngine;
using System;
using System.Collections;
using UniRx;

public class SamplePublish : MonoBehaviour 
{

	void Start () 
    {
        TestA();

        TestB();
	}

    /// <summary>
    /// result:
    ///     A : OnNext (1)
    ///     B : OnNext (1)
    ///     A : OnNext (2)
    ///     B : OnNext (2)
    ///     A : OnNext (3)
    ///     B : OnNext (4)
    ///     A : OnCompleted
    ///     B : OnCompleted
    /// </summary>
    void TestA()
    {
        var sequence = Observable.Range(1, 3).Publish();

        sequence.Subscribe(
            val => { Debug.LogFormat("A : OnNext ({0})", val); },
            () => { Debug.LogFormat("A : OnCompleted"); });

        sequence.Subscribe(
            val => { Debug.LogFormat("B : OnNext ({0})", val); },
            () => { Debug.LogFormat("B : OnCompleted"); });

        // start  
        sequence.Connect().AddTo(this);
    }

    void TestB()
    {
        string[] names = {"cube", "cylinder", "sphere"};

        var sequence = Observable.Range(1, 3).Publish();

        var streamA = sequence.Do(val => { ResourceStreamA(names[val-1]); });
        var streamB = sequence.Do(val => { ResourceStreamB(names[val-1]); });

        Observable.WhenAll(streamA, streamB).Subscribe(
            x => {
                Debug.LogFormat("A: {0}", x[0]);
                Debug.LogFormat("B: {0}", x[1]);

                // how can I access to the actually loaded gameobject?
                // ...
            });

        sequence.Connect();
    }

    static IObservable<ResourceRequest> ResourceStreamA(string name)
    {
        name += "A";
        Debug.LogFormat("load: {0}", name);
        return Resources.LoadAsync<GameObject>(name).AsAsyncOperationObservable().Last();
    }

    static IObservable<ResourceRequest> ResourceStreamB(string name)
    {
        name += "B";
        Debug.LogFormat("load: {0}", name);
        return Resources.LoadAsync<GameObject>(name).AsAsyncOperationObservable().Last();
    }

}
