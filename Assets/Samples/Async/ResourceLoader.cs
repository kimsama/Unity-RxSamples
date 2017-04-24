using UnityEngine;
using System.Collections;
using UniRx;

public class ResourceLoader : MonoBehaviour 
{

	void Start () 
    {
        // Observable for AsyncOperation
        Resources.LoadAsync<GameObject>("Prefabs/Cube")
            .AsAsyncOperationObservable()
            .Last() // last sequence, when the loading is completed.
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
