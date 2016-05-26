using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// This demonstrates tap and dragging action via rx streams. 
/// In some cases, tap and drag action can be distinguished.
/// </summary>
public class TapAndDrag : MonoBehaviour 
{
    public GameObject cube;

    private readonly float rotationSpeed = 500f;

	void Start () 
    {
        // picking stream
        var mouseDown = this.UpdateAsObservable()
                            .Where(_ => Input.GetMouseButtonDown(0))                        // start to picking
                            .Select(_ => Camera.main.ScreenPointToRay(Input.mousePosition)) // select Ray
                            .Select(ray =>                                                  // check whether something is hit or not
                                {
                                    RaycastHit result;
                                    var isHit = Physics.Raycast(ray, out result);
                                    return Tuple.Create(isHit, result);
                                })
                             .Where(x => x.Item1 && x.Item2.collider.gameObject == cube);

        // end of picking sream
        var mouseUp = this.UpdateAsObservable()
                          .Where(_ => Input.GetMouseButtonUp(0));

        // tap stream
        const float delta = 0.001f;
        this.UpdateAsObservable()
            .SkipUntil(mouseDown)
            .TakeUntil(mouseUp)
            .Select(_ => new Vector2(Input.mousePosition.x, Input.mousePosition.y))
            .Buffer(mouseDown.Throttle(TimeSpan.FromMilliseconds(100))) // exactly saying, it's same as just waiting.
            .RepeatUntilDestroy(this)
            .Subscribe(x =>
            {
                List<Vector2> list = x as List<Vector2>;
                if (list != null && list.Count > 2)
                {
                    Vector2 first = list[0];
                    Vector2 last = list[list.Count - 1];
                    if (Mathf.Abs(first.x - last.x) < delta && Mathf.Abs(first.y - last.y) < delta)
                    {
                        Debug.Log("Tap event");
                    }
                }
            });
            //.Subscribe(list =>
            //    {
            //        if (list != null && list.Count > 2)
            //        {
            //            Vector2 first = list.First(); 
            //            Vector2 last = list.Last();
            //            if (Mathf.Abs(first.x - last.x) < delta && Mathf.Abs(first.y - last.y) < delta)
            //            {
            //                Debug.Log("Tap event");
            //            }
            //        }
            //});

        // drag stream
        this.UpdateAsObservable()
            .SkipUntil(mouseDown)
            .Select(_ =>                                 // get the amount of mouse movement.
                new Vector2(Input.GetAxis("Mouse X"),
                            Input.GetAxis("Mouse Y")))
            .TakeUntil(mouseUp)
            .RepeatUntilDestroy(this)
            .Subscribe(move =>
            {
                cube.transform.rotation =
                        Quaternion.AngleAxis(move.y * rotationSpeed * Time.deltaTime, Vector3.right) *
                        Quaternion.AngleAxis(-move.x * rotationSpeed * Time.deltaTime, Vector3.up) *
                        cube.transform.rotation;
            });
	}
	
}
