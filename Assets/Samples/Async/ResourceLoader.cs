using UnityEngine;
using System.Collections;
using UniRx;

public class ResourceLoader : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        // Observable for AsyncOperation
        Resources.LoadAsync<GameObject>("Prefabs/Cube")
            .AsAsyncOperationObservable()
            .Last() // last sequence is load complete.
            .Do(o => { 
                if (o.asset != null)
                {
                    GameObject go = GameObject.Instantiate(o.asset) as GameObject;
                }
            })
            .Subscribe(
                _ => {
                    // OnNext
                    Debug.Log("OnNext");
                },
                () => {
                    // OnComplete
                    Debug.Log("OnComplete");
                }
            );
	}
	
}
