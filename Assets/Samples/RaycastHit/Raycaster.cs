using UnityEngine;
using System.Collections;
using UniRx;

/// <summary>
/// Generate a ray during left mouse button is held down.
/// 
/// </summary>
public class Raycaster : MonoBehaviour 
{
	void Update () 
    {
        // during left mouse button is held down
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitted = hit.collider.gameObject;
                OnRaycastHit raycastHit = hitted.GetComponent<OnRaycastHit>();
                if (raycastHit)
                    raycastHit.RaycastHit();
            }
        }
	}
}
