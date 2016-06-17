using UnityEngine;
using System.Collections.Generic;
using UniRx;
using System;

public class ObserverB : MonoBehaviour 
{
    public GameObject group;

	void Start () 
    {
        var onVisibles = group.GetComponentsInChildren<OnVisible>();

        //List<IObservable<GameObject>> list = new List<IObservable<GameObject>>();
        //foreach (OnVisible v in onVisibles)
        //    list.Add(v.OnVisibleObservable);

        var visibleObserver = Observable.Merge(
            // list            
            onVisibles.ToObservable().Select(x=>x.OnVisibleObservable)
            );

        visibleObserver
            .Buffer( visibleObserver.Throttle(TimeSpan.FromMilliseconds(250)) )
            .Subscribe(x => Debug.Log(x.Count));
	}
	
}
