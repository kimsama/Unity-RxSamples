using UnityEngine;
using System.Collections;
using UniRx;

/// <summary>
/// Subscribe ray cast messages.
/// </summary>
public class Cylinder : MonoBehaviour 
{
	void Start () 
    {
        OnRaycastHit rayHit = GetComponent<OnRaycastHit>();

        // Subscribe ray cast enter message.
        rayHit.onRaycastEnterAsObservable.Subscribe(_ => 
        {
            Debug.Log("Cylinder: onRaycastEnter");
        });

        // Subscribe ray cast stay message.
        rayHit.onRaycastStayAsObservable.Subscribe(_ =>
        {
            Debug.Log("Cylinder: onRaycastStay");
        });

        // Subscribe ray cast exit message.
        rayHit.onRaycastExitAsObservable.Subscribe(_ =>
        {
            Debug.Log("Cylinder: onRaycastExit");
        });
	
	}

}
