using UnityEngine;
using System.Collections;
using UniRx;

/// <summary>
/// 
/// </summary>
public class OnVisible : MonoBehaviour 
{
    /// <summary>
    /// A subject which notifies this gameobject when it is inside of the camera frustum.
    /// </summary>
    Subject<GameObject> onVisibleStream = new Subject<GameObject>();

    public IObservable<GameObject> OnVisibleObservable
    {
        get { return onVisibleStream.AsObservable(); }
    }

    /// <summary>
    /// A Unity callback which is called when the gameobject is inside of the camera viewfrustum.
    /// </summary>
    public void OnBecameVisible()
    {
        onVisibleStream.OnNext(this.gameObject);
    }

    void OnWillRenderObject () 
    { 

    #if UNITY_EDITOR 
	    if (Camera.current.name != "SceneCamera" && 
            Camera.current.name != "Preview Camera") 
    #endif 
    	{
            //onVisibleStream.OnNext(this.gameObject);
	    } 
    }
}
