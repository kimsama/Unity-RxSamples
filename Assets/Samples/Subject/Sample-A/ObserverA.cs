using UnityEngine;
using System.Collections;
using UniRx;

public class ObserverA : MonoBehaviour 
{
    public GameObject target;

	void Start () 
    {
        var targetOnVisible = target.GetComponent<OnVisible>();
        targetOnVisible.OnVisibleObservable
            .Subscribe( x => { Debug.Log(x); });
	}
	
}
