using UnityEngine;
using System;
using System.Collections.Generic;
using UniRx;

public class SamplePublish : MonoBehaviour
{
    string[] names = { "Cube", "Cylinder", "Sphere" };

    void Start()
    {
        //Test_Default();

        //Test_OnlyPublisih();
        Test_WhenAll();
        Test_Zip();
        Test_Merge();
        Test_CombineLatest();
        Test_Concat();
    }

    /// <summary>
    /// Result:
    ///     A : OnNext (1)
    ///     B : OnNext (1)
    ///     A : OnNext (2)
    ///     B : OnNext (2)
    ///     A : OnNext (3)
    ///     B : OnNext (4)
    ///     A : OnCompleted
    ///     B : OnCompleted
    /// </summary>
    void Test_Default()
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

    /// <summary>
    /// 
    /// </summary>
    void Test_OnlyPublisih()
    {
        var sequence = Observable.Range(1, 3).Publish();

        var streamA = sequence.Select(e => names[e - 1] + "A")
            .Do(s => Debug.Log(s))
            .SelectMany(x => Resources.LoadAsync<GameObject>(x).AsAsyncOperationObservable())
            .Subscribe(val =>
           {
               GameObject o = val.asset as GameObject;
               GameObject inst = GameObject.Instantiate(o);
               Debug.LogFormat("{0}", inst.name);
           }, () => { /*OnCompleted*/ });

        var streamB = sequence.Select(e => names[e - 1] + "B")
            .Do(s => Debug.Log(s))
            .SelectMany(x => Resources.LoadAsync<GameObject>(x).AsAsyncOperationObservable())
            .Subscribe(val =>
            {
                GameObject o = val.asset as GameObject;
                GameObject inst = GameObject.Instantiate(o);
                Debug.LogFormat("{0}", inst.name);
            }, () => { /*OnCompleted*/ });

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

    /// <summary>
    /// Result:
    ///     A: SphereA
    ///     B: SphereB
    /// </summary>
    void Test_WhenAll()
    {
        var sequence = Observable.Range(0, 3).Publish(); // from 0
        var streamA = sequence.SelectMany(x => ResourceStreamA(names[x]));
        var streamB = sequence.SelectMany(x => ResourceStreamB(names[x]));

        Observable.WhenAll(streamA, streamB).Subscribe(
            x =>
            {
                Debug.Log("+++ WhenAll +++");
                Debug.LogFormat("length: {0}", x.Length);
                GameObject a = x[0].asset as GameObject;
                if (a != null)
                    Debug.LogFormat("A: {0}", a.name);

                GameObject b = x[1].asset as GameObject;
                if (b != null)
                    Debug.LogFormat("B: {0}", b.name);

            });

        sequence.Connect();
    }

    /// <summary>
    /// Result:
    ///     CubeA
    ///     CubeB
    ///     CylinderA
    ///     CylinderB
    ///     SphereA
    ///     SphereB
    /// </summary>
    void Test_Zip()
    {
        var sequence = Observable.Range(0, 3).Publish(); // from 0
        var streamA = sequence.SelectMany(x => ResourceStreamA(names[x]));
        var streamB = sequence.SelectMany(x => ResourceStreamB(names[x]));

        // Note that Zip().Take(3) same as Merge()
        Observable.Zip(streamA, streamB).Take(3).Subscribe(
            x => 
            {
                Debug.Log("+++ Zip +++");
                foreach(ResourceRequest r in x)
                {
                    GameObject a = r.asset as GameObject;
                    Debug.LogFormat("name: {0}", a.name);
                    GameObject o = GameObject.Instantiate(a);
                }
            });

        sequence.Connect();
    }

    /// <summary>
    /// Result:
    ///     CubeA
    ///     CubeB
    ///     CylinderA
    ///     CylinderB
    ///     SphereA
    ///     SphereB
    /// </summary>
    void Test_Merge()
    {
        var sequence = Observable.Range(0, 3).Publish(); // from 0
        var streamA = sequence.SelectMany(x => ResourceStreamA(names[x]));
        var streamB = sequence.SelectMany(x => ResourceStreamB(names[x]));

        Observable.Merge(streamA, streamB).Subscribe(
            x => 
            {
                Debug.Log("+++ Merge +++");
                GameObject a = x.asset as GameObject;
                Debug.LogFormat("name: {0}", a.name);
                GameObject o = GameObject.Instantiate(a);
                
            });

        sequence.Connect();
    }

    /// <summary>
    /// Result:
    ///     
    /// </summary>
    void Test_CombineLatest()
    {
        var sequence = Observable.Range(0, 3).Publish(); // from 0
        var streamA = sequence.SelectMany(x => ResourceStreamA(names[x]));
        var streamB = sequence.SelectMany(x => ResourceStreamB(names[x]));

        Observable.CombineLatest(streamA, streamB).Subscribe(
            x =>
            {
                Debug.Log("+++ CombineLatest +++");
                foreach (ResourceRequest r in x)
                {
                    GameObject o = r.asset as GameObject;
                    Debug.LogFormat("name: {0}", o.name);
                }
            });

        sequence.Connect();
    }

    /// <summary>
    /// Result:
    ///     CubeA
    ///     CubeB
    ///     CylinderA
    ///     CubeB
    ///     CylinderA
    ///     CylinderB
    ///     SphereA
    ///     CylinderB
    ///     SphereA
    ///     SphereB
    ///     CubeA
    ///     CylinderA
    ///     SphereA
    /// </summary>
    void Test_Concat()
    {
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
        Observable.Concat(list).ToArray().Subscribe(
            x =>
            {
                Debug.Log("+++ Concat +++");
                foreach(ResourceRequest rq in x)
                {
                    GameObject r = rq.asset as GameObject;
                    Debug.LogFormat("name: {0}", r.name);
                    GameObject o = GameObject.Instantiate(r);
                }
            }, ()=> { /*OnCompleted*/ });

        sequence.Connect();
    }
}

