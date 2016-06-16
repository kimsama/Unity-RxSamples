using UnityEngine;
using System;
using System.Collections.Generic;
using UniRx;

public class SamplePublish : MonoBehaviour
{

    void Start()
    {
        //TestA();

        TestB();

        TestC();
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
        string[] names = { "Cube", "Cylinder", "Sphere" };

        var sequence = Observable.Range(1, 3).Publish();

        //var streamA = sequence.Do(val => { ResourceStreamA(names[val-1]); });
        var streamA = sequence.Select(e => names[e - 1] + "A")
            .Do(s => Debug.Log(s))
            .SelectMany(x => Resources.LoadAsync<GameObject>(x).AsAsyncOperationObservable())

            .Subscribe(val =>
           {
               GameObject o = val.asset as GameObject;
               GameObject inst = GameObject.Instantiate(o);
               Debug.LogFormat("{0}", inst.name);
           }, () => { });
        //var streamB = sequence.Do(val => { ResourceStreamB(names[val-1]); });
        var streamB = sequence.Select(e => names[e - 1] + "B")
            .Do(s => Debug.Log(s))
            .SelectMany(x => Resources.LoadAsync<GameObject>(x).AsAsyncOperationObservable())

            .Subscribe(val =>
            {
                GameObject o = val.asset as GameObject;
                GameObject inst = GameObject.Instantiate(o);
                Debug.LogFormat("{0}", inst.name);
            }, () => { });

        /*
        Observable.WhenAll(streamA, streamB).Subscribe(
            x => {
                Debug.Log("OnNext");
                GameObject a = x[0].asset as GameObject;
                if (a != null)
                    Debug.LogFormat("A: {0}", a.name);

                GameObject b = x[1].asset as GameObject;
                if (b != null)
                    Debug.LogFormat("B: {0}", b.name);



                // how can I access to the actually loaded gameobject?
                // ...
            });
        */

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

    void TestC()
    {
        string[] names = { "Cube", "Cylinder", "Sphere" };

        var sequence = Observable.Range(1, 3).Publish();

        var streamA = sequence.Select(e => names[e - 1] + "A")
            .Do(s => Debug.Log(s))
            .SelectMany(x => Resources.LoadAsync<GameObject>(x).AsAsyncOperationObservable());
            

        var streamB = sequence.Select(e => names[e - 1] + "B")
            .Do(s => Debug.Log(s))
            .SelectMany(x => Resources.LoadAsync<GameObject>(x).AsAsyncOperationObservable());


        List< IObservable <ResourceRequest>> list = new List<IObservable<ResourceRequest>>();
        list.Add(streamA);
        list.Add(streamB);
        //Observable.WhenAll(streamA, streamB).Subscribe(
        Observable.Concat(list).ToArray().Subscribe(
            x =>
            {
                foreach(ResourceRequest rq in x)
                {
                    GameObject r = rq.asset as GameObject;
                    GameObject o = GameObject.Instantiate(r);
                }
                //GameObject r = x[0].asset as GameObject;
                //GameObject o = GameObject.Instantiate(r);
                //Debug.LogFormat("concat {0}", x);
                
                /*
                GameObject a = x[0].asset as GameObject;
                GameObject o = Instantiate(a) as GameObject;
                GameObject b = x[1].asset as GameObject;
                GameObject q = Instantiate(b) as GameObject;
                */
            }, ()=> { });

        sequence.Connect();
    }
}

